using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CommonUtilLib.ThreadSafe;


public class MapGenTest : SingleTonForGameObject<MapGenTest>
{
    //internal struct Node
    //{
    //    public int XPos;
    //    public List<Vector2Int> LinkedNodePoses;
    //    public MapNode.EventNodeType EventNodeType;
    //}


    [SerializeField] private GameObject m_prefab_Node;
    [SerializeField] private GameObject m_prefab_Path;

    [Header("Map Gen Setting")]
    [SerializeField] private Vector2Int m_mapSize;
    private float m_nodeFillPercent;

    [Header("Node Setting")]
    [SerializeField] private int m_startNodeXPos;
    [SerializeField] private List<int> m_endNodeXPoses;

    [Header("Path Setting")]
    [SerializeField] private Vector2Int m_pathSearchRange;
    [SerializeField] private float m_pathLinkChanceWeight = 1.0f;

    [Header("Visual Setting")]
    [SerializeField] private Vector2 m_nodeSpawnGap;

    [SerializeField] private float m_nodeSpawnPosWiggleWeight;

    private bool[,] m_nodePlane;
  
    private Dictionary<int, List<MapData.Node>> m_nodes = new Dictionary<int, List<MapData.Node>>();

    private Dictionary<Vector2Int, MapNode> m_mapNodeTable = new Dictionary<Vector2Int, MapNode>();


    internal bool[,] NodePlane
    {
        get
        {
            return m_nodePlane;
        }
    }
    internal Dictionary<int, List<MapData.Node>> Nodes
    {
        get
        {
            return m_nodes;
        }
    }
    internal Dictionary<Vector2Int, MapNode> MapNodeTable
    {
        get
        {
            return m_mapNodeTable;
        }
    }

