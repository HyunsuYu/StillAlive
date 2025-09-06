using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Slot : MonoBehaviour
{
    [SerializeField] private Image m_Image;
    [SerializeField] private TMP_Text m_Count;

    private ItemTypes m_ItemType;

    internal ItemTypes m_ItemTypes
    {
        get { return m_ItemType; }
        set { m_ItemType = value; }
    }
}
