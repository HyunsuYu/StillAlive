using CommonUtilLib.ThreadSafe;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class ItemSelecter : SingleTonForGameObject<ItemSelecter>
{

    public List<GameObject> itemSlots;

    private List<GameObject> _selectedItems = new List<GameObject>();

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
        if (_selectedItems.Contains(itemObject))
        {
            // 이미 선택된 상태면, 리스트에서 제거 (선택 해제)
            _selectedItems.Remove(itemObject);
        }
        else
        {
            // 선택되지 않은 상태면, 리스트에 추가 (선택)
            _selectedItems.Add(itemObject);
        }

        // 변경이 있었으므로 모든 아이템의 시각적 상태를 업데이트
        UpdateAllItemVisuals();
    }

    /// <summary>
    /// 모든 아이템의 색상을 현재 선택 상태에 맞게 업데이트합니다.
    /// </summary>
    private void UpdateAllItemVisuals()
    {
        // 관리 중인 모든 아이템 슬롯을 순회
        foreach (var slot in itemSlots)
        {
            // 이 슬롯이 현재 선택된 아이템 리스트에 포함되어 있는지 확인
            bool isSelected = _selectedItems.Contains(slot);

            // UI.Image 컴포넌트의 색상을 변경 (만약 SpriteRenderer를 쓴다면 GetComponent<SpriteRenderer>()로 변경)
            /*var image = slot.GetComponent<SpriteRenderer>();
            if (image != null)
            {
                image.color = isSelected ? new Color32(85, 85, 85, 255) : new Color32(160, 160, 160, 255);
            }
            */

            if(isSelected)
            {
                slot.GetComponent<Image>().color = new Color32(85, 85, 85, 255);
            }
            else
                slot.GetComponent<Image>().color = new Color32(160, 160, 160, 255);
        }
    }

    /// <summary>
    /// 모든 선택을 해제합니다.
    /// </summary>
    public void ClearSelection()
    {
        _selectedItems.Clear();
        UpdateAllItemVisuals();
    }


    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}
