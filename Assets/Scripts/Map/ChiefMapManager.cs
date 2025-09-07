using System;
using System.Collections.Generic;

using UnityEngine;
using TMPro;

using CommonUtilLib.ThreadSafe;


public sealed class ChiefMapManager : SingleTonForGameObject<ChiefMapManager>
{
    public enum EnterancePopupPhase
    {
        None,
        LateNight,
        Radio
    }

    [Serializable] public struct RadioTemplate
    {
        public string[] SpeakTexts;
    }


    [Header("Late Night")]
    [SerializeField] private GameObject m_layout_LateNight;

    [Header("Radio")]
    [SerializeField] private GameObject m_layout_Radio;
    [SerializeField] private TMP_Text m_text_RadioSpeak;

    [SerializeField] private RadioTemplate[] m_radioTemplates_NoTraiter;
    [SerializeField] private RadioTemplate[] m_radioTemplates_WithTraiter;

    private CardData? m_radioSelectedTraiterCardData = null;
    private CardData.NPCLookPartType m_radioSelectedLoopPart = CardData.NPCLookPartType.Top;
    private int m_selectedTemplateIndex = -1;
    private int m_curRadioSpeakIndex = 0;

    private EnterancePopupPhase m_curPhase = EnterancePopupPhase.None;


    public void Awake()
    {
        SetInstance(this);
    }
    public void Start()
    {
        SaveData curSaveData = SaveDataBuffer.Instance.Data;

        //#region For Dummy Data
        //if (curSaveData.CardDatas.Count == 0)
        //{
        //    curSaveData.CardDatas.Add(GetRandomCardDataDummy(true, false, CardData.LastNightState.Peace));
        //    curSaveData.CardDatas.Add(GetRandomCardDataDummy(false, true, CardData.LastNightState.Peace));
        //    curSaveData.CardDatas.Add(GetRandomCardDataDummy(false, false, CardData.LastNightState.Peace));
        //    curSaveData.CardDatas.Add(GetRandomCardDataDummy(false, false, CardData.LastNightState.Peace));
        //    SaveDataBuffer.Instance.TrySetData(curSaveData);
        //    SaveDataBuffer.Instance.TrySaveData();
        //}
        //#endregion

        RenderAll();

        #region Late Night
        curSaveData = SaveDataBuffer.Instance.Data;
        if (curSaveData.DPlusDay != curSaveData.LightActionAppliedDay)
        {
            bool bisSomeoneHit = false;

            var aliveCardIndexes = SaveDataInterface.GetAliveCardIndexes();
            foreach(int cardIndex in aliveCardIndexes)
            {
                // Attack Someone
                if(SaveDataInterface.GetCardData(cardIndex).BIsTraitor && UnityEngine.Random.Range(0, 2) == 0)
                {
                    int randomTargetCardIndex = -1;
                    while(true)
                    {
                        int tempTargetCardIndex = UnityEngine.Random.Range(0, aliveCardIndexes.Count);
                        if(tempTargetCardIndex != cardIndex)
                        {
                            randomTargetCardIndex = tempTargetCardIndex;
                            break;
                        }
                    }

                    var attackTargetCardData = curSaveData.CardDatas[randomTargetCardIndex];
                    attackTargetCardData.Status.CurHP -= (int)(attackTargetCardData.Status.CurHP * 0.2f);
                    curSaveData.CardDatas[randomTargetCardIndex] = attackTargetCardData;

                    bisSomeoneHit = true;
                }
            }

            if(bisSomeoneHit)
            {
                m_curPhase = EnterancePopupPhase.LateNight;
                m_layout_LateNight.SetActive(true);
            }

            curSaveData.LightActionAppliedDay = curSaveData.DPlusDay;

            SaveDataBuffer.Instance.TrySetData(curSaveData);
            SaveDataBuffer.Instance.TrySaveData();
        }
        #endregion

        #region Radio
        if (SaveDataBuffer.Instance.Data.ItemAmountTable[0] > 0)
        {
            if(m_curPhase == EnterancePopupPhase.None)
            {
                m_curPhase = EnterancePopupPhase.Radio;
                m_layout_Radio.SetActive(true);
            }
            
            var aliveCardDatas = SaveDataInterface.GetAliveCardInfos();

            foreach (CardData cardData in aliveCardDatas)
            {
                if(cardData.BIsTraitor)
                {
                    m_radioSelectedTraiterCardData = cardData;
                    break;
                }
            }

            if(m_radioSelectedTraiterCardData.HasValue)
            {
                m_radioSelectedLoopPart = (CardData.NPCLookPartType)UnityEngine.Random.Range(0, Enum.GetNames(typeof(CardData.NPCLookPartType)).Length);
                m_selectedTemplateIndex = UnityEngine.Random.Range(0, m_radioTemplates_WithTraiter.Length);

                m_text_RadioSpeak.text = m_radioTemplates_WithTraiter[m_selectedTemplateIndex].SpeakTexts[0];
            }
            else
            {
                m_selectedTemplateIndex = UnityEngine.Random.Range(0, m_radioTemplates_NoTraiter.Length);

                m_text_RadioSpeak.text = m_radioTemplates_NoTraiter[m_selectedTemplateIndex].SpeakTexts[0];
            }
            m_curRadioSpeakIndex = 1;
        }
        #endregion
    }
    public void Update()
    {
        if(m_curPhase == EnterancePopupPhase.None)
        {
            return;
        }

        if(m_curPhase == EnterancePopupPhase.LateNight && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            Debug.Log("A");
            m_layout_LateNight.SetActive(false);

            if (SaveDataBuffer.Instance.Data.ItemAmountTable[0] > 0)
            {
                m_curPhase = EnterancePopupPhase.Radio;
                Invoke(nameof(ActiveRadioLayout), 1.0f);
            }
            else
            {
                m_curPhase = EnterancePopupPhase.None;
            }
        }
        else if(m_curPhase == EnterancePopupPhase.Radio && m_layout_Radio.activeSelf && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            Debug.Log("B");
            if (m_radioSelectedTraiterCardData.HasValue)
            {
                m_curRadioSpeakIndex++;
                if(m_curRadioSpeakIndex >= m_radioTemplates_WithTraiter[m_selectedTemplateIndex].SpeakTexts.Length - 1)
                {
                    m_curPhase = EnterancePopupPhase.None;
                    m_layout_Radio.SetActive(false);
                    return;
                }

                m_text_RadioSpeak.text = m_radioTemplates_WithTraiter[m_selectedTemplateIndex].SpeakTexts[m_curRadioSpeakIndex];
            }
            else
            {
                m_curRadioSpeakIndex++;
                if (m_curRadioSpeakIndex >= m_radioTemplates_NoTraiter[m_selectedTemplateIndex].SpeakTexts.Length - 1)
                {
                    m_curPhase = EnterancePopupPhase.None;
                    m_layout_Radio.SetActive(false);
                    return;
                }

                m_text_RadioSpeak.text = m_radioTemplates_NoTraiter[m_selectedTemplateIndex].SpeakTexts[m_curRadioSpeakIndex];
            }
        }
    }

