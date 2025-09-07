using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class BattleField : MonoBehaviour
{

    [Header("카드 프리팹")]
    [SerializeField] private GameObject m_cardPrefab;

    [Header("카드 필드 리스트")]
    private List<BattleCard> m_teamCardList;
    private List<BattleCard> m_enemyCardList;

    [Header("플레이어 진영 설정")]
    [SerializeField] private Transform m_teamFieldAnchor;     // 플레이어 진영 카드 중심 앵커값 
    [SerializeField] private float m_teamCardSpacing = 1.5f;  // 카드 범위 넓이

    [Header("적 진영 설정")]
    [SerializeField] private Transform m_enemyFieldAnchor;      // 에너미 진영 카드 중심 앵커값
    [SerializeField] private float m_enemyCardSpacing = 1.5f;   // 카드 범위 넓이

    [Header("테스트 카운트 설정")]
    [SerializeField] private int m_playerTestCount;
    [SerializeField] private int m_enemyTestCount;

    private static bool m_isBattleEnd;
    public static bool IsBattleEnd => m_isBattleEnd;

    [Header("전투 연출 설정")]
    [Tooltip("공격 시 카드가 얼마나 커질지")]
    [SerializeField, Range(1f, 2f)]
    private float scaleAmount = 1.2f;

    [Tooltip("공격 전 뒤로 물러나는 거리")]
    [SerializeField, Range(0f, 2f)]
    private float retreatDistance = 0.5f;

    [Tooltip("후퇴 속도")]
    [SerializeField, Range(1f, 20f)]
    private float retreatSpeed = 5f;

    [Tooltip("커지는 데 걸리는 시간 (초)")]
    [SerializeField, Range(0.05f, 1f)]
    private float scaleUpDuration = 0.15f;

    [Tooltip("적에게 돌진하는 시간 (초)")]
    [SerializeField, Range(0.05f, 1f)]
    private float attackDashDuration = 0.2f;

    [Tooltip("타격 시 멈추는 시간 (초)")]
    [SerializeField, Range(0f, 1f)]
    private float impactPauseDuration = 0.1f;

    [Tooltip("원래 자리로 돌아오는 시간 (초)")]
    [SerializeField, Range(0.05f, 1f)]
    private float returnDuration = 0.4f;

    [Header("BattleField관련 Canvas클래스 UI")]
    [SerializeField] private BattleFieldMenu m_battleFieldMenu;
    [SerializeField] private BattleResult m_battleResult;

    private bool m_isAppearTraitor;

    // BattleField 세팅
    // 0. 세이브 데이터 로드하여 card, Item(인벤토리), 상태이상
    // 1. 카드 세팅
    // 2. UI 세팅
    public void Start()
    {
        SaveDataBuffer.Instance.TryLoadData();

        m_teamCardList = new List<BattleCard>();
        m_enemyCardList = new List<BattleCard>();

        List<CardData> cardDatas = SaveDataBuffer.Instance.Data.CardDatas;

        Vector2Int nowPlayerMapPos = SaveDataBuffer.Instance.Data.CurPlayerMapPos;

        // TODO: 나중에 적용해야 할 코드 
        //MapNode.EventNodeType nowEventType = SaveDataBuffer.Instance.Data.MapData.Nodes[nowPlayerMapPos.y]
        //    .Find(node => node.XPos == nowPlayerMapPos.x).EventNodeType;

        // PlaceCards(nowEventType, cardDatas.Count, Random.Range(1, 5), cardDatas);
        m_isBattleEnd = false;
        m_isAppearTraitor = false;

        // 임시 코드
        PlaceCards(MapNode.EventNodeType.Combat_Common, m_playerTestCount, m_enemyTestCount, cardDatas);

        // 팀 상태창 초기화
        m_battleFieldMenu.InitTeamStatus(m_teamCardList);
    }

    /// <summary>
    /// 월드 공간에 카드 GameObject를 배치하고 정렬하는 재사용 가능한 함수
    /// </summary>
    private void PlaceCards(MapNode.EventNodeType _eventType, int _friendlyCardCount, int _enemyCardCount, List<CardData> _playerCardDatas)
    {
        if (_friendlyCardCount <= 0 || _enemyCardCount <= 0) return;

        // 플레이어 카드 세팅 --> 임의로 데이터 세팅 중 
        float totalWidth = (_friendlyCardCount - 1) * m_teamCardSpacing;
        Vector3 startPosition = m_teamFieldAnchor.position - new Vector3(totalWidth / 2f, 0, 0);

        for (int i = 0; i < _friendlyCardCount; i++)
        {
            CardData cd = new CardData();
            cd.NPCLookTable = new Dictionary<CardData.NPCLookPartType, int>();
            cd.ColorPalleteIndex = 0;
            cd.Status = CardData.DefaultStatus();
            cd.Status.MaxHP += 1;
            cd.Status.CurHP += 1;

            // 배신자 로직 테스트
            if(i==1)
            {
                cd.BIsTraitor = true;
            }

            _playerCardDatas.Add(cd);

            _playerCardDatas[i].NPCLookTable[CardData.NPCLookPartType.Face] = 0;
            _playerCardDatas[i].NPCLookTable[CardData.NPCLookPartType.Eye] = 0;
            _playerCardDatas[i].NPCLookTable[CardData.NPCLookPartType.Glasses] = 0;            

            Vector3 cardPosition = startPosition + new Vector3(i * m_teamCardSpacing, 0, 0);
            GameObject newCardObject = Instantiate(m_cardPrefab, cardPosition, m_teamFieldAnchor.rotation);
            BattleCard newCard = newCardObject.GetComponent<BattleCard>();
            newCard.Init(_playerCardDatas[i]);
            newCard.transform.SetParent(m_teamFieldAnchor);
            m_teamCardList.Add(newCard);
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

                    monsterData.NPCLookTable[CardData.NPCLookPartType.Face] = 1;
                    monsterData.NPCLookTable[CardData.NPCLookPartType.Eye] = 1;
                    monsterData.NPCLookTable[CardData.NPCLookPartType.Glasses] = 0;


                    for (int j = 0; j < nowDay; j++)
                    {
                        monsterData.Status.CurHP += UnityEngine.Random.Range(1, MathUtility.CalculateValueByDay(nowDay));
                        monsterData.Status.MaxHP = monsterData.Status.CurHP;
                        monsterData.Status.DefencePower += UnityEngine.Random.Range(1, MathUtility.CalculateValueByDay(nowDay));
                        monsterData.Status.AttackPower += UnityEngine.Random.Range(1, MathUtility.CalculateValueByDay(nowDay));
                        monsterData.Status.Speed += UnityEngine.Random.Range(1, MathUtility.CalculateValueByDay(nowDay));
                    }

                    monsterData.Status.CurHP = monsterData.Status.CurHP / 4;
                    monsterData.Status.MaxHP = monsterData.Status.CurHP;
                    monsterData.Status.DefencePower = monsterData.Status.DefencePower / 4;
                    monsterData.Status.AttackPower = monsterData.Status.AttackPower / 4;
                    monsterData.Status.Speed = monsterData.Status.Speed / 4;

                    CardData.StatusInfo defaultStatus = CardData.DefaultStatus();
                    monsterData.Status.MaxHP = defaultStatus.MaxHP;
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
            totalWidth = (enemyDatas.Count - 1) * m_enemyCardSpacing;
            startPosition = m_enemyFieldAnchor.position - new Vector3(totalWidth / 2f, 0, 0);

            for (int i = 0; i < enemyDatas.Count; i++)
            {
                Vector3 cardPosition = startPosition + new Vector3(i * m_enemyCardSpacing, 0, 0);
                GameObject newCardObject = Instantiate(m_cardPrefab, cardPosition, m_enemyFieldAnchor.rotation);
                BattleCard newCard = newCardObject.GetComponent<BattleCard>();
                newCard.Init(enemyDatas[i]);
                newCard.transform.SetParent(m_enemyFieldAnchor);
                m_enemyCardList.Add(newCard);
            }
        }
    }

    public void UpdateTeamMemberStatusUI(BattleCard teamCard)
    {
        if (m_battleFieldMenu != null && teamCard != null)
        {
            int index = m_teamCardList.IndexOf(teamCard);
            if (index >= 0)
            {
                m_battleFieldMenu.UpdateTeamMemberStatus(teamCard, index);
            }
        }
    }

    public void OnBattleStartButtonClicked()
    {
        if (!m_isBattleEnd)
        {
            StartCoroutine(StartCombat());
        }
    }

    public void OnInventoryButtonClicked()
    {

    }

    public void OnInfoButtonClicked()
    {

    }

    /// <summary>
    /// 자동 전투를 관리하는 메인 코루틴
    /// </summary>
    private IEnumerator StartCombat()
    {
        string resultText = "";
        // 전투가 끝날 때까지 (한쪽 진영이 모두 쓰러질 때까지) 반복
        while (m_teamCardList.Count > 0 && m_enemyCardList.Count > 0)
        {
            List<BattleCard> nowBattleCard = new List<BattleCard>();
            nowBattleCard.AddRange(m_teamCardList);
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
                if (m_enemyCardList.Count == 0)
                {
                    resultText = "승 리!";
                    break;
                }
                else if (m_teamCardList.Count == 0)
                {
                    resultText = "패 배...";
                    break;
                }
            }
            m_isBattleEnd = true;
            // 한 턴이 끝난 후 잠시 대기
            yield return new WaitForSeconds(0.5f);
        }

        m_battleResult.BattleFinished(resultText);

        yield return new WaitForSeconds(2f);

        ExitBattleField();
    }


    /// <summary>
    /// 단일 카드의 공격 애니메이션과 로직을 처리하는 코루틴
    /// </summary>
    /// <param name="attacker">공격하는 카드</param>
    private IEnumerator AttackSequence(BattleCard attacker)
    {
        // 공격 대상 선정
        bool isFriendly = m_teamCardList.Contains(attacker);
      
        List<BattleCard> targetList = isFriendly ? m_enemyCardList : m_teamCardList;

        if (targetList.Count == 0) yield break;

        BattleCard target = targetList[UnityEngine.Random.Range(0, targetList.Count)];

        Vector3 originalPosition = attacker.transform.position;
        Vector3 originalScale = attacker.transform.localScale;
        Vector3 targetPosition = target.transform.position;

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

        // 배신자 행동 가능성 존재
        if (isFriendly && attacker.IsTraitor() && !m_isAppearTraitor && UnityEngine.Random.value < 0.5f)
        {
            m_isAppearTraitor = true;
            attacker.transform.localScale = originalScale;

            int index = m_teamCardList.IndexOf(attacker);
            m_battleFieldMenu.UpdateTeamMemberStatus(attacker, index, true);
            
            // 일반 공격 대신 배신 시퀀스를 실행하고, 이 공격은 여기서 종료
            yield return StartCoroutine(TraitorSequence(attacker));
            yield break;
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

        // 죽은 카드 처리
        if (target.IsDead())
        {
            targetList.Remove(target);

            Destroy(target.gameObject);
        }
    }

    /// <summary>
    /// 지정된 카드 리스트를 해당 진영의 정 위치로 부드럽게 재배치하는 코루틴
    /// </summary>
    private IEnumerator RearRangeField(List<BattleCard> cardList, Transform anchor, float spacing, float duration = 0.5f)
    {
        if (cardList.Count == 0) yield break;

        // 각 카드의 최종 목표 위치 계산
        float totalWidth = (cardList.Count - 1) * spacing;
        Vector3 startPos = anchor.position - new Vector3(totalWidth / 2f, 0, 0);

        Dictionary<BattleCard, Vector3> targetPositions = new Dictionary<BattleCard, Vector3>();
        for (int i = 0; i < cardList.Count; i++)
        {
            Vector3 targetPos = startPos + new Vector3(i * spacing, 0, 0);
            targetPositions[cardList[i]] = targetPos;
        }

        // 현재 위치 저장
        Dictionary<BattleCard, Vector3> originalPositions = new Dictionary<BattleCard, Vector3>();
        foreach (var card in cardList)
        {
            originalPositions[card] = card.transform.position;
        }

        // 타이머를 이용해 부드럽게 이동
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            foreach (var card in cardList)
            {
                if (card != null) // 카드가 파괴되었을 경우 대비
                {
                    card.transform.position = Vector3.Lerp(originalPositions[card], targetPositions[card], t);
                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 최종 위치 보정
        foreach (var card in cardList)
        {
            if (card != null)
            {
                card.transform.position = targetPositions[card];
            }
        }
    }

    /// <summary>
    /// 배신자 카드의 배신 시퀀스를 처리하는 코루틴
    /// </summary>
    private IEnumerator TraitorSequence(BattleCard traitor)
    {
        Debug.Log($"{traitor.name}이(가) 배신했습니다!");

        // 아군 리스트에서 제거하고 적군 리스트에 무작위 위치로 추가
        m_teamCardList.Remove(traitor);
        int insertIndex = UnityEngine.Random.Range(0, m_enemyCardList.Count + 1); // 맨 뒤에 추가될 수도 있도록 +1
        m_enemyCardList.Insert(insertIndex, traitor);
        traitor.transform.SetParent(m_enemyFieldAnchor); // 부모 변경

        // 양쪽 진영을 동시에 부드럽게 재배치
        Coroutine friendlyRearrange = StartCoroutine(RearRangeField(m_teamCardList, m_teamFieldAnchor, m_teamCardSpacing));
        Coroutine enemyRearrange = StartCoroutine(RearRangeField(m_enemyCardList, m_enemyFieldAnchor, m_enemyCardSpacing));

        // 두 재배치가 모두 끝날 때까지 대기
        yield return friendlyRearrange;
        yield return enemyRearrange;

        // 잠시 대기하여 상황을 인지시킴
        yield return new WaitForSeconds(0.5f);

        // 이제 적이 된 배신자가 아군을 공격 (기존 공격 시퀀스 재사용)
        yield return StartCoroutine(AttackSequence(traitor));
    }

    // BattleField 탈출할 때 호출함
    void ExitBattleField()
    {
        foreach (BattleCard card in m_teamCardList)
        {
            Destroy(card);
        }
        foreach (BattleCard card in m_enemyCardList)
        {
            Destroy(card);
        }
        m_teamCardList.Clear();
        m_enemyCardList.Clear();      
    }
}