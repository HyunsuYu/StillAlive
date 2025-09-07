using TMPro;
using UnityEngine;


public sealed class SimpleCollegueItem : MonoBehaviour
{
    [SerializeField] private Transform m_transform_PortraitParent;
    [SerializeField] private TMP_Text m_text_CurHP;


    internal void Render(in CardData targetCardData)
    {
        RectTransform portraitRectTransform = CharacterPortraitHelper.CreatePortrait(targetCardData, m_transform_PortraitParent).GetComponent<RectTransform>();
        portraitRectTransform.localScale = new Vector3(0.6f, 0.6f, 1.0f);

        m_text_CurHP.text = $"{targetCardData.Status.CurHP} / {targetCardData.Status.MaxHP}";
    }
}