    internal void RenderAll()
    {
        MapRenderControl.Instance.Render();
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }

    private static CardData GetRandomCardDataDummy(in bool bisPlayer = false, in bool bisTraiter = false, in CardData.LastNightState lastNightState = CardData.LastNightState.Peace)
    {
        CardData dummy = new CardData()
        {
            BIsPlayer = bisPlayer,
            NPCLookTable = new Dictionary<CardData.NPCLookPartType, int>(),
            ColorPalleteIndex = 0,
            BIsTraitor = bisTraiter,
            Diseases = (CardData.DiseasesType)UnityEngine.Random.Range(0, System.Enum.GetNames(typeof(CardData.DiseasesType)).Length),
            Personality = (CardData.CardPersonality)UnityEngine.Random.Range(0, System.Enum.GetNames(typeof(CardData.CardPersonality)).Length),
            LastNight = lastNightState,
            Items = new List<CardData.AttachedItemData>(),
            Status = new CardData.StatusInfo()
            {
                MaxHP = 30,
                CurHP = 10
            }
        };
        dummy.NPCLookTable.Add(CardData.NPCLookPartType.Top, UnityEngine.Random.Range(0, 4));
        dummy.NPCLookTable.Add(CardData.NPCLookPartType.Face, UnityEngine.Random.Range(0, 4));
        dummy.NPCLookTable.Add(CardData.NPCLookPartType.Eye, UnityEngine.Random.Range(0, 4));
        dummy.NPCLookTable.Add(CardData.NPCLookPartType.Mouth, UnityEngine.Random.Range(0, 4));
        dummy.NPCLookTable.Add(CardData.NPCLookPartType.Glasses, 0);
        dummy.NPCLookTable.Add(CardData.NPCLookPartType.Cap, 0);
        dummy.NPCLookTable.Add(CardData.NPCLookPartType.FrontHair, UnityEngine.Random.Range(0, 4));
        dummy.NPCLookTable.Add(CardData.NPCLookPartType.BackHair, UnityEngine.Random.Range(0, 4));

        return dummy;
    }

    private void ActiveRadioLayout()
    {
        m_layout_Radio.SetActive(true);
    }
}