    internal void GenerateMap()
    {
        Init();

        #region Gen Random Node Plane
        int filledNodeCount = (int)((m_mapSize.x * m_mapSize.y) * m_nodeFillPercent);

        //    ۳       
        m_nodePlane[0, m_startNodeXPos] = true;

        //                
        foreach (int endNodeXPos in m_endNodeXPoses)
        {
            m_nodePlane[m_mapSize.y - 1, endNodeXPos] = true;
        }

        List<Vector2Int> nodePoses = new List<Vector2Int>();
        for (int count = 0; count < filledNodeCount; count++)
        {
            while (true)
            {
                Vector2 targetPos = new Vector2()
                {
                    x = UnityEngine.Random.Range(0, m_mapSize.x),
                    y = UnityEngine.Random.Range(0, m_mapSize.y)
                };

                // Check Shape
                // f(x) = (x - m_startNodeXPos) & f(x) = -(x - m_startNodeXPos)

                //        ġ
                if (targetPos.y < (targetPos.x - m_startNodeXPos) || targetPos.y < -(targetPos.x - m_startNodeXPos))
                {
                    continue;
                }

                // f(x) = (x - m_endNodeXPoses[n]) + m_mapSize.y & f(x) = -(x - m_endNodeXPoses[n])
                bool bisValidEndShape = false;
                foreach (int endNodeXPos in m_endNodeXPoses)
                {
                    if (targetPos.y < ((targetPos.x - endNodeXPos) + m_mapSize.y) && targetPos.y < (-(targetPos.x - endNodeXPos) + m_mapSize.y))
                    {
                        bisValidEndShape = true;
                        break;
                    }
                }
                if (!bisValidEndShape)
                {
                    continue;
                }

                if (m_nodePlane[(int)targetPos.y, (int)targetPos.x] == false)
                {
                    m_nodePlane[(int)targetPos.y, (int)targetPos.x] = true;
                    nodePoses.Add(new Vector2Int((int)targetPos.x, (int)targetPos.y));
                    break;
                }
            }
        }
        #endregion

        #region Spawn Node
        for (int coord_y = 0; coord_y < m_mapSize.y; coord_y++)
        {
            for (int coord_x = 0; coord_x < m_mapSize.x; coord_x++)
            {
                if (m_nodePlane[coord_y, coord_x])
                {
                    // For Visual
                    GameObject nodeObject = Instantiate(m_prefab_Node, GetNodePos(new Vector2Int(coord_x, coord_y)), Quaternion.identity);
                    nodeObject.transform.SetParent(this.transform);

                    m_mapNodeTable.Add(new Vector2Int(coord_x, coord_y), nodeObject.GetComponent<MapNode>());
                }
            }
        }
        #endregion

        #region Calculate Path
        for (int coord_y = 0; coord_y < m_mapSize.y; coord_y++)
        {
            for (int coord_x = 0; coord_x < m_mapSize.x; coord_x++)
            {
                if (m_nodePlane[coord_y, coord_x])
                {
                    List<Vector2Int> pathLinkCandidates = new List<Vector2Int>();
                    for (int searchCoord_x = (coord_x - m_pathSearchRange.x >= 0 ? coord_x - m_pathSearchRange.x : 0);
                        searchCoord_x < (coord_x + m_pathSearchRange.x < m_mapSize.x ? coord_x + m_pathSearchRange.x : m_mapSize.x - 1);
                        searchCoord_x++)
                    {
                        for (int searchCoord_y = coord_y + 1; searchCoord_y < (coord_y + 1 + m_pathSearchRange.y < m_mapSize.y ? coord_y + 1 + m_pathSearchRange.y : m_mapSize.y); searchCoord_y++)
                        {
                            if (m_nodePlane[searchCoord_y, searchCoord_x])
                            {
                                pathLinkCandidates.Add(new Vector2Int(searchCoord_x, searchCoord_y));
                                break;
                            }
                        }
                    }

                    if (pathLinkCandidates.Count == 0 && coord_y != m_mapSize.y - 1)
                    {
                        float minDistance = float.MaxValue;
                        Vector2Int targetNodePos = -Vector2Int.one;
                        for (int searchCoord_y = coord_y + 1; searchCoord_y < m_mapSize.y; searchCoord_y++)
                        {
                            for (int searchCoord_x = 0; searchCoord_x < m_mapSize.x; searchCoord_x++)
                            {
                                if (m_nodePlane[searchCoord_y, searchCoord_x])
                                {
                                    float distance = Vector2.Distance(new Vector2(coord_x, coord_y), new Vector2(searchCoord_x, searchCoord_y));
                                    if (distance < minDistance)
                                    {
                                        minDistance = distance;
                                        targetNodePos = new Vector2Int(searchCoord_x, searchCoord_y);
                                    }
                                }
                            }
                        }

                        pathLinkCandidates.Add(targetNodePos);
                    }

                    MapData.Node node = new MapData.Node()
                    {
                        XPos = coord_x,
                        LinkedNodePoses = new List<Vector2Int>(),
                        EventNodeType = (MapNode.EventNodeType)UnityEngine.Random.Range(0, Enum.GetNames(typeof(MapNode.EventNodeType)).Length)
                    };
                    pathLinkCandidates = RandomizePathCandidatePoses(pathLinkCandidates);
                    for (int pathLinkCandidateIndex = 0; pathLinkCandidateIndex < pathLinkCandidates.Count; pathLinkCandidateIndex++)
                    {
                        if (BIsPathCanLink(pathLinkCandidateIndex + 1))
                        {
                            node.LinkedNodePoses.Add(pathLinkCandidates[pathLinkCandidateIndex]);
                            nodePoses.Remove(pathLinkCandidates[pathLinkCandidateIndex]);
                        }
                    }

                    m_nodes[coord_y].Add(node);
                    m_mapNodeTable[new Vector2Int(coord_x, coord_y)].Init(node.EventNodeType);
                }
            }
        }

        //      Node              Node        Node
        foreach (Vector2Int nodePos in nodePoses)
        {
            float minDistance = float.MaxValue;
            Vector2Int targetNodePos = -Vector2Int.one;
            for (int coord_y = nodePos.y - 1; coord_y >= 0; coord_y--)
            {
                for (int coord_x = 0; coord_x < m_mapSize.x; coord_x++)
                {
                    if (m_nodePlane[coord_y, coord_x])
                    {
                        float distance = Vector2.Distance(nodePos, new Vector2(coord_x, coord_y));
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            targetNodePos = new Vector2Int(coord_x, coord_y);
                        }
                    }
                }
            }

            if (targetNodePos == -Vector2Int.one)
            {
                Debug.Log("Can't Find Target Node");
            }

            m_nodes[nodePos.y].Find(node => node.XPos == nodePos.x).LinkedNodePoses.Add(targetNodePos);
        }
        #endregion

