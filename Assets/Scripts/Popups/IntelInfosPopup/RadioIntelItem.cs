using UnityEngine;
using TMPro;


public sealed class RadioIntelItem : MonoBehaviour
{
    [SerializeField] private TMP_Text m_text_DPlusDay;
    [SerializeField] private TMP_Text m_text_RadioSpeak;


    internal void Init(in int targetDPlusDay)
    {
        m_text_DPlusDay.text = $"D+{targetDPlusDay} :";
        m_text_RadioSpeak.text = SaveDataBuffer.Instance.Data.IntelInfos.RadioInfos[targetDPlusDay];
    }
}