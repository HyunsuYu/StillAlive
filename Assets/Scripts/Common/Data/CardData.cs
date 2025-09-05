using System.Collections.Generic;

using UnityEngine;


public struct CardData
{
    public enum NPCLookPartType
    {
        Top,
        Face,
        Eye,
        Mouth,
        Glasses,
        Cap,
        hair
    }
    public enum DiseasesType : byte
    {
        None        = 0b0000_0000,
        ColdFever   = 0b0000_0001,
        Zombie      = 0b0000_0010,
        Tetanus     = 0b0000_0100
    }

    public struct StatusInfo
    {
        public int HP;
        public int AttackPower;
        public int DefencePower;
        public int Speed;
    }


    public StatusInfo Status;

    public bool BIsPlayer;
    public Dictionary<NPCLookPartType, int> NPCLookTable;

    public DiseasesType Diseases;

    public List<int> ItemIndexes;
}