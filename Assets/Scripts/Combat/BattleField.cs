using UnityEngine;
using System.Collections.Generic;
using static CardData;
using UnityEditor.Experimental.GraphView;

public class BattleField : MonoBehaviour
{
    [System.Flags]
    public enum CombatType : byte
    {
        None = 0,
        Common = 1 << 0,
        MiddleBoss = 1 << 1,
        ChapterBoss = 1 << 2,
    }

    [Header("ī�� ������")]
    [SerializeField] private GameObject m_cardPrefab;

    [Header("ī�� �ʵ� ����Ʈ")]
    private List<BattleCard> m_friendlyCardList;
    private List<BattleCard> m_enemyCardList;

    [Header("�÷��̾� ���� ����")]
    [SerializeField] private Transform playerFieldAnchor;     // �÷��̾� ���� ī�� �߽� ��Ŀ�� 
    [SerializeField] private float playerCardSpacing = 1.5f;  // ī�� ���� ����

    [Header("�� ���� ����")]
    [SerializeField] private Transform enemyFieldAnchor;      // ���ʹ� ���� ī�� �߽� ��Ŀ��
    [SerializeField] private float enemyCardSpacing = 1.5f;   // ī�� ���� ����

    [Header("�׽�Ʈ ī��Ʈ ����")]
    [SerializeField] private int playerTestCount;
    [SerializeField] private int enemyTestCount;
    
    // BattleField ����
    // 0. ���̺� ������ �ε��Ͽ� card, Item(�κ��丮), 
    // 1. ī�� ����
    // 2. UI ����
    public void Start()
    {
        SaveDataBuffer.Instance.TryLoadData();
        
        m_friendlyCardList  = new List<BattleCard>();
        m_enemyCardList = new List<BattleCard>();

        List<CardData> cardDatas = SaveDataBuffer.Instance.Data.CardDatas;

        Vector2Int nowPlayerMapPos = SaveDataBuffer.Instance.Data.CurPlayerMapPos;

        MapNode.EventNodeType nowEventType = SaveDataBuffer.Instance.Data.MapData.Nodes[nowPlayerMapPos.y]
            .Find(node => node.XPos == nowPlayerMapPos.x).EventNodeType;

        // PlaceCards(cardDatas.Count, Random.Range(1,5), cardDatas);
        PlaceCards(nowEventType, playerTestCount, enemyTestCount, cardDatas);
    }

    /// <summary>
    /// ���� ������ ī�� GameObject�� ��ġ�ϰ� �����ϴ� ���� ������ �Լ�
    /// </summary>
    private void PlaceCards(MapNode.EventNodeType _eventType, int _friendlyCardCount, int _enemyCardCount, List<CardData> _playerCardDatas)
    {             
        if (_friendlyCardCount <= 0 || _enemyCardCount <= 0) return;

        // �÷��̾� ī�� ����
        float totalWidth = (_friendlyCardCount - 1) * playerCardSpacing;
        Vector3 startPosition = playerFieldAnchor.position - new Vector3(totalWidth / 2f, 0, 0);       

        for (int i = 0; i < _friendlyCardCount; i++)
        {
            Vector3 cardPosition = startPosition + new Vector3(i * playerCardSpacing, 0, 0);
            BattleCard newCard = Instantiate(m_cardPrefab, cardPosition, playerFieldAnchor.rotation).AddComponent<BattleCard>();
            // newCard.Init(_playerCardDatas[i]);
            newCard.transform.SetParent(playerFieldAnchor);
            m_friendlyCardList.Add(newCard);
        }

        
        // ���� ī�� ����
        List<CardData> enemyDatas = new List<CardData>();
        int nowDay = SaveDataBuffer.Instance.Data.DPlusDay;

        switch (_eventType)
        {
            case MapNode.EventNodeType.Combat_Common:
                // �Ϲ� ���� ���� ����
                for (int i = 0; i < _enemyCardCount; i++)
                {
                    CardData monsterData = new CardData();
                    for (int j = 0; j < nowDay; j++)
                    {
                        monsterData.Status.CurHP += Random.Range(1, MathUtility.CalculateValueByDay(nowDay));
                        monsterData.Status.DefencePower += Random.Range(1, MathUtility.CalculateValueByDay(nowDay));
                        monsterData.Status.AttackPower += Random.Range(1, MathUtility.CalculateValueByDay(nowDay));
                        monsterData.Status.Speed += Random.Range(1, MathUtility.CalculateValueByDay(nowDay));
                    }
                    StatusInfo defaultStatus = CardData.DefaultStatus();
                    monsterData.Status.CurHP = defaultStatus.CurHP;
                    monsterData.Status.DefencePower = defaultStatus.DefencePower;
                    monsterData.Status.AttackPower = defaultStatus.AttackPower;
                    monsterData.Status.Speed = defaultStatus.Speed;
                    enemyDatas.Add(monsterData);
                }
                break;
            
            case MapNode.EventNodeType.Combat_MiddleBoss:
                // TODO: �߰� ���� ���� ���� ����
                Debug.Log("�߰� ���� ���� ����");
                break;

            case MapNode.EventNodeType.Combat_ChapterBoss:
                // TODO: é�� ���� ���� ���� ����
                Debug.Log("é�� ���� ���� ����");
                break;
            
            default:
                Debug.LogWarning($"���ǵ��� ���� ���� Ÿ���Դϴ�: {_eventType}");
                break;
        }

        // ������ ������ ������� ���� ī�� ��ġ
        if (enemyDatas.Count > 0)
        {
            float totalWidth = (enemyDatas.Count - 1) * enemyCardSpacing;
            Vector3 startPosition = enemyFieldAnchor.position - new Vector3(totalWidth / 2f, 0, 0);

            for (int i = 0; i < enemyDatas.Count; i++)
            {
                Vector3 cardPosition = startPosition + new Vector3(i * enemyCardSpacing, 0, 0);
                BattleCard newCard = Instantiate(m_cardPrefab, cardPosition, enemyFieldAnchor.rotation).AddComponent<BattleCard>();
                // newCard.Init(enemyDatas[i]);
                newCard.transform.SetParent(enemyFieldAnchor);
                m_enemyCardList.Add(newCard);
            }
        }
    }

    // BattleField Ż���� �� ȣ����
    void ExitBattleField()
    {
        foreach (BattleCard card in m_friendlyCardList)
        {
            Destroy(card);
        }
        foreach (BattleCard card in m_enemyCardList)
        {
            Destroy(card);
        }
        m_friendlyCardList.Clear();
        m_enemyCardList.Clear();
    }    
}