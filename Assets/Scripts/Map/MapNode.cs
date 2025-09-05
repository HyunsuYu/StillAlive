using UnityEngine;


public sealed class MapNode : MonoBehaviour
{
    public enum EventNodeType
    {
        Combat_Common,
        Conversation,
        Explolor,
        Rest,

        Combat_MiddleBoss,
        Combat_ChapterBoss,
    }


    [SerializeField] private EventNodeType m_eventNodeType;

    [SerializeField] private Sprite[] m_Icons;

    private SpriteRenderer m_spriteRenderer_Icon;


    public void Awake()
    {
        m_spriteRenderer_Icon = GetComponent<SpriteRenderer>();
    }

    internal EventNodeType CurEventNodeType
    {
        get
        {
            return m_eventNodeType;
        }
    }

    internal void Init(in EventNodeType targetEventNodeType)
    {
        m_eventNodeType = targetEventNodeType;

        //m_spriteRenderer_Icon.sprite = m_Icons[(int)targetEventNodeType];
        switch(targetEventNodeType)
        {
            case EventNodeType.Combat_Common:
                m_spriteRenderer_Icon.color = Color.white;
                break;

            case EventNodeType.Rest:
                m_spriteRenderer_Icon.color = Color.blue;
                break;

            case EventNodeType.Conversation:
                m_spriteRenderer_Icon.color = Color.green;
                break;

            case EventNodeType.Combat_MiddleBoss:
                m_spriteRenderer_Icon.color = Color.black;
                break;

            case EventNodeType.Combat_ChapterBoss:
                m_spriteRenderer_Icon.color = Color.red;
                break;

            case EventNodeType.Explolor:
                m_spriteRenderer_Icon.color = Color.yellow;
                break;
        }
    }
}