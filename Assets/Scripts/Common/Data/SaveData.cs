using System.Collections.Generic;

using UnityEngine;


public struct SaveData
{
    public struct IntelnfoData
    {
        public struct SingleConversationInfo
        {
            public int SpeakerCardIndex;
            public string SpeakText;
        }


        public List<string> RadioInfos;
        public List<SingleConversationInfo> ConversationInfos;
    }


    public List<CardData> CardDatas;

    /// <summary>
    /// Key는 Item Type, Value는 Item 개수
    /// </summary>
    public Dictionary<int, int> ItemAmountTable;

    public MapData? MapData;
    public Vector2Int CurPlayerMapPos;

    public int DPlusDay;

    public int Money;

    public IntelnfoData IntelInfos;

    public List<CardData> LastCombatEnemys;

    public List<Vector2Int> PassedWays;
}