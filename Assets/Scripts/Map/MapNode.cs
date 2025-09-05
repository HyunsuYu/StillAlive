using UnityEngine;


public sealed class MapNode : MonoBehaviour
{
    public enum EventNodeType
    {
        Combat_Common,
        Combat_MiddleBoss,
        Combat_ChapterBoss,
        Conversation,
        Explolor,
        Rest
    }


    private EventNodeType m_eventNodeType;

    private Sprite m_sprite_Icon;


    internal void Init(in EventNodeType targetEventNodeType)
    {
        m_eventNodeType = targetEventNodeType;
    }
}