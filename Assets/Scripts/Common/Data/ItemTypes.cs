using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemTypes", menuName = "StillAlive/ItemTypes")]
public class ItemTypes : ScriptableObject
{
    public enum BufEffectType
    {
        PlusHP,
        PlusAttack,
        PlusSpeed,
        Boom,
        RemoveDisease_All,
        RemoveDisease_Specific
    }

    [Serializable] public struct ItemData
    {
        [Serializable] public struct BufInfo
        {
            public BufEffectType BufEffect;
            public int BufDegree;
        }


        public string Name;
        public Sprite ItemSprite;

        public int MaxDurability;

        public BufInfo[] BufInfos;
    }


    public ItemData[] ItemDatas;


    internal static int ItemCount
    {
        get
        {
            return 8;
        }
    }
}