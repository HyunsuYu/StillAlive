using CommonUtilLib.ThreadSafe;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardSelecter : SingleTonForGameObject<CardSelecter>
{
    public List<GameObject> CardSlots;
    public List<GameObject> MyTeams;
    private List<GameObject> _selectedCard = new List<GameObject>();


    private void Awake()
    {
        SetInstance(this);
    }

    private void Start()
    {
        UpdateAllItemVisuals();
    }

    public void ToggleSelection(GameObject itemObject)
    {
        if (_selectedCard.Contains(itemObject))
        {
            // �̹� ���õ� ���¸�, ����Ʈ���� ���� (���� ����)
            _selectedCard.Remove(itemObject);
        }
        else
        {
            // ���õ��� ���� ���¸�, ����Ʈ�� �߰� (����)
            _selectedCard.Add(itemObject);
        }

        // ������ �־����Ƿ� ��� ī���� �ð��� ���¸� ������Ʈ
        UpdateAllItemVisuals();
    }

    /// <summary>
    /// ��� ī���� ������ ���� ���� ���¿� �°� ������Ʈ�մϴ�.
    /// </summary>
    private void UpdateAllItemVisuals()
    {
        // ���� ���� ��� ī�� ������ ��ȸ
        foreach (var slot in CardSlots)
        {
            // �� ������ ���� ���õ� ī�� ����Ʈ�� ���ԵǾ� �ִ��� Ȯ��
            bool isSelected = _selectedCard.Contains(slot);

            //���õ� ī���� ���� ����
            if (isSelected)
            {
                slot.GetComponent<Image>().color = new Color32(85, 85, 85, 255);
                if (slot.GetComponentInChildren<BattleCard>() != null)
                {
                    slot.GetComponentInChildren<BattleCard>().GetComponentInChildren<Image>().color = new Color32(85, 85, 85, 255);
                }
            }
            else
            {
                slot.GetComponent<Image>().color = new Color32(160, 160, 160, 255);
                if (slot.GetComponentInChildren<BattleCard>() != null)
                {
                    slot.GetComponentInChildren<BattleCard>().GetComponentInChildren<Image>().color = new Color32(160, 160, 160, 255);
                }
            }
        }

        foreach (var Myteam in MyTeams)
        {
            // �� ������ ���� ���õ� ī�� ����Ʈ�� ���ԵǾ� �ִ��� Ȯ��
            bool isSelected = _selectedCard.Contains(Myteam);
            //���õ� ī���� ���� ����
            if (isSelected)
            {
                Myteam.GetComponent<Image>().color = new Color32(85, 85, 85, 255);
                if (Myteam.GetComponentInChildren<BattleCard>() != null)
                {
                    Myteam.GetComponentInChildren<BattleCard>().GetComponentInChildren<Image>().color = new Color32(85, 85, 85, 255);
                }
            }
            else
            {
                Myteam.GetComponent<Image>().color = new Color32(160, 160, 160, 255);
                if (Myteam.GetComponentInChildren<BattleCard>() != null)
                {
                    Myteam.GetComponentInChildren<BattleCard>().GetComponentInChildren<Image>().color = new Color32(160, 160, 160, 255);
                }
            }
        }
    }

    public void RemoveCard() // ���õ� ī�� ����
    {
        foreach (var card in _selectedCard)
        {
            if (CardSlots.Contains(card))
            {
                ClearSelection();
                return;
            }

            BattleCard battleCard = card.GetComponentInChildren<BattleCard>();
            if (battleCard != null)
            {
                SaveData curSaveData = SaveDataBuffer.Instance.Data;
                CardData removeColleague = battleCard.MyData;
                removeColleague.Status.CurHP = 0; // ü���� 0���� �����Ͽ� ���� ǥ��
                Debug.Log(SaveDataInterface.GetCardIndex(battleCard.MyData));
                curSaveData.CardDatas[SaveDataInterface.GetCardIndex(battleCard.MyData)] = removeColleague;
                SaveDataBuffer.Instance.TrySetData(curSaveData);
                SaveDataBuffer.Instance.TrySaveData();
            }
            else
            {
                Debug.LogError("���õ� ī�忡 BattleCard ������Ʈ�� �����ϴ�.");
            }
        }

        SaveDataBuffer.Instance.TrySaveData();
        foreach (var card in _selectedCard)
        {
            card.SetActive(false);
        }
        ClearSelection();
    }

    public void GetCard() // ���õ� ī�� ���� �߰�
    {
        if (_selectedCard.Count == 0)
        {
            return;
        }

        foreach (var card in _selectedCard)
        {
            if (SaveDataInterface.GetAliveCardInfos().Count() >= 4)
            {
                Debug.Log("�� �ο� �ʰ�");
                return;
            }

            BattleCard battleCard = card.GetComponent<BattleCard>();
            CardData newColleague = battleCard.MyData;
            SaveDataBuffer.Instance.Data.CardDatas.Add(newColleague);
            Debug.Log("���� �߰���" + newColleague.Status.AttackPower.ToString());
            SaveDataBuffer.Instance.TrySaveData();

        }
        ClearSelection();
        Explolor.Instance.exState = Explolor.ExplolorState.None;
        SceneManager.LoadScene("Map");
    }

    public void KeepTeam() // �� �����ϰ� ������ ���ư�
    {
        SaveDataBuffer.Instance.TrySaveData();
        ClearSelection();
        SceneManager.LoadScene("Map");
    }

    /// <summary>
    /// ��� ������ �����մϴ�.
    /// </summary>
    public void ClearSelection()
    {
        _selectedCard.Clear();
        UpdateAllItemVisuals();
    }


    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}
