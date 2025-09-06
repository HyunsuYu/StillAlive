using UnityEngine;
using System.Collections.Generic;
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

    private bool m_isStartBattle;

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

        // PlaceCards(nowEventType, cardDatas.Count, Random.Range(1, 5), cardDatas);
        m_isStartBattle = false;

        // 임시 코드
        PlaceCards(MapNode.EventNodeType.Combat_Common, playerTestCount, enemyTestCount, cardDatas);
   
    }

    /// <summary>
    /// 월드 공간에 카드 GameObject를 배치하고 정렬하는 재사용 가능한 함수
    /// </summary>
    private void PlaceCards(MapNode.EventNodeType _eventType, int _friendlyCardCount, int _enemyCardCount, List<CardData> _playerCardDatas)
    {             
        if (_friendlyCardCount <= 0 || _enemyCardCount <= 0) return;

        // 플레이어 카드 세팅 --> 임의로 데이터 세팅 중 
        float totalWidth = (_friendlyCardCount - 1) * playerCardSpacing;
        Vector3 startPosition = playerFieldAnchor.position - new Vector3(totalWidth / 2f, 0, 0);
        
        for (int i = 0; i < _friendlyCardCount; i++)
        {
            CardData cd = new CardData();
            cd.NPCLookTable = new Dictionary<CardData.NPCLookPartType, int>();
            cd.ColorPalleteIndex = 0;
            _playerCardDatas.Add(cd);

            _playerCardDatas[i].NPCLookTable[CardData.NPCLookPartType.Face] = 0;
            _playerCardDatas[i].NPCLookTable[CardData.NPCLookPartType.Eye] = 0;
            _playerCardDatas[i].NPCLookTable[CardData.NPCLookPartType.Glasses] = 0;


            Vector3 cardPosition = startPosition + new Vector3(i * playerCardSpacing, 0, 0);
            GameObject newCardObject = Instantiate(m_cardPrefab, cardPosition, playerFieldAnchor.rotation);
            BattleCard newCard = newCardObject.GetComponent<BattleCard>();
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
                    monsterData.BIsPlayer = false;
                    monsterData.BIsTraitor = true;

                    monsterData.NPCLookTable = new Dictionary<CardData.NPCLookPartType, int>();
                    monsterData.ColorPalleteIndex = 0;
                    enemyDatas.Add(monsterData);

                    enemyDatas[i].NPCLookTable[CardData.NPCLookPartType.Face] = 1;
                    enemyDatas[i].NPCLookTable[CardData.NPCLookPartType.Eye] = 1;
                    enemyDatas[i].NPCLookTable[CardData.NPCLookPartType.Glasses] = 0;


                    for (int j = 0; j < nowDay; j++)
                    {
                        monsterData.Status.CurHP += Random.Range(1, MathUtility.CalculateValueByDay(nowDay));
                        monsterData.Status.DefencePower += Random.Range(1, MathUtility.CalculateValueByDay(nowDay));
                        monsterData.Status.AttackPower += Random.Range(1, MathUtility.CalculateValueByDay(nowDay));
                        monsterData.Status.Speed += Random.Range(1, MathUtility.CalculateValueByDay(nowDay));
                    }
                    CardData.StatusInfo defaultStatus = CardData.DefaultStatus();
                    monsterData.Status.CurHP = defaultStatus.CurHP;
                    monsterData.Status.DefencePower = defaultStatus.DefencePower;
                    monsterData.Status.AttackPower = defaultStatus.AttackPower;
                    monsterData.Status.Speed = defaultStatus.Speed;
                    // enemyDatas.Add(monsterData);
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
                BattleCard newCard = newCardObject.GetComponent<BattleCard>();
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
        if (!m_isStartBattle)
        {
            m_isStartBattle = true;
            StartCoroutine(StartCombat());
        }
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
        // 공격 대상 선정
        bool isFriendly = m_friendlyCardList.Contains(attacker);
        List<BattleCard> targetList = isFriendly ? m_enemyCardList : m_friendlyCardList;

        if (targetList.Count == 0) yield break;

        BattleCard target = targetList[Random.Range(0, targetList.Count)];
        Vector3 originalPosition = attacker.transform.position;
        Vector3 originalScale = attacker.transform.localScale;
        Vector3 targetPosition = target.transform.position;

        // 애니메이션 연출을 위한 설정값 
        float scaleAmount = 1.2f;         // 얼마나 커질지
        float retreatDistance = 0.5f;     // 후퇴 거리 
        float retreatSpeed = 5f;
        float scaleUpDuration = 0.15f;    // 커지는 데 걸리는 시간
        float attackDashDuration = 0.2f;  // 적에게 돌진하는 시간
        float impactPauseDuration = 0.1f; // 적과 부딪혔을 때 잠시 멈추는 시간
        float returnDuration = 0.4f;     // 원래 자리로 돌아오는 시간

        // 공격 준비 (살짝 커지기)
        float elapsedTime = 0f;
        Vector3 targetScale = originalScale * scaleAmount;
        while (elapsedTime < scaleUpDuration)
        {
            // 오브젝트가 파괴되었는지 확인
            if (attacker == null || !attacker.gameObject.activeSelf)
            {
                yield break; // 코루틴 종료
            }
            
            attacker.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / scaleUpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        attacker.transform.localScale = targetScale;

        // 후퇴하며 공격 준비
        Vector3 retreatPosition = originalPosition - (targetPosition - originalPosition).normalized * retreatDistance;
        while (Vector3.Distance(attacker.transform.position, retreatPosition) > 0.01f)
        {
            attacker.transform.position = Vector3.Lerp(attacker.transform.position, retreatPosition, Time.deltaTime * retreatSpeed);
            yield return null;
        }

        // 적에게 빠르게 돌진
        elapsedTime = 0f;
        Vector3 startDashPosition = attacker.transform.position; // 현재 위치에서 시작
        while (elapsedTime < attackDashDuration)
        {            
            float t = elapsedTime / attackDashDuration;
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f); 

            attacker.transform.position = Vector3.Lerp(startDashPosition, targetPosition, easedT);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        attacker.transform.position = targetPosition;

        // 타격 및 잠시 멈춤 
        target.TakeDamage(attacker.GetAttackPower());

        yield return new WaitForSeconds(impactPauseDuration); 

        // 원래 위치로 복귀 및 크기 원래대로
        elapsedTime = 0f;
        Vector3 afterImpactPosition = attacker.transform.position;
        while (elapsedTime < returnDuration)
        {
            float t = elapsedTime / returnDuration;
            attacker.transform.position = Vector3.Lerp(afterImpactPosition, originalPosition, t);
            attacker.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 위치 및 크기 보정
        attacker.transform.position = originalPosition;
        attacker.transform.localScale = originalScale;

        // 죽은 카드 처리 (주석 해제 필요)
        if (target.IsDead())
        {
            targetList.Remove(target);
            Destroy(target.gameObject); 
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