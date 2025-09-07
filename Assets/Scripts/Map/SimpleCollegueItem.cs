using UnityEngine;


public sealed class SimpleCollegueItem : MonoBehaviour
{
    [SerializeField] private Transform m_transform_PortraitParent;
    [SerializeField] private RectTransform m_rectTransform_HPBar;


    internal void Render(in CardData targetCardData)
    {
        RectTransform portraitRectTransform = CharacterPortraitHelper.CreatePortrait(targetCardData, m_transform_PortraitParent).GetComponent<RectTransform>();
        portraitRectTransform.localScale = new Vector3(0.6f, 0.6f, 1.0f);

        m_rectTransform_HPBar.localScale = new Vector3()
        {
            x = (float)targetCardData.Status.CurHP / targetCardData.Status.MaxHP,
            y = 1.0f,
            z = 1.0f
        };
    }
}