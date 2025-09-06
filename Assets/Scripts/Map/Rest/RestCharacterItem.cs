using UnityEngine;
using TMPro;


public sealed class RestCharacterItem : MonoBehaviour
{
    [SerializeField] private Transform m_transform_CharacterPortraitParent;

    [SerializeField] private TMP_Text m_text_PrevHP;
    [SerializeField] private TMP_Text m_text_HPAddAmount;

    [SerializeField] private Animator m_animator_TextAdd;

    private int m_targetHPValue;

    [Header("Delays")]
    [SerializeField] private float m_animationDelay;
    [SerializeField] private float m_HPTextChangeDelay;


    internal void Init(in int cardIndex)
    {
        CardData cardData = SaveDataBuffer.Instance.Data.CardDatas[cardIndex];
        //CharacterPortraitHelper.CreatePortrait(cardData, m_transform_CharacterPortraitParent);

        m_text_PrevHP.text = $"{cardData.Status.CurHP}/{cardData.Status.MaxHP}";

        int addTargetHP = UnityEngine.Random.Range(1, 6);
        m_text_HPAddAmount.text = addTargetHP.ToString();

        m_targetHPValue = cardData.Status.CurHP + addTargetHP;
        if(m_targetHPValue > cardData.Status.MaxHP)
        {
            m_targetHPValue = cardData.Status.MaxHP;
        }
        m_text_HPAddAmount.gameObject.SetActive(true);

        Invoke(nameof(PlayAnimation), m_animationDelay);
        Invoke(nameof(DisplayAddedHP), m_HPTextChangeDelay);
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