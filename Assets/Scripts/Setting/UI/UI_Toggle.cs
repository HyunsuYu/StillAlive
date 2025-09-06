using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;


public class UI_Toggle : MonoBehaviour
{
    [Header("UIs")]
    [SerializeField] private TMP_Text m_text_SelectedState;

    [SerializeField] private Image m_image_ToggleStateBackground;
    [SerializeField] private Image m_image_ToggleStateIcon;

    [Header("Settings")]
    [SerializeField] private string m_selectedStateText = "On";
    [SerializeField] private string m_unselectedStateText = "Off";

    [SerializeField] private Color m_selectedStateColor = Color.green;
    [SerializeField] private Color m_unselectedStateColor = Color.red;

    private bool m_bisSelected = false;

    [Header("Events")]
    [SerializeField] private UnityEvent m_OnToggle;


    public void Awake()
    {
        Render();
    }

    internal bool Value
    {
        get
        {
            return m_bisSelected;
        }
        set
        {
            m_bisSelected = value;
            m_OnToggle.Invoke();
            Render();
        }
    }

    #region Unity Callbacks
    public void Toggle()
    {
        m_bisSelected = !m_bisSelected;
        Render();

        m_OnToggle.Invoke();
    }
    #endregion

    internal void SetValueWithoutNotify(in bool value)
    {
        m_bisSelected = value;
        Render();
    }

    private void Render()
    {
        if (m_bisSelected)
        {
            m_text_SelectedState.text = m_selectedStateText;
            m_image_ToggleStateBackground.color = m_selectedStateColor;
            m_image_ToggleStateIcon.enabled = true;
        }
        else
        {
            m_text_SelectedState.text = m_unselectedStateText;
            m_image_ToggleStateBackground.color = m_unselectedStateColor;
            m_image_ToggleStateIcon.enabled = false;
        }
    }
}