using System.Collections.Generic;

using UnityEngine;


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
}