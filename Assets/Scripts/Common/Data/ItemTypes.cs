using System;

using UnityEngine;


[CreateAssetMenu(fileName = "ItemTypes", menuName = "StillAlive/ItemTypes")]
public class ItemTypes : ScriptableObject
{
    public enum BufEffectType
    {

    }

    [Serializable] public struct ItemData
    {
        public string Name;
        public Sprite ItemSprite;

        public BufEffectType BufEffect;
        public int BufDegree;
    }


    public ItemData[] ItemDatas;
}