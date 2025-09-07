using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public sealed class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image m_image_ItemIcon;
    [SerializeField] private TMP_Text m_text_ItemCount;

    [SerializeField] private int m_itemIndex;


    public void OnBeginDrag(PointerEventData eventData)
    {
        if(m_itemIndex == 0)
        {
            return;
        }

        InventoryPopupControl.Instance.DraggingItemIndex = m_itemIndex;
        InventoryPopupControl.Instance.BisItemDragging = true;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (m_itemIndex == 0)
        {
            return;
        }

        InventoryPopupControl.Instance.DraggingItemIndex = m_itemIndex;
        InventoryPopupControl.Instance.BisItemDragging = true;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        InventoryPopupControl.Instance.BisItemDragging = false;

        InventoryPopupControl.Instance.InvokeEndDragEvent();
    }

    internal void Render()
    {
        m_image_ItemIcon.sprite = InventoryPopupControl.Instance.ItemType.ItemDatas[m_itemIndex].ItemSprite;
        m_text_ItemCount.text = SaveDataBuffer.Instance.Data.ItemAmountTable[m_itemIndex].ToString();
    }
}