using CommonUtilLib.ThreadSafe;
using System.Collections.Generic;
using UnityEngine;


public sealed class MapRenderControl : SingleTonForGameObject<MapRenderControl>
{
    [SerializeField] private GameObject m_prefab_Node;
    [SerializeField] private GameObject m_prefab_Path;

    [Header("Visual Setting")]
    [SerializeField] private Vector2 m_nodeSpawnGap;

    [SerializeField] private float m_nodeSpawnPosWiggleWeight;

    private Dictionary<Vector2Int, MapNode> m_mapNodeTable = new Dictionary<Vector2Int, MapNode>();

    [SerializeField] private Material m_material_PassedPath;
    [SerializeField] private Material m_material_NotPassedPath;

    [Header("Scrool Speed")]
    [SerializeField] private float m_scroolSpeed = 1.0f;


    public void Awake()
    {
        SetInstance(this);
    }
    public void Update()
    {
        if(Mathf.Abs(Input.mouseScrollDelta.y) > 0.5f)
        {
            Camera.main.transform.position = new Vector3()
            {
                x = Camera.main.transform.position.x,
                y = Camera.main.transform.position.y + m_scroolSpeed * Input.mouseScrollDelta.y,
                z = Camera.main.transform.position.z
            };

            if(Camera.main.transform.position.y < 0.0f)
            {
                Camera.main.transform.position = new Vector3()
                {
                    x = Camera.main.transform.position.x,
                    y = 0.0f,
                    z = Camera.main.transform.position.z
                };
            }
            else if(Camera.main.transform.position.y >= SaveDataBuffer.Instance.Data.MapData.Value.MapSize.y * m_nodeSpawnGap.y)
            {
                Camera.main.transform.position = new Vector3()
                {
                    x = Camera.main.transform.position.x,
                    y = SaveDataBuffer.Instance.Data.MapData.Value.MapSize.y * m_nodeSpawnGap.y,
                    z = Camera.main.transform.position.z
                };
            }
        }

        Ray ray = Camera.main.ScreenPointToRay(Camera.main.transform.position);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        foreach(RaycastHit hit in hits )
        {
            var mapNode = hit.transform.GetComponent<MapNode>();
            if (mapNode != null)
            {
                switch(mapNode.CurEventNodeType)
                {
                    case MapNode.EventNodeType.Combat_Common:
                    case MapNode.EventNodeType.Combat_MiddleBoss:
                    case MapNode.EventNodeType.Combat_ChapterBoss:

                        break;

                    case MapNode.EventNodeType.Rest:

                        break;

                    case MapNode.EventNodeType.Conversation:

                        break;

                    case MapNode.EventNodeType.Explolor:

                        break;
                }

                break;
            }
        }
    }

    internal void Render()
    {
        if(!SaveDataBuffer.Instance.Data.MapData.HasValue)
        {
            MapGenerator.Instance.GenerateMap();
        }

        MapData mapData = SaveDataBuffer.Instance.Data.MapData.Value;

        #region Spawn Node
        for (int coord_y = 0; coord_y < mapData.MapSize.y; coord_y++)
        {
            for (int coord_x = 0; coord_x < mapData.MapSize.x; coord_x++)
            {
                if (mapData.NodePlane[coord_y, coord_x])
                {
                    // For Visual
                    GameObject nodeObject = Instantiate(m_prefab_Node, GetNodePos(new Vector2Int(coord_x, coord_y)), Quaternion.identity);
                    nodeObject.transform.SetParent(this.transform);

                    var mapNode = nodeObject.GetComponent<MapNode>();
                    foreach(var temp in mapData.Nodes[coord_y])
                    {
                        if(temp.XPos == coord_x)
                        {
                            mapNode.Init(temp.EventNodeType);
                            break;
                        }
                    }
                    m_mapNodeTable.Add(new Vector2Int(coord_x, coord_y), mapNode);
                }
            }
        }
        #endregion

        #region Spawn Path
        var passedWays = SaveDataBuffer.Instance.Data.PassedWays;
        foreach (int coord_y in mapData.Nodes.Keys)
        {
            foreach (MapData.Node node in mapData.Nodes[coord_y])
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

                    if(passedWays.Contains(new Vector2Int(node.XPos, coord_y)) && passedWays.Contains(linkedNodePos))
                    {
                        lineRenderer.material = m_material_PassedPath;
                    }
                    else
                    {
                        lineRenderer.material = m_material_NotPassedPath;
                    }
                }
            }
        }
        #endregion

        Camera.main.transform.position = new Vector3()
        {
            x = m_nodeSpawnGap.x * mapData.MapSize.x / 2.0f,
            y = 0.0f,
            z = -10.0f
        };
    }

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }

    private Vector2 GetNodePos(in Vector2Int nodePos)
    {
        float wiggleXFactor = Mathf.PerlinNoise(nodePos.x / 10.0f, nodePos.y / 10.0f) * m_nodeSpawnPosWiggleWeight;
        float wiggleYFactor = Mathf.PerlinNoise(nodePos.y / 10.0f, nodePos.x / 10.0f) * m_nodeSpawnPosWiggleWeight;

        return new Vector2()
        {
            x = nodePos.x * m_nodeSpawnGap.x + wiggleXFactor,
            y = nodePos.y * m_nodeSpawnGap.y + wiggleYFactor
        };
    }
}