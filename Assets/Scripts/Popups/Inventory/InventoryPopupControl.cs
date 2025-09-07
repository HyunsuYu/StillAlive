using UnityEngine;
using UnityEngine.UI;

using CommonUtilLib.ThreadSafe;
using UnityEngine.Events;


public sealed class InventoryPopupControl : SingleTonForGameObject<InventoryPopupControl>
{
    [SerializeField] private Animator m_animator_InventoryPopup;

    [SerializeField] private InventoryItem[] m_inventoryItems;

    [SerializeField] private ItemTypes m_itemType;
    [SerializeField] private SpriteRenderer m_spriteRenderer_DraggingItemIcon;

    [SerializeField] private bool m_bisCombatScene = false;

    private bool m_bisPopupOpened = false;

    private int m_draggingItemIndex = -1;
    private bool m_bisItemDragging = false;

    [SerializeField] private UnityEvent m_onDragEnd;


    public void Awake()
    {
        SetInstance(this);
    }
    public void Start()
    {
        Render();
    }
    public void Update()
    {
        if(!m_bisCombatScene)
        {
            m_spriteRenderer_DraggingItemIcon.gameObject.SetActive(false);
            return;
        }

        if(m_bisItemDragging)
        {
            m_spriteRenderer_DraggingItemIcon.gameObject.SetActive(true);

            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0.0f;
            m_spriteRenderer_DraggingItemIcon.transform.position = pos;
            m_spriteRenderer_DraggingItemIcon.sprite = m_itemType.ItemDatas[m_draggingItemIndex].ItemSprite;
        }
        else
        {
            m_spriteRenderer_DraggingItemIcon.gameObject.SetActive(false);
        }
    }

    internal ItemTypes ItemType
    {
        get
        {
            return m_itemType;
        }
    }

    internal int DraggingItemIndex
    {
        get
        {
            return m_draggingItemIndex;
        }
        set
        {
            m_draggingItemIndex = value;
        }
    }
    internal bool BisItemDragging
    {
        set
        {
            m_bisItemDragging = value;
        }
    }

    #region Unity Callbacks
    public void OpenAndClosePopup()
    {
        if(m_bisPopupOpened)
        {
            m_animator_InventoryPopup.SetTrigger("Close");
        }
        else
        {
            m_animator_InventoryPopup.SetTrigger("Open");
        }
        m_bisPopupOpened = !m_bisPopupOpened;
    }
    #endregion

    internal void SuccessSetItem2Card()
    {
        SaveData curSaveData = SaveDataBuffer.Instance.Data;
        curSaveData.ItemAmountTable[m_draggingItemIndex]--;
        SaveDataBuffer.Instance.TrySetData(curSaveData);
        SaveDataBuffer.Instance.TrySaveData();

        Render();
    }
    internal void Render()
    {
        foreach(InventoryItem item in m_inventoryItems)
        {
            item.Render();
        }
    }

    internal void InvokeEndDragEvent()
    {
        m_onDragEnd.Invoke();
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}