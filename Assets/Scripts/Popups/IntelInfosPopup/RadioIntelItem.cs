using UnityEngine;
using TMPro;


public sealed class RadioIntelItem : MonoBehaviour
{
    [SerializeField] private TMP_Text m_text_DPlusDay;
    [SerializeField] private TMP_Text m_text_RadioSpeak;


    internal void Init(in SaveData.IntelnfoData.SingleRadioInfo targetSingleRadioIntelInfo)
    {
        m_text_DPlusDay.text = $"D+{targetSingleRadioIntelInfo.GeneratedDay} :";
        m_text_RadioSpeak.text = targetSingleRadioIntelInfo.SpeakText;
    }
}