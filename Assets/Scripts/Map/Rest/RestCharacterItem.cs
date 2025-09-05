using UnityEngine;
using TMPro;


public sealed class RestCharacterItem : MonoBehaviour
{
    [SerializeField] private Transform m_transform_CharacterPortraitParent;

    [SerializeField] private TMP_Text m_text_PrevHP;
    [SerializeField] private TMP_Text m_text_HPAddAmount;

    [SerializeField] private Animator m_animator_TextAdd;

    private int m_targetHPValue;


    internal void Init(in int cardIndex)
    {



        m_text_HPAddAmount.gameObject.SetActive(true);
    }

    private void PlayAnimation()
    {
        m_animator_TextAdd.Play("HPAdd");
    }
    private void DisplayAddedHP()
    {
        m_text_PrevHP.text = m_text_HPAddAmount.text;
        m_text_HPAddAmount.gameObject.SetActive(false);
    }
}