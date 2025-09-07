using CommonUtilLib.ThreadSafe;
using System.Collections.Generic;
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
            }
            else
                slot.GetComponent<Image>().color = new Color32(160, 160, 160, 255);
        }
    }

    public void RemoveCard() // ���õ� ī�� ����
    {
        foreach (var card in _selectedCard)
        {
            if (MyTeams.Contains(card))
            {
                MyTeams.Remove(card);
            }

            BattleCard battleCard = card.GetComponent<BattleCard>();
            if (battleCard != null)
            {
                CardData removeColleague = battleCard.MyData;
                SaveDataBuffer.Instance.Data.CardDatas.Remove(removeColleague);
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
            if (SaveDataBuffer.Instance.Data.CardDatas.Count >= 3)
            {
                Debug.Log("�� �ο� �ʰ�");
                break;
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
