using CommonUtilLib.ThreadSafe;
using System.Collections.Generic;
using UnityEngine;


public sealed class ChiefMapManager : SingleTonForGameObject<ChiefMapManager>
{
    [SerializeField] private GameObject m_layout_LateNight;


    public void Awake()
    {
        SetInstance(this);
    }
    public void Start()
    {
        SaveData curSaveData = SaveDataBuffer.Instance.Data;

        #region For Dummy Data
        if (curSaveData.CardDatas.Count == 0)
        {
            curSaveData.CardDatas.Add(GetRandomCardDataDummy(true, false, CardData.LastNightState.Peace));
            curSaveData.CardDatas.Add(GetRandomCardDataDummy(false, true, CardData.LastNightState.Peace));
            curSaveData.CardDatas.Add(GetRandomCardDataDummy(false, false, CardData.LastNightState.Peace));
            curSaveData.CardDatas.Add(GetRandomCardDataDummy(false, false, CardData.LastNightState.Peace));
            SaveDataBuffer.Instance.TrySetData(curSaveData);
            SaveDataBuffer.Instance.TrySaveData();
        }
        #endregion

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
                m_layout_LateNight.SetActive(true);
            }

            curSaveData.LightActionAppliedDay = curSaveData.DPlusDay;
        }
        SaveDataBuffer.Instance.TrySetData(curSaveData);
        SaveDataBuffer.Instance.TrySaveData();
        #endregion

        #region Radio

        #endregion
    }
    public void Update()
    {
        if(m_layout_LateNight.activeSelf && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            m_layout_LateNight.SetActive(false);
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
}