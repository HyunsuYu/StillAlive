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

            if(isSelected)
            {
                slot.GetComponent<Image>().color = new Color32(85, 85, 85, 255);
            }
            else
                slot.GetComponent<Image>().color = new Color32(160, 160, 160, 255);
        }
    }

    /// <summary>
    /// ��� ������ �����մϴ�.
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
