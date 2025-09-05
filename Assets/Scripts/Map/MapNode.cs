using System.Collections.Generic;
using System.Linq;
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


    private EventNodeType m_eventNodeType;
    private Vector2Int m_position;

    [SerializeField] private GameObject m_gameObject_CurPosIcon;
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
    internal Vector2Int Position
    {
        get
        {
            return m_position;
        }
    }

    internal void Init(in EventNodeType targetEventNodeType, in Vector2Int targetPosition)
    {
        m_eventNodeType = targetEventNodeType;
        m_position = targetPosition;

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

        #region Render
        List<Vector2Int> linkedNodePoses = MapRenderControl.GetCurNextLinkedNodePoses();
        if (!linkedNodePoses.Contains(m_position))
        {
            m_spriteRenderer_Icon.color = Color.Lerp(m_spriteRenderer_Icon.color, Color.black, 0.5f);
        }

        if(SaveDataBuffer.Instance.Data.CurPlayerMapPos == m_position)
        {
            m_gameObject_CurPosIcon.SetActive(true);
        }
        #endregion
    }
}