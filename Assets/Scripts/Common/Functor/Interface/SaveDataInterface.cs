using System.Collections.Generic;

using UnityEngine;
using static NPCLookPart;


internal static class SaveDataInterface
{
    internal static List<CardData> GetAliveCardInfos()
    {
        List<CardData> cardDatas = new List<CardData>();

        SaveData curSaveData = SaveDataBuffer.Instance.Data;
        foreach(CardData cardData in curSaveData.CardDatas)
        {
            if(cardData.Status.CurHP > 0)
            {
                cardDatas.Add(cardData);
            }
        }

        return cardDatas;
    }
    internal static List<int> GetAliveCardIndexes()
    {
        List<int> cardDatas = new List<int>();

        SaveData curSaveData = SaveDataBuffer.Instance.Data;
        for(int index = 0; index < curSaveData.CardDatas.Count; index++)
        {
            if (curSaveData.CardDatas[index].Status.CurHP > 0)
            {
                cardDatas.Add(index);
            }
        }

        return cardDatas;
    }

    internal static CardData GetCardData(int index)
    {
        return SaveDataBuffer.Instance.Data.CardDatas[index];
    }
}