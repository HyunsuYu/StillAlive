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
            // 이미 선택된 상태면, 리스트에서 제거 (선택 해제)
            _selectedCard.Remove(itemObject);
        }
        else
        {
            // 선택되지 않은 상태면, 리스트에 추가 (선택)
            _selectedCard.Add(itemObject);
        }

        // 변경이 있었으므로 모든 카드의 시각적 상태를 업데이트
        UpdateAllItemVisuals();
    }

    /// <summary>
    /// 모든 카드의 색상을 현재 선택 상태에 맞게 업데이트합니다.
    /// </summary>
    private void UpdateAllItemVisuals()
    {
        // 관리 중인 모든 카드 슬롯을 순회
        foreach (var slot in CardSlots)
        {
            // 이 슬롯이 현재 선택된 카드 리스트에 포함되어 있는지 확인
            bool isSelected = _selectedCard.Contains(slot);

            //선택된 카드의 색상 변경
            if (isSelected)
            {
                slot.GetComponent<Image>().color = new Color32(85, 85, 85, 255);
            }
            else
                slot.GetComponent<Image>().color = new Color32(160, 160, 160, 255);
        }
    }

    public void RemoveCard() // 선택된 카드 제거
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
                Debug.LogError("선택된 카드에 BattleCard 컴포넌트가 없습니다.");
            }
        }

        SaveDataBuffer.Instance.TrySaveData();
        foreach (var card in _selectedCard)
        {
            card.SetActive(false);
        }
        ClearSelection();
    }

    public void GetCard() // 선택된 카드 팀에 추가
    {
        if (_selectedCard.Count == 0)
        {
            return;
        }

        foreach (var card in _selectedCard)
        {
            if (SaveDataBuffer.Instance.Data.CardDatas.Count >= 3)
            {
                Debug.Log("팀 인원 초과");
                break;
            }

            BattleCard battleCard = card.GetComponent<BattleCard>();
            CardData newColleague = battleCard.MyData;
            SaveDataBuffer.Instance.Data.CardDatas.Add(newColleague);
            Debug.Log("팀에 추가됨" + newColleague.Status.AttackPower.ToString());
            SaveDataBuffer.Instance.TrySaveData();

        }
        ClearSelection();
        Explolor.Instance.exState = Explolor.ExplolorState.None;
        SceneManager.LoadScene("Map");
    }

    public void KeepTeam() // 팀 유지하고 맵으로 돌아감
    {
        SaveDataBuffer.Instance.TrySaveData();
        ClearSelection();
        SceneManager.LoadScene("Map");
    }

    /// <summary>
    /// 모든 선택을 해제합니다.
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
