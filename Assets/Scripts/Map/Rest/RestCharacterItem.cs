using TMPro;
using UnityEngine;
using static NPCLookPart;


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

    private int m_cardIndex;


    internal void Init(in int cardIndex)
    {
        m_cardIndex = cardIndex;

        CardData cardData = SaveDataBuffer.Instance.Data.CardDatas[m_cardIndex];
        var portraitRectTransform = CharacterPortraitHelper.CreatePortrait(cardData, m_transform_CharacterPortraitParent).GetComponent<RectTransform>();
        portraitRectTransform.localScale = new Vector3(2.0f, 2.0f, 1.0f);

        m_text_PrevHP.text = $"{cardData.Status.CurHP} / {cardData.Status.MaxHP}";

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
        m_animator_TextAdd.SetTrigger("HPAdd");
    }
    private void DisplayAddedHP()
    {
        SaveData curSaveData = SaveDataBuffer.Instance.Data;
        var curCardData = curSaveData.CardDatas[m_cardIndex];
        curCardData.Status.CurHP = m_targetHPValue;
        curSaveData.CardDatas[m_cardIndex] = curCardData;
        SaveDataBuffer.Instance.TrySetData(curSaveData);

        m_text_PrevHP.text = $"{SaveDataBuffer.Instance.Data.CardDatas[m_cardIndex].Status.CurHP} / {SaveDataBuffer.Instance.Data.CardDatas[m_cardIndex].Status.MaxHP}";
        m_text_HPAddAmount.gameObject.SetActive(false);
    }
}