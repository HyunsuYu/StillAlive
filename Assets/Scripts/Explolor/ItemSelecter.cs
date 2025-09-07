using CommonUtilLib.ThreadSafe;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            // �̹� ���õ� ���¸�, ����Ʈ���� ���� (���� ����)
            _selectedItems.Remove(itemObject);
        }
        else
        {
            // ���õ��� ���� ���¸�, ����Ʈ�� �߰� (����)
            _selectedItems.Add(itemObject);
        }

        // ������ �־����Ƿ� ��� �������� �ð��� ���¸� ������Ʈ
        UpdateAllItemVisuals();
    }

    /// <summary>
    /// ��� �������� ������ ���� ���� ���¿� �°� ������Ʈ�մϴ�.
    /// </summary>
    private void UpdateAllItemVisuals()
    {
        // ���� ���� ��� ������ ������ ��ȸ
        foreach (var slot in itemSlots)
        {
            // �� ������ ���� ���õ� ������ ����Ʈ�� ���ԵǾ� �ִ��� Ȯ��
            bool isSelected = _selectedItems.Contains(slot);

            // UI.Image ������Ʈ�� ������ ���� (���� SpriteRenderer�� ���ٸ� GetComponent<SpriteRenderer>()�� ����)
            /*var image = slot.GetComponent<SpriteRenderer>();
            if (image != null)
            {
                image.color = isSelected ? new Color32(85, 85, 85, 255) : new Color32(160, 160, 160, 255);
            }
            */

            if (isSelected)
            {
                slot.GetComponent<Image>().color = new Color32(85, 85, 85, 255);
            }
            else
                slot.GetComponent<Image>().color = new Color32(160, 160, 160, 255);
        }
    }

    public void TrashItem()
    {
        foreach (var item in _selectedItems)
        {
            itemSlots.Remove(item);
            Destroy(item);
        }
        _selectedItems.Clear();
        UpdateAllItemVisuals();
    }

    public void GetItem()
    {
        SaveData curSaveData = SaveDataBuffer.Instance.Data;
        foreach (var item in _selectedItems)
        {
            curSaveData.ItemAmountTable[Explolor.Instance.ItemId[itemSlots.IndexOf(item)]]++;
        }
        SaveDataBuffer.Instance.TrySetData(curSaveData);
        SaveDataBuffer.Instance.TrySaveData();

        _selectedItems.Clear();
        UpdateAllItemVisuals();
        SceneManager.LoadScene("Map");
    }

    /// <summary>
    /// ��� ������ �����մϴ�.
    /// </summary>
    public void ClearSelection()
    {
        _selectedItems.Clear();
        UpdateAllItemVisuals();
    }

    public void ShowAllItems()
    {
        Debug.Log("--- �� �κ��丮 ---");
        foreach (KeyValuePair<int, int> item in SaveDataBuffer.Instance.Data.ItemAmountTable)
        {
            // item.Key�� ������ ID, item.Value�� �����Դϴ�.
            Debug.Log($"������: {item.Key}, ����: {item.Value}");

        }
        Debug.Log(Explolor.Instance.ItemId[0].ToString());
        Debug.Log("-----------------");
    }
    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}
