using UnityEngine;
using TMPro;


public sealed class ConversationIntelItem : MonoBehaviour
{
    [SerializeField] private Transform m_transform_PortraitParent;
    [SerializeField] private TMP_Text m_text_SpeakText;


    internal void Init(in SaveData.IntelnfoData.SingleConversationInfo singleConversationInfo) 
    {
        CharacterPortraitHelper.CreatePortrait(SaveDataInterface.GetCardData(singleConversationInfo.SpeakerCardIndex), m_transform_PortraitParent);
        m_text_SpeakText.text = singleConversationInfo.SpeakText;
    }
}