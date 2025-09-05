using UnityEngine;
using System.Collections.Generic;
using static CardData;
using UnityEditor.Experimental.GraphView;
using System.Collections;

public class BattleField : MonoBehaviour
{
   
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
    // 0. 세이브 데이터 로드하여 card, Item(인벤토리), 상태이상
    // 1. 카드 세팅
    // 2. UI 세팅
    public void Start()
    {
        SaveDataBuffer.Instance.TryLoadData();
        
        m_friendlyCardList  = new List<BattleCard>();
        m_enemyCardList = new List<BattleCard>();

        List<CardData> cardDatas = SaveDataBuffer.Instance.Data.CardDatas;

        Vector2Int nowPlayerMapPos = SaveDataBuffer.Instance.Data.CurPlayerMapPos;

        // TODO: 나중에 적용해야 할 코드 
        //MapNode.EventNodeType nowEventType = SaveDataBuffer.Instance.Data.MapData.Nodes[nowPlayerMapPos.y]
        //    .Find(node => node.XPos == nowPlayerMapPos.x).EventNodeType;

        // PlaceCards(nowEventType, playerTestCount, enemyTestCount, cardDatas);

        // 임시 코드
        PlaceCards(MapNode.EventNodeType.Combat_Common, playerTestCount, enemyTestCount, cardDatas);
   
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
            GameObject newCardObject = Instantiate(m_cardPrefab, cardPosition, playerFieldAnchor.rotation);
            BattleCard newCard = newCardObject.AddComponent<BattleCard>();
            newCard.Init(_playerCardDatas[i]);
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
                Debug.LogError($"정의되지 않은 전투 타입입니다: {_eventType}");
                break;
        }

        // 생성된 데이터 기반으로 몬스터 카드 배치
        if (enemyDatas.Count > 0)
        {
            totalWidth = (enemyDatas.Count - 1) * enemyCardSpacing;
            startPosition = enemyFieldAnchor.position - new Vector3(totalWidth / 2f, 0, 0);

            for (int i = 0; i < enemyDatas.Count; i++)
            {
                Vector3 cardPosition = startPosition + new Vector3(i * enemyCardSpacing, 0, 0);
                GameObject newCardObject = Instantiate(m_cardPrefab, cardPosition, enemyFieldAnchor.rotation);
                BattleCard newCard = newCardObject.AddComponent<BattleCard>();
                newCard.Init(enemyDatas[i]);
                newCard.transform.SetParent(enemyFieldAnchor);
                m_enemyCardList.Add(newCard);
            }
        }
    }

    /// <summary>
    /// UI 버튼 클릭 시 전투를 시작하는 공개 메서드
    /// </summary>
    public void OnCombatStartButtonClicked()
    {
        Debug.Log("전투 시작 버튼 클릭됨!");
        StartCoroutine(StartCombat());
    }

    /// <summary>
    /// 자동 전투를 관리하는 메인 코루틴
    /// </summary>
    private IEnumerator StartCombat()
    {
        // 전투가 끝날 때까지 (한쪽 진영이 모두 쓰러질 때까지) 반복
        while (m_friendlyCardList.Count > 0 && m_enemyCardList.Count > 0)
        {            
            List<BattleCard> nowBattleCard = new List<BattleCard>();
            nowBattleCard.AddRange(m_friendlyCardList);
            nowBattleCard.AddRange(m_enemyCardList);

            // Speed 스탯에 따라 내림차순으로 정렬
            nowBattleCard.Sort((a, b) => b.GetSpeed().CompareTo(a.GetSpeed()));

            // 정렬된 순서에 따라 각 카드 공격 실행
            foreach (BattleCard attacker in nowBattleCard)
            {
                // 공격자가 전투 중에 쓰러졌으면 건너뛰기
                if (attacker == null || !attacker.gameObject.activeSelf)
                {
                    continue;
                }

                // 공격 시퀀스 코루틴을 실행하고 끝날 때까지 대기
                yield return StartCoroutine(AttackSequence(attacker));

                // 한쪽이 전멸했는지 확인 후 전투 조기 종료
                if (m_friendlyCardList.Count == 0 || m_enemyCardList.Count == 0)
                {
                    break;
                }
            }
            
            // 한 턴이 끝난 후 잠시 대기
            yield return new WaitForSeconds(0.5f);
        }
        
        Debug.Log("전투 종료!");        
        // TODO:
        // 전투 결과 창 표시, 보상 지급 등
    }

    /// <summary>
    /// 단일 카드의 공격 애니메이션과 로직을 처리하는 코루틴
    /// </summary>
    /// <param name="attacker">공격하는 카드</param>
    private IEnumerator AttackSequence(BattleCard attacker)
    {
        // 공격 대상 리스트 결정
        bool isFriendly = m_friendlyCardList.Contains(attacker);
        List<BattleCard> targetList = isFriendly ? m_enemyCardList : m_friendlyCardList;

        // 공격할 대상이 없으면 즉시 종료
        if (targetList.Count == 0)
        {
            yield break;
        }

        // 랜덤 타겟 선정
        BattleCard target = targetList[Random.Range(0, targetList.Count)];
        Vector3 originalPosition = attacker.transform.position;
        Vector3 targetPosition = target.transform.position;

        // 공격 애니메이션
        float moveSpeed = 5f;
        float retreatDistance = 0.5f;

        // 뒤로 살짝 물러나기
        Vector3 retreatPosition = originalPosition - (targetPosition - originalPosition).normalized * retreatDistance;
        while (Vector3.Distance(attacker.transform.position, retreatPosition) > 0.01f)
        {
            attacker.transform.position = Vector3.Lerp(attacker.transform.position, retreatPosition, Time.deltaTime * moveSpeed);
            yield return null;
        }

        // 타겟을 향해 돌진
        while (Vector3.Distance(attacker.transform.position, targetPosition) > 0.5f) // 충돌 거리 조절
        {
            attacker.transform.position = Vector3.Lerp(attacker.transform.position, targetPosition, Time.deltaTime * moveSpeed);
            yield return null;
        }

        // 데미지 처리
        Debug.Log($"{attacker.name}가 {target.name}에게 공격함");
        target.TakeDamage(attacker.GetAttackPower()); 
        
        // TODO: 데미지 텍스트 표시, 피격 효과음, 타격시, 마스킹별 셰이더효과

        // 원래 위치로 복귀
        while (Vector3.Distance(attacker.transform.position, originalPosition) > 0.01f)
        {
            attacker.transform.position = Vector3.Lerp(attacker.transform.position, originalPosition, Time.deltaTime * moveSpeed);
            yield return null;
        }
        attacker.transform.position = originalPosition; // 정확한 위치 보정

        // 죽은 카드 처리
        // if (target.IsDead())
        // {
        //     targetList.Remove(target);
        //     target.gameObject.SetActive(false);
        // }
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