        #region Spawn Path
        foreach (int coord_y in m_nodes.Keys)
        {
            foreach (MapData.Node node in m_nodes[coord_y])
            {
                //Debug.Log($"Node X Pos: {node.XPos}, Linked Node Count: {node.LinkedNodePoses.Count}");
                foreach (Vector2Int linkedNodePos in node.LinkedNodePoses)
                {
                    GameObject pathObject = Instantiate(m_prefab_Path, GetNodePos(new Vector2Int(node.XPos, coord_y)), Quaternion.identity);
                    pathObject.transform.SetParent(this.transform);

                    LineRenderer lineRenderer = pathObject.GetComponent<LineRenderer>();
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPosition(0, GetNodePos(new Vector2Int(node.XPos, coord_y)));
                    lineRenderer.SetPosition(1, GetNodePos(linkedNodePos));
                }
            }
        }
        #endregion
    }

    private void Init()
    {
        if (m_mapSize.x % 2 == 0)
        {
            throw new System.Exception("Map Size X is Even Number");
        }
        else if (m_startNodeXPos < 0 || m_startNodeXPos >= m_mapSize.x)
        {
            throw new System.Exception("Start Node X Pos is Out of Range");
        }

        m_nodePlane = new bool[m_mapSize.y, m_mapSize.x];
        for (int index = 0; index < m_mapSize.y; index++)
        {
            m_nodes.Add(index, new List<MapData.Node>());
        }
    }

    //       ġ               Խ Ŵ
    private Vector2 GetNodePos(in Vector2Int nodePos)
    {
        float wiggleXFactor = Mathf.PerlinNoise(nodePos.x / 10.0f, nodePos.y / 10.0f) * m_nodeSpawnPosWiggleWeight;
        float wiggleYFactor = Mathf.PerlinNoise(nodePos.y / 10.0f, nodePos.x / 10.0f) * m_nodeSpawnPosWiggleWeight;

        Debug.Log($"Node Pos: {nodePos}, Wiggle X Factor: {wiggleXFactor}, Wiggle Y Factor: {wiggleYFactor}");

        return new Vector2()
        {
            x = nodePos.x * m_nodeSpawnGap.x + wiggleXFactor,
            y = nodePos.y * m_nodeSpawnGap.y + wiggleYFactor
        };
    }
    private bool BIsPathCanLink(in int index)
    {
        return (UnityEngine.Random.Range(0.0f, 1.0f) <= Mathf.Pow((1.0f / index), m_pathLinkChanceWeight)) ? true : false;
    }

    private List<Vector2Int> RandomizePathCandidatePoses(in List<Vector2Int> pathLinkCandidates)
    {
        List<Vector2Int> randomizedPathLinkCandidates = new List<Vector2Int>();
        while (pathLinkCandidates.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, pathLinkCandidates.Count);
            randomizedPathLinkCandidates.Add(pathLinkCandidates[randomIndex]);
            pathLinkCandidates.RemoveAt(randomIndex);
        }
        return randomizedPathLinkCandidates;
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new NotImplementedException();
    }
}