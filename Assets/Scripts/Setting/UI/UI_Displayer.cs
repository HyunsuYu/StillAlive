using System;
using System.Collections.Generic;
using System.Linq;

using TMPro;
using UnityEngine;
using UnityEngine.Events;


public sealed class UI_Displayer : MonoBehaviour
{
    [Header("UIs")]
    [SerializeField] private TMP_Text m_text_Info;

    [Header("Settings")]
    private int m_currentValue = 0;

    private Dictionary<int, string> m_valueToInfoTable = new Dictionary<int, string>();

    [Header("Events")]
    [SerializeField] private UnityEvent m_on2LeftItem;
    [SerializeField] private UnityEvent m_On2RightItem;


    public void Awake()
    {
        if (m_text_Info == null)
        {
            Debug.LogError("Info Text is not assigned!");
        }
    }

    internal int Value
    {
        get
        {
            return m_currentValue;
        }
        set
        {
            if (m_valueToInfoTable.Count == 0)
            {
                Debug.LogError("Value to info table is empty. Please initialize the displayer first.");
                return;
            }

            if (value < m_currentValue)
            {
                Turn2LeftItem();
            }
            else if (value > m_currentValue)
            {
                Turn2RightItem();
            }

            m_currentValue = value;
        }
    }

    #region Unity Callbacks
    public void Turn2LeftItem()
    {
        int curValueIndex = m_valueToInfoTable.Keys.ToList().IndexOf(m_currentValue);
        if (curValueIndex - 1 >= 0)
        {
            m_currentValue = m_valueToInfoTable.Keys.ToList()[curValueIndex - 1];
        }
        else
        {
            m_currentValue = m_valueToInfoTable.Keys.Last();
        }

        m_text_Info.text = m_valueToInfoTable[m_currentValue];

        m_on2LeftItem.Invoke();
    }
    public void Turn2RightItem()
    {
        int curValueIndex = m_valueToInfoTable.Keys.ToList().IndexOf(m_currentValue);
        if (curValueIndex + 1 < m_valueToInfoTable.Count)
        {
            m_currentValue = m_valueToInfoTable.Keys.ToList()[curValueIndex + 1];
        }
        else
        {
            m_currentValue = m_valueToInfoTable.Keys.First();
        }

        m_text_Info.text = m_valueToInfoTable[m_currentValue];

        m_On2RightItem.Invoke();
    }
    #endregion

    internal void Init(in Type targetEnum)
    {
        if (Enum.GetNames(targetEnum).Length == 0)
        {
            Debug.LogError("The provided type is not an enum or has no values.");
            return;
        }

        foreach (var value in Enum.GetValues(targetEnum))
        {
            m_valueToInfoTable.Add((int)value, value.ToString());
        }

        m_currentValue = m_valueToInfoTable.Keys.First();
    }
    internal void ClearOptions()
    {
        m_valueToInfoTable.Clear();
        m_text_Info.text = string.Empty;
        m_currentValue = 0;
    }
    internal void AddOptions(in List<string> options)
    {
        if (options == null || options.Count == 0)
        {
            Debug.LogError("Options list is null or empty.");
            return;
        }

        m_valueToInfoTable.Clear();
        for (int index = 0; index < options.Count; index++)
        {
            m_valueToInfoTable.Add(index, options[index]);
        }

        if (m_valueToInfoTable.Count > 0)
        {
            m_text_Info.text = m_valueToInfoTable[m_currentValue];
        }
    }

    internal void SetValueWithoutNotify(in int targetValue)
    {
        if (m_valueToInfoTable.Count == 0)
        {
            Debug.LogError("Value to info table is empty. Please initialize the displayer first.");
            return;
        }

        if (m_valueToInfoTable.TryGetValue(targetValue, out string infoText))
        {
            m_text_Info.text = infoText;
        }
        else
        {
            Debug.LogError($"Value {targetValue} not found in value to info table.");
        }

        m_currentValue = targetValue;
    }
}