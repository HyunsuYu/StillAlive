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
    public enum CardPersonality
    {
        Optimistic,
        Pessimistic,
        Realistic,
        Rebellious,
        Dependent
    }
    public enum LastNightState
    {
        /// <summary>
        /// Peace는 공격이 일어났는지, 아닌지도 모르는 상태입니다. 즉, 전날 밤에 공격이 애초에 이루어지지 않았다면 살아있는 동료 전원이 Peace 상태입니다. 만약 전날 밤에 공격이 일어났더라도 목격조차 못한 인원은 Peace 상태일 수 있습니다.
        /// </summary>
        Peace,
        Attacker,
        AttackedPerson,
        Witness,
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
    public CardPersonality Personality;
    public LastNightState LastNight;

    public List<int> ItemIndexes;
}