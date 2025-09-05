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
        FrontHair,
        BackHair
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
        public int MaxHP;
        public int CurHP;

        public int AttackPower;
        public int DefencePower;
        public int Speed;
    }

    public StatusInfo Status;

    public static StatusInfo DefaultStatus()
    {
        StatusInfo status = new StatusInfo();

        status.MaxHP = Random.Range(1,4);
        status.CurHP = status.MaxHP;
        status.AttackPower = Random.Range(1,4);
        status.DefencePower = Random.Range(1,4);
        status.Speed = Random.Range(1,4);

        return status;
    }

    // 플레이어 여부
    public bool BIsPlayer;
    
    public Dictionary<NPCLookPartType, int> NPCLookTable;
    public int ColorPalleteIndex;

    public bool BIsTraitor;

    public DiseasesType Diseases;

    public List<int> ItemIndexes;
}