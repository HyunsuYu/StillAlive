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

    [Header("카드 프리팹")]
    [SerializeField] private GameObject m_cardPrefab;

    [Header("카드 필드 리스트")]
    private List<BattleCard> m_friendlyCardList;
    private List<BattleCard> m_enemyCardList;

    [Header("플레이어 진영 설정")]
    [SerializeField] private Transform playerFieldAnchor;     // 플레이어 진영 카드 중심 앵커값 
    [SerializeField] private float playerCardSpacing = 1.5f;  // 카드 범위 넓이

    [Header("적 진영 설정")]
    [SerializeField] private Transform enemyFieldAnchor;      // 에너미 진영 카드 중심 앵커값
    [SerializeField] private float enemyCardSpacing = 1.5f;   // 카드 범위 넓이

    [Header("테스트 카운트 설정")]
    [SerializeField] private int playerTestCount;
    [SerializeField] private int enemyTestCount;
    
    // BattleField 세팅
    // 0. 세이브 데이터 로드하여 card, Item(인벤토리), 
    // 1. 카드 세팅
    // 2. UI 세팅
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
    /// 월드 공간에 카드 GameObject를 배치하고 정렬하는 재사용 가능한 함수
    /// </summary>
    private void PlaceCards(MapNode.EventNodeType _eventType, int _friendlyCardCount, int _enemyCardCount, List<CardData> _playerCardDatas)
    {             
        if (_friendlyCardCount <= 0 || _enemyCardCount <= 0) return;

        // 플레이어 카드 세팅
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

        
        // 몬스터 카드 세팅
        List<CardData> enemyDatas = new List<CardData>();
        int nowDay = SaveDataBuffer.Instance.Data.DPlusDay;

        switch (_eventType)
        {
            case MapNode.EventNodeType.Combat_Common:
                // 일반 몬스터 생성 로직
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
                // TODO: 중간 보스 생성 로직 구현
                Debug.Log("중간 보스 전투 세팅");
                break;

            case MapNode.EventNodeType.Combat_ChapterBoss:
                // TODO: 챕터 보스 생성 로직 구현
                Debug.Log("챕터 보스 전투 세팅");
                break;
            
            default:
                Debug.LogWarning($"정의되지 않은 전투 타입입니다: {_eventType}");
                break;
        }

        // 생성된 데이터 기반으로 몬스터 카드 배치
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

    // BattleField 탈출할 때 호출함
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