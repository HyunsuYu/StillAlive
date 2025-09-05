using System.Collections.Generic;

using UnityEngine;


public struct SaveData
{
    public struct IntelnfoData
    {
        public List<string> RadioInfos;
        public Dictionary<int, string> ConversationInfos;
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

    public List<string> RadioInfos;
    public List<string> ConversationInfos;

    public List<CardData> LastCombatEnemys;
}