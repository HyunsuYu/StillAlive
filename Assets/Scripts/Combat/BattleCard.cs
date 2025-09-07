using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleCard : MonoBehaviour
{
    private CardData m_persistentData; // 영구 저장될 원본 데이터 (아이템 스탯 포함)
    private CardData m_battleData;     // 이번 전투에만 사용될 임시 데이터 (디버프 등 적용)
    private bool isDebuffed = false;

    public CardData MyData => m_persistentData;

    private Canvas m_canvas;
    [SerializeField] private Image m_cardImage;
    [SerializeField] private TMP_Text m_hpText;
    [SerializeField] private TMP_Text m_defenceText;
    [SerializeField] private TMP_Text m_speedText;
    [SerializeField] private TMP_Text m_atkText;
    [SerializeField] private Transform m_portraitPos;
    [SerializeField] private Transform m_itemContainer;
    [SerializeField] private List<GameObject> m_itemSlots;
    private NPCPortrait m_portraitInstance;
    private BattleField m_battleField;


    public void Init(CardData originalData, bool isUI = false)
    {
        m_persistentData = originalData;
        m_battleData = originalData;
        isDebuffed = false;

        m_canvas = GetComponentInChildren<Canvas>();
        if (m_canvas != null && !isUI) m_canvas.worldCamera = Camera.main;

        m_battleField = FindFirstObjectByType<BattleField>();
        if (m_battleField)
        {
            if (!originalData.BIsMonster)
                m_itemContainer.gameObject.SetActive(true);
            else
            {
                m_cardImage.sprite = Resources.Load<Sprite>($"Card/괴물카드");
                m_itemContainer.gameObject.SetActive(false);
            }
            ApplyDiseaseEffects();

            RefreshItemSlotsUI();
        }

        CreatePortrait(isUI);
        UpdateStatusUI();
    }

    /// <summary>
    /// 전투 시작 시 질병 효과를 전투용 데이터(m_battleData)에 적용합니다.
    /// </summary>
    private void ApplyDiseaseEffects()
    {
        if ((m_battleData.Diseases & CardData.DiseasesType.ColdFever) != 0)
        {
            m_cardImage.sprite = Resources.Load<Sprite>($"Card/열감기");
            isDebuffed = true;
            m_battleData.Status.CurHP = Mathf.Max(1, m_battleData.Status.CurHP / 2);
            m_battleData.Status.MaxHP = Mathf.Max(1, m_battleData.Status.MaxHP / 2);
            m_battleData.Status.DefencePower = Mathf.Max(1, m_battleData.Status.DefencePower / 2);
            m_battleData.Status.AttackPower = Mathf.Max(1, m_battleData.Status.AttackPower / 2);
            m_battleData.Status.Speed = Mathf.Max(1, m_battleData.Status.Speed / 2);
        }
    }

    /// <summary>
    /// 질병 치료 시 원래 능력치로 복구하는 함수
    /// </summary>
    private void RestoreOriginalStats()
    {
        isDebuffed = false;
        int hpBeforeRestore = m_battleData.Status.CurHP;
        m_battleData.Status = m_persistentData.Status;
        m_battleData.Status.CurHP = hpBeforeRestore; 
        UpdateStatusUI();
    }

    /// <summary>
    /// 전투 후 저장할 최종 CardData를 반환하는 함수
    /// </summary>
    public CardData GetFinalCardDataForSave()
    {
        m_persistentData.Status.CurHP = m_battleData.Status.CurHP;
        m_persistentData.Items = m_battleData.Items;
        m_persistentData.Diseases = m_battleData.Diseases;
        return m_persistentData;
    }

    /// <summary>
    /// 인벤토리에서 아이템을 드롭했을 때 호출되는 함수
    /// </summary>
    public void ApplyItem(int itemIndex)
    {
        if (m_persistentData.Items != null && m_persistentData.Items.Count >= 4) return;

        ItemTypes.ItemData itemData = InventoryPopupControl.Instance.ItemType.ItemDatas[itemIndex];
        bool isConsumed = false;
        bool isEquipped = false;

        foreach (var buf in itemData.BufInfos)
        {
            switch (buf.BufEffect)
            {
                case ItemTypes.BufEffectType.RemoveDisease_All:
                    if (isDebuffed) RestoreOriginalStats();
                    m_persistentData.Diseases = CardData.DiseasesType.None;
                    m_battleData.Diseases = CardData.DiseasesType.None;
                    isConsumed = true;
                    break;
                case ItemTypes.BufEffectType.PlusHP:
                    m_battleData.Status.CurHP = Mathf.Min(m_battleData.Status.MaxHP, m_battleData.Status.CurHP + buf.BufDegree);
                    isConsumed = true;
                    break;
                case ItemTypes.BufEffectType.PlusAttack:
                case ItemTypes.BufEffectType.PlusSpeed:
                case ItemTypes.BufEffectType.Boom:
                    if (!isEquipped)
                    {
                        var newItem = new CardData.AttachedItemData { ItemIndex = itemIndex, CurDuruvility = itemData.MaxDurability };

                        if (m_persistentData.Items == null) m_persistentData.Items = new List<CardData.AttachedItemData>();
                        m_persistentData.Items.Add(newItem);
                        ApplyStatsToData(ref m_persistentData, newItem, 1);

           
                        int hpBeforeUpdate = m_battleData.Status.CurHP;
                        m_battleData = m_persistentData;
                        m_battleData.Status.CurHP = hpBeforeUpdate;

                        if (isDebuffed) ApplyDiseaseEffects();

                        isEquipped = true;
                    }
                    break;
            }
        }
        if (isConsumed || isEquipped) InventoryPopupControl.Instance.SuccessSetItem2Card();
        RefreshItemSlotsUI();
        UpdateStatusUI();
        m_battleField.UpdateTeamMemberStatusUI(this);
    }

    /// <summary>
    /// 공격 직후 호출되어 아이템 효과(내구도, Boom 등)를 처리합니다.
    /// </summary>
    public void ProcessAttackEffects(BattleCard target, List<BattleCard> allEnemies, List<BattleCard> allAllies)
    {
        if (m_battleData.Items == null || m_battleData.Items.Count == 0) return;
        List<CardData.AttachedItemData> itemsToDestroy = new List<CardData.AttachedItemData>();

        // Boom 효과 처리
        foreach (var attachedItem in m_battleData.Items)
        {
            ItemTypes.ItemData itemData = InventoryPopupControl.Instance.ItemType.ItemDatas[attachedItem.ItemIndex];
            if (itemData.BufInfos.Any(b => b.BufEffect == ItemTypes.BufEffectType.Boom))
            {
                int splashDamage = itemData.BufInfos.FirstOrDefault(b => b.BufEffect == ItemTypes.BufEffectType.PlusAttack).BufDegree / 2;
                foreach (var enemy in allEnemies.Where(e => e != target && e != null && !e.IsDead())) enemy.TakeDamage(splashDamage);
                foreach (var ally in allAllies.Where(a => a != this && a != null && !a.IsDead())) ally.TakeDamage(splashDamage);
            }
        }

        // 내구도 감소 및 파괴 처리
        for (int i = 0; i < m_battleData.Items.Count; i++)
        {
            var item = m_battleData.Items[i];
            item.CurDuruvility--;
            m_battleData.Items[i] = item;
            if (item.CurDuruvility <= 0) itemsToDestroy.Add(item);
        }

        if (itemsToDestroy.Count > 0)
        {
            foreach (var item in itemsToDestroy) UnequipItem(item);
            RefreshItemSlotsUI();
            UpdateStatusUI();
            m_battleField.UpdateTeamMemberStatusUI(this);
        }
    }

    /// <summary>
    /// 내구도가 다 닳은 아이템을 파괴하고 능력치를 되돌립니다.
    /// </summary>
    private void UnequipItem(CardData.AttachedItemData itemToRemove)
    {
        // 1. 원본 데이터에서 아이템 제거 및 스탯 영구 감소
        m_persistentData.Items.Remove(itemToRemove);
        ApplyStatsToData(ref m_persistentData, itemToRemove, -1);

        // 2. 전투 데이터는 원본 데이터를 다시 복사하여 갱신
        int hpBeforeUpdate = m_battleData.Status.CurHP;
        m_battleData = m_persistentData;
        m_battleData.Status.CurHP = hpBeforeUpdate;

        // 3. 디버프가 있었다면 전투 데이터에 다시 적용
        if (isDebuffed) ApplyDiseaseEffects();
    }

    /// <summary>
    /// 특정 CardData에 아이템 능력치를 적용하거나 제거하는 함수
    /// </summary>
    private void ApplyStatsToData(ref CardData data, CardData.AttachedItemData item, int multiplier)
    {
        ItemTypes.ItemData itemData = InventoryPopupControl.Instance.ItemType.ItemDatas[item.ItemIndex];
        foreach (var buf in itemData.BufInfos)
        {
            switch (buf.BufEffect)
            {
                case ItemTypes.BufEffectType.PlusAttack:
                    data.Status.AttackPower += buf.BufDegree * multiplier;
                    break;
                case ItemTypes.BufEffectType.PlusSpeed:
                    data.Status.Speed += buf.BufDegree * multiplier;
                    break;
            }
        }
    }

    /// <summary>
    /// 현재 장착된 아이템들을 UI 슬롯에 다시 그립니다.
    /// </summary>
    private void RefreshItemSlotsUI()
    {
        foreach (var slot in m_itemSlots)
        {
            foreach (Transform child in slot.transform) Destroy(child.gameObject);
        }
        if (m_battleData.Items == null) return;
        for (int i = 0; i < m_battleData.Items.Count; i++)
        {
            if (i < m_itemSlots.Count)
            {
                var itemData = InventoryPopupControl.Instance.ItemType.ItemDatas[m_battleData.Items[i].ItemIndex];
                GameObject iconObj = new GameObject("ItemIcon", typeof(Image));
                iconObj.GetComponent<Image>().sprite = itemData.ItemSprite;
                iconObj.transform.SetParent(m_itemSlots[i].transform, false);
                RectTransform iconRect = iconObj.GetComponent<RectTransform>();
                iconRect.anchorMin = Vector2.zero; iconRect.anchorMax = Vector2.one;
                iconRect.offsetMin = Vector2.zero; iconRect.offsetMax = Vector2.zero;
            }
        }
    }

    /// <summary>
    /// 초상화를 생성합니다.
    /// </summary>
    private void CreatePortrait(bool isUI)
    {
        if (m_portraitInstance != null) Destroy(m_portraitInstance.gameObject);

        // 초상화는 전투용 데이터(디버프 시 모습이 바뀔 수 있으므로)를 기준으로 생성
        m_battleData.ColorPalleteIndex = 0;
        GameObject portraitObj = CharacterPortraitHelper.CreatePortrait(m_battleData);
        if (portraitObj != null)
        {
            m_portraitInstance = portraitObj.GetComponent<NPCPortrait>();
            m_portraitInstance.transform.SetParent(m_portraitPos, false);
            m_portraitInstance.transform.localPosition = Vector3.zero;
            if (!isUI) m_portraitInstance.transform.localScale = new Vector3(0.02f, 0.02f);
        }
    }

    /// <summary>
    /// UI 텍스트를 현재 전투 능력치로 업데이트합니다.
    /// </summary>
    public void UpdateStatusUI()
    {
        m_hpText.text = $"{m_battleData.Status.CurHP}/{m_battleData.Status.MaxHP}";
        m_defenceText.text = $"방어력\n{m_battleData.Status.DefencePower}";
        m_speedText.text = $"스피드\n{m_battleData.Status.Speed}";
        m_atkText.text = $"공격력\n{m_battleData.Status.AttackPower}";
    }

    /// <summary>
    /// 피해를 입었을 때 호출됩니다.
    /// </summary>
    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(1, damage - m_battleData.Status.DefencePower);
        m_battleData.Status.CurHP -= finalDamage;
        if (m_battleData.Status.CurHP < 0) m_battleData.Status.CurHP = 0;
        if (m_portraitInstance != null) StartCoroutine(m_portraitInstance.PlayHitEffect());
        UpdateStatusUI();
        m_battleField.UpdateTeamMemberStatusUI(this);
    }

    // --- Getter 함수들 (전투용 데이터를 기준으로 값을 반환) ---
    public int GetSpeed() => m_battleData.Status.Speed;
    public int GetAttackPower() => m_battleData.Status.AttackPower;
    public int GetCurrentHP() => m_battleData.Status.CurHP;
    public int GetMaxHP() => m_battleData.Status.MaxHP;
    public bool IsDead() => m_battleData.Status.CurHP <= 0;
    public bool IsTraitor() => m_battleData.BIsTraitor;
    public NPCPortrait GetPortraitInstance() => m_portraitInstance;

    private void OnDestroy()
    {
        if (m_portraitInstance != null) Destroy(m_portraitInstance.gameObject);
    }
}