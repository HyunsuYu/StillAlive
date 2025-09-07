using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class BattleField : MonoBehaviour
{

    [Header("ī�� ������")]
    [SerializeField] private GameObject m_cardPrefab;

    [Header("ī�� �ʵ� ����Ʈ")]
    private List<BattleCard> m_teamCardList;
    private List<BattleCard> m_enemyCardList;

    [Header("�÷��̾� ���� ����")]
    [SerializeField] private Transform m_teamFieldAnchor;     // �÷��̾� ���� ī�� �߽� ��Ŀ�� 
    [SerializeField] private float m_teamCardSpacing = 1.5f;  // ī�� ���� ����

    [Header("�� ���� ����")]
    [SerializeField] private Transform m_enemyFieldAnchor;      // ���ʹ� ���� ī�� �߽� ��Ŀ��
    [SerializeField] private float m_enemyCardSpacing = 1.5f;   // ī�� ���� ����

    [Header("�׽�Ʈ ī��Ʈ ����")]
    [SerializeField] private int m_playerTestCount;
    [SerializeField] private int m_enemyTestCount;

    private static bool m_isBattleEnd;
    public static bool IsBattleEnd => m_isBattleEnd;

    [Header("���� ���� ����")]
    [Tooltip("���� �� ī�尡 �󸶳� Ŀ����")]
    [SerializeField, Range(1f, 2f)]
    private float scaleAmount = 1.2f;

    [Tooltip("���� �� �ڷ� �������� �Ÿ�")]
    [SerializeField, Range(0f, 2f)]
    private float retreatDistance = 0.5f;

    [Tooltip("���� �ӵ�")]
    [SerializeField, Range(1f, 20f)]
    private float retreatSpeed = 5f;

    [Tooltip("Ŀ���� �� �ɸ��� �ð� (��)")]
    [SerializeField, Range(0.05f, 1f)]
    private float scaleUpDuration = 0.15f;

    [Tooltip("������ �����ϴ� �ð� (��)")]
    [SerializeField, Range(0.05f, 1f)]
    private float attackDashDuration = 0.2f;

    [Tooltip("Ÿ�� �� ���ߴ� �ð� (��)")]
    [SerializeField, Range(0f, 1f)]
    private float impactPauseDuration = 0.1f;

    [Tooltip("���� �ڸ��� ���ƿ��� �ð� (��)")]
    [SerializeField, Range(0.05f, 1f)]
    private float returnDuration = 0.4f;

    [Header("BattleField���� CanvasŬ���� UI")]
    [SerializeField] private BattleFieldMenu m_battleFieldMenu;
    [SerializeField] private BattleResult m_battleResult;

    private bool m_isAppearTraitor;

    // BattleField ����
    // 0. ���̺� ������ �ε��Ͽ� card, Item(�κ��丮), �����̻�
    // 1. ī�� ����
    // 2. UI ����
    public void Start()
    {
        SaveDataBuffer.Instance.TryLoadData();

        m_teamCardList = new List<BattleCard>();
        m_enemyCardList = new List<BattleCard>();

        List<CardData> cardDatas = SaveDataBuffer.Instance.Data.CardDatas;

        Vector2Int nowPlayerMapPos = SaveDataBuffer.Instance.Data.CurPlayerMapPos;

        // TODO: ���߿� �����ؾ� �� �ڵ� 
        //MapNode.EventNodeType nowEventType = SaveDataBuffer.Instance.Data.MapData.Nodes[nowPlayerMapPos.y]
        //    .Find(node => node.XPos == nowPlayerMapPos.x).EventNodeType;

        // PlaceCards(nowEventType, cardDatas.Count, Random.Range(1, 5), cardDatas);
        m_isBattleEnd = false;
        m_isAppearTraitor = false;

        // �ӽ� �ڵ�
        PlaceCards(MapNode.EventNodeType.Combat_Common, m_playerTestCount, m_enemyTestCount, cardDatas);

        // �� ����â �ʱ�ȭ
        m_battleFieldMenu.InitTeamStatus(m_teamCardList);
    }

    /// <summary>
    /// ���� ������ ī�� GameObject�� ��ġ�ϰ� �����ϴ� ���� ������ �Լ�
    /// </summary>
    private void PlaceCards(MapNode.EventNodeType _eventType, int _friendlyCardCount, int _enemyCardCount, List<CardData> _playerCardDatas)
    {
        if (_friendlyCardCount <= 0 || _enemyCardCount <= 0) return;

        // �÷��̾� ī�� ���� --> ���Ƿ� ������ ���� �� 
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

            // ����� ���� �׽�Ʈ
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
                // TODO: �߰� ���� ���� ���� ����
                Debug.Log("�߰� ���� ���� ����");
                break;

            case MapNode.EventNodeType.Combat_ChapterBoss:
                // TODO: é�� ���� ���� ���� ����
                Debug.Log("é�� ���� ���� ����");
                break;

            default:
                Debug.LogError($"���ǵ��� ���� ���� Ÿ���Դϴ�: {_eventType}");
                break;
        }

        // ������ ������ ������� ���� ī�� ��ġ
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
    /// �ڵ� ������ �����ϴ� ���� �ڷ�ƾ
    /// </summary>
    private IEnumerator StartCombat()
    {
        string resultText = "";
        // ������ ���� ������ (���� ������ ��� ������ ������) �ݺ�
        while (m_teamCardList.Count > 0 && m_enemyCardList.Count > 0)
        {
            List<BattleCard> nowBattleCard = new List<BattleCard>();
            nowBattleCard.AddRange(m_teamCardList);
            nowBattleCard.AddRange(m_enemyCardList);

            // Speed ���ȿ� ���� ������������ ����
            nowBattleCard.Sort((a, b) => b.GetSpeed().CompareTo(a.GetSpeed()));

            // ���ĵ� ������ ���� �� ī�� ���� ����
            foreach (BattleCard attacker in nowBattleCard)
            {
                // �����ڰ� ���� �߿� ���������� �ǳʶٱ�
                if (attacker == null || !attacker.gameObject.activeSelf)
                {
                    continue;
                }

                // ���� ������ �ڷ�ƾ�� �����ϰ� ���� ������ ���
                yield return StartCoroutine(AttackSequence(attacker));

                // ������ �����ߴ��� Ȯ�� �� ���� ���� ����
                if (m_enemyCardList.Count == 0)
                {
                    resultText = "�� ��!";
                    break;
                }
                else if (m_teamCardList.Count == 0)
                {
                    resultText = "�� ��...";
                    break;
                }
            }
            m_isBattleEnd = true;
            // �� ���� ���� �� ��� ���
            yield return new WaitForSeconds(0.5f);
        }

        m_battleResult.BattleFinished(resultText);

        yield return new WaitForSeconds(2f);

        ExitBattleField();
    }


    /// <summary>
    /// ���� ī���� ���� �ִϸ��̼ǰ� ������ ó���ϴ� �ڷ�ƾ
    /// </summary>
    /// <param name="attacker">�����ϴ� ī��</param>
    private IEnumerator AttackSequence(BattleCard attacker)
    {
        // ���� ��� ����
        bool isFriendly = m_teamCardList.Contains(attacker);
      
        List<BattleCard> targetList = isFriendly ? m_enemyCardList : m_teamCardList;

        if (targetList.Count == 0) yield break;

        BattleCard target = targetList[UnityEngine.Random.Range(0, targetList.Count)];

        Vector3 originalPosition = attacker.transform.position;
        Vector3 originalScale = attacker.transform.localScale;
        Vector3 targetPosition = target.transform.position;

        // ���� �غ� (��¦ Ŀ����)
        float elapsedTime = 0f;
        Vector3 targetScale = originalScale * scaleAmount;
        while (elapsedTime < scaleUpDuration)
        {
            // ������Ʈ�� �ı��Ǿ����� Ȯ��
            if (attacker == null || !attacker.gameObject.activeSelf)
            {
                yield break; // �ڷ�ƾ ����
            }

            attacker.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / scaleUpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        attacker.transform.localScale = targetScale;

        // �����ϸ� ���� �غ�
        Vector3 retreatPosition = originalPosition - (targetPosition - originalPosition).normalized * retreatDistance;
        while (Vector3.Distance(attacker.transform.position, retreatPosition) > 0.01f)
        {
            attacker.transform.position = Vector3.Lerp(attacker.transform.position, retreatPosition, Time.deltaTime * retreatSpeed);
            yield return null;
        }

        // ����� �ൿ ���ɼ� ����
        if (isFriendly && attacker.IsTraitor() && !m_isAppearTraitor && UnityEngine.Random.value < 0.5f)
        {
            m_isAppearTraitor = true;
            attacker.transform.localScale = originalScale;

            int index = m_teamCardList.IndexOf(attacker);
            m_battleFieldMenu.UpdateTeamMemberStatus(attacker, index, true);
            
            // �Ϲ� ���� ��� ��� �������� �����ϰ�, �� ������ ���⼭ ����
            yield return StartCoroutine(TraitorSequence(attacker));
            yield break;
        }


        // ������ ������ ����
        elapsedTime = 0f;
        Vector3 startDashPosition = attacker.transform.position; // ���� ��ġ���� ����
        while (elapsedTime < attackDashDuration)
        {
            float t = elapsedTime / attackDashDuration;
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f);

            attacker.transform.position = Vector3.Lerp(startDashPosition, targetPosition, easedT);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        attacker.transform.position = targetPosition;

        // Ÿ�� �� ��� ���� 
        target.TakeDamage(attacker.GetAttackPower());

        yield return new WaitForSeconds(impactPauseDuration);

        // ���� ��ġ�� ���� �� ũ�� �������
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

        // ��ġ �� ũ�� ����
        attacker.transform.position = originalPosition;
        attacker.transform.localScale = originalScale;

        // ���� ī�� ó��
        if (target.IsDead())
        {
            targetList.Remove(target);

            Destroy(target.gameObject);
        }
    }

    /// <summary>
    /// ������ ī�� ����Ʈ�� �ش� ������ �� ��ġ�� �ε巴�� ���ġ�ϴ� �ڷ�ƾ
    /// </summary>
    private IEnumerator RearRangeField(List<BattleCard> cardList, Transform anchor, float spacing, float duration = 0.5f)
    {
        if (cardList.Count == 0) yield break;

        // �� ī���� ���� ��ǥ ��ġ ���
        float totalWidth = (cardList.Count - 1) * spacing;
        Vector3 startPos = anchor.position - new Vector3(totalWidth / 2f, 0, 0);

        Dictionary<BattleCard, Vector3> targetPositions = new Dictionary<BattleCard, Vector3>();
        for (int i = 0; i < cardList.Count; i++)
        {
            Vector3 targetPos = startPos + new Vector3(i * spacing, 0, 0);
            targetPositions[cardList[i]] = targetPos;
        }

        // ���� ��ġ ����
        Dictionary<BattleCard, Vector3> originalPositions = new Dictionary<BattleCard, Vector3>();
        foreach (var card in cardList)
        {
            originalPositions[card] = card.transform.position;
        }

        // Ÿ�̸Ӹ� �̿��� �ε巴�� �̵�
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            foreach (var card in cardList)
            {
                if (card != null) // ī�尡 �ı��Ǿ��� ��� ���
                {
                    card.transform.position = Vector3.Lerp(originalPositions[card], targetPositions[card], t);
                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ���� ��ġ ����
        foreach (var card in cardList)
        {
            if (card != null)
            {
                card.transform.position = targetPositions[card];
            }
        }
    }

    /// <summary>
    /// ����� ī���� ��� �������� ó���ϴ� �ڷ�ƾ
    /// </summary>
    private IEnumerator TraitorSequence(BattleCard traitor)
    {
        Debug.Log($"{traitor.name}��(��) ����߽��ϴ�!");

        // �Ʊ� ����Ʈ���� �����ϰ� ���� ����Ʈ�� ������ ��ġ�� �߰�
        m_teamCardList.Remove(traitor);
        int insertIndex = UnityEngine.Random.Range(0, m_enemyCardList.Count + 1); // �� �ڿ� �߰��� ���� �ֵ��� +1
        m_enemyCardList.Insert(insertIndex, traitor);
        traitor.transform.SetParent(m_enemyFieldAnchor); // �θ� ����

        // ���� ������ ���ÿ� �ε巴�� ���ġ
        Coroutine friendlyRearrange = StartCoroutine(RearRangeField(m_teamCardList, m_teamFieldAnchor, m_teamCardSpacing));
        Coroutine enemyRearrange = StartCoroutine(RearRangeField(m_enemyCardList, m_enemyFieldAnchor, m_enemyCardSpacing));

        // �� ���ġ�� ��� ���� ������ ���
        yield return friendlyRearrange;
        yield return enemyRearrange;

        // ��� ����Ͽ� ��Ȳ�� ������Ŵ
        yield return new WaitForSeconds(0.5f);

        // ���� ���� �� ����ڰ� �Ʊ��� ���� (���� ���� ������ ����)
        yield return StartCoroutine(AttackSequence(traitor));
    }

    // BattleField Ż���� �� ȣ����
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