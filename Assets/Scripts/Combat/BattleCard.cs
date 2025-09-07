using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleCard : MonoBehaviour
{
    private CardData m_myData;
    public CardData MyData => m_myData;
    private Canvas m_canvas;

    [SerializeField] private TMP_Text m_hpText;
    [SerializeField] private TMP_Text m_defenceText;
    [SerializeField] private TMP_Text m_speedText;
    [SerializeField] private TMP_Text m_atkText;
    [SerializeField] private Transform portraitPos;

    private NPCPortrait m_portraitInstance; // 생성된 초상화 인스턴스

    private BattleField m_battleField;

    public void Init(CardData _data)
    {
        m_myData = _data;

        m_battleField = GetComponentInParent<BattleField>();
        if (m_battleField == null)
        {
            m_battleField = FindFirstObjectByType<BattleField>();
        }

        m_canvas = GetComponentInChildren<Canvas>();

        m_canvas.worldCamera = Camera.main;
        // 기존 초상화가 있다면 파괴
        if (m_portraitInstance != null)
        {
            Destroy(m_portraitInstance);
        }

        // 임의로 0번으로 지정
        m_myData.ColorPalleteIndex = 0;

        // CharacterPortraitHelper를 사용하여 초상화 생성 (Resources에서 자동 로드)
        m_portraitInstance = CharacterPortraitHelper.CreatePortrait(m_myData).GetComponent<NPCPortrait>();

        if (m_portraitInstance != null)
        {
            // 생성된 초상화를 이 오브젝트의 자식으로 설정
            m_portraitInstance.transform.SetParent(portraitPos.transform);

            m_portraitInstance.transform.position = portraitPos.position;
            m_portraitInstance.transform.rotation = portraitPos.rotation;
            m_portraitInstance.transform.localScale = new Vector3(0.02f, 0.02f);
        }
        else
        {
            Debug.LogWarning("BattleCard: 초상화 생성에 실패했습니다. Resources 폴더에 'Portrait' 프리팹과 'NPCLookPart' 에셋이 있는지 확인해주세요.");
        }

        m_hpText.text = m_myData.Status.CurHP.ToString();
        m_defenceText.text = m_myData.Status.DefencePower.ToString();
        m_speedText.text = m_myData.Status.Speed.ToString();
        m_atkText.text = m_myData.Status.AttackPower.ToString();
    }

    public int GetSpeed()
    {
        return m_myData.Status.Speed;
    }

    public int GetAttackPower()
    {
        return m_myData.Status.AttackPower;
    }

    public int GetCurrentHP()
    {
        return m_myData.Status.CurHP;
    }

    public int GetMaxHP()
    {
        return m_myData.Status.MaxHP;
    }

    public NPCPortrait GetPortraitInstance()
    {
        return m_portraitInstance;
    }

    public bool IsTraitor()
    {
        return m_myData.BIsTraitor;
    }

    public void TakeDamage(int damage)
    {
        int finalDamage = damage - m_myData.Status.DefencePower;
        if (finalDamage < 1) finalDamage = 1; // 최소 1의 데미지 보장
        m_myData.Status.CurHP -= finalDamage;

        if (m_myData.Status.CurHP < 0)
        {
            m_myData.Status.CurHP = 0;
        }
        // Debug.Log($"{name}이(가) {finalDamage}의 피해를 입었습니다. 남은 체력: {m_myData.Status.CurHP}");

        // 피격 효과 재생
        if (m_portraitInstance != null)
        {
            StartCoroutine(m_portraitInstance.PlayHitEffect());
        }

        // UI 업데이트 (HP바와 텍스트)
        m_battleField.UpdateTeamMemberStatusUI(this);      
    }

    public bool IsDead()
    {
        return m_myData.Status.CurHP <= 0;
    }

    private void OnDestroy()
    {
        // 오브젝트가 파괴될 때 초상화도 함께 정리
        if (m_portraitInstance != null)
        {
            Destroy(m_portraitInstance);
        }
    }
}