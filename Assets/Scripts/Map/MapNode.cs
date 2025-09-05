using System.Collections.Generic;
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
    [SerializeField] private Vector2Int m_position;

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
        Vector2Int curPlayerMapPos = SaveDataBuffer.Instance.Data.CurPlayerMapPos;
        List<Vector2Int> linkedNodePoses = null;
        foreach (var nodeData in SaveDataBuffer.Instance.Data.MapData.Value.Nodes[curPlayerMapPos.y])
        {
            if (nodeData.XPos == curPlayerMapPos.x)
            {
                linkedNodePoses = nodeData.LinkedNodePoses;
                break;
            }
        }
        foreach(int yPos in SaveDataBuffer.Instance.Data.MapData.Value.Nodes.Keys)
        {
            foreach(var nodeData in SaveDataBuffer.Instance.Data.MapData.Value.Nodes[yPos])
            {
                if(nodeData.LinkedNodePoses.Contains(SaveDataBuffer.Instance.Data.CurPlayerMapPos))
                {
                    linkedNodePoses.Add(new Vector2Int(nodeData.XPos, yPos));
                }
            }
        }

        if(!linkedNodePoses.Contains(m_position))
        {
            m_spriteRenderer_Icon.color = Color.Lerp(m_spriteRenderer_Icon.color, Color.black, 0.5f);
        }
        #endregion
    }
}