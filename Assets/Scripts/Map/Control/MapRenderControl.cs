using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

using CommonUtilLib.ThreadSafe;
using Unity.VisualScripting;


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

    [Header("UIs")]
    [SerializeField] private TMP_Text m_text_DPlusDay;

    [SerializeField] private SimpleCollegueItem[] m_simpleCollegueItems;


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

        if(Input.GetMouseButtonDown(0))
        {
            List<Vector2Int> linkedNodePoses = GetCurNextLinkedNodePoses();

            string log = string.Empty;
            foreach(var nodePoses in linkedNodePoses)
            {
                log += nodePoses.ToString() + "\n";
            }
            Debug.Log(log);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            foreach (RaycastHit hit in hits)
            {
                var mapNode = hit.transform.GetComponent<MapNode>();
                if (mapNode != null)
                {
                    if(!linkedNodePoses.Contains(mapNode.Position))
                    {
                        return;
                    }

                    SaveData curSaveData = SaveDataBuffer.Instance.Data;
                    curSaveData.CurPlayerMapPos = mapNode.Position;
                    curSaveData.DPlusDay++;
                    curSaveData.PassedWays.Add(mapNode.Position);
                    SaveDataBuffer.Instance.TrySetData(curSaveData);
                    SaveDataBuffer.Instance.TrySaveData();

                    switch (mapNode.CurEventNodeType)
                    {
                        case MapNode.EventNodeType.Combat_Common:
                        case MapNode.EventNodeType.Combat_MiddleBoss:
                            SceneManager.LoadScene("Combat");
                            break;

                        case MapNode.EventNodeType.Combat_ChapterBoss:
                            MapGenerator.Instance.GenerateMap();
                            curSaveData = SaveDataBuffer.Instance.Data;
                            curSaveData.CurPlayerMapPos = new Vector2Int(7, 0);
                            SaveDataBuffer.Instance.TrySetData(curSaveData);
                            SaveDataBuffer.Instance.TrySaveData();

                            SceneManager.LoadScene("Combat");
                            break;

                        case MapNode.EventNodeType.Rest:
                            RestPopupControl.Instance.OepnPopup();
                            break;

                        case MapNode.EventNodeType.Conversation:
                            SceneManager.LoadScene("Conversation");
                            break;

                        case MapNode.EventNodeType.Explolor:
                            SceneManager.LoadScene("Explolor");
                            break;
                    }

                    break;
                }
            }
        }
    }

    internal static List<Vector2Int> GetCurNextLinkedNodePoses()
    {
        Vector2Int curPlayerMapPos = SaveDataBuffer.Instance.Data.CurPlayerMapPos;
        List<Vector2Int> linkedNodePoses = null;
        foreach (var nodeData in SaveDataBuffer.Instance.Data.MapData.Value.Nodes[curPlayerMapPos.y])
        {
            if (nodeData.XPos == curPlayerMapPos.x)
            {
                linkedNodePoses = ((Vector2Int[])nodeData.LinkedNodePoses.ToArray().Clone()).ToList();
                break;
            }
        }

        foreach (int yPos in SaveDataBuffer.Instance.Data.MapData.Value.Nodes.Keys)
        {
            foreach (var nodeData in SaveDataBuffer.Instance.Data.MapData.Value.Nodes[yPos])
            {
                Vector2Int curPos = new Vector2Int(nodeData.XPos, yPos);
                if (nodeData.LinkedNodePoses.Contains(SaveDataBuffer.Instance.Data.CurPlayerMapPos)
                    && !linkedNodePoses.Contains(curPos))
                {
                    linkedNodePoses.Add(curPos);
                }
            }
        }

        List<int> removalLinkIndexes = new List<int>();
        for (int index = 0; index < linkedNodePoses.Count; index++)
        {
            if (linkedNodePoses[index].y <= curPlayerMapPos.y)
            {
                removalLinkIndexes.Add(index);
            }
        }
        for (int index = removalLinkIndexes.Count - 1; index >= 0; index--)
        {
            linkedNodePoses.RemoveAt(removalLinkIndexes[index]);
        }

        return linkedNodePoses;
    }

    internal void Render()
    {
        if(!SaveDataBuffer.Instance.Data.MapData.HasValue)
        {
            MapGenerator.Instance.GenerateMap();
        }

        Debug.Log(SaveDataBuffer.Instance.Data.CurPlayerMapPos);

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
                            mapNode.Init(temp.EventNodeType, new Vector2Int(coord_x, coord_y));
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

        m_text_DPlusDay.text = "D+" + SaveDataBuffer.Instance.Data.DPlusDay.ToString();

        var aliveCards = SaveDataInterface.GetAliveCardInfos();
        for(int index = 0; index < m_simpleCollegueItems.Length; index++)
        {
            m_simpleCollegueItems[index].gameObject.SetActive(false);
            if (index < aliveCards.Count)
            {
                m_simpleCollegueItems[index].Render(aliveCards[index]);
                m_simpleCollegueItems[index].gameObject.SetActive(true);
            }
        }
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