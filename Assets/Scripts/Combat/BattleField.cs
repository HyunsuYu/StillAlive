using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BattleField : MonoBehaviour
{
   
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

    private bool m_isStartBattle;

    // BattleField ����
    // 0. ���̺� ������ �ε��Ͽ� card, Item(�κ��丮), �����̻�
    // 1. ī�� ����
    // 2. UI ����
    public void Start()
    {
        SaveDataBuffer.Instance.TryLoadData();
        
        m_friendlyCardList  = new List<BattleCard>();
        m_enemyCardList = new List<BattleCard>();

        List<CardData> cardDatas = SaveDataBuffer.Instance.Data.CardDatas;
                
        Vector2Int nowPlayerMapPos = SaveDataBuffer.Instance.Data.CurPlayerMapPos;

        // TODO: ���߿� �����ؾ� �� �ڵ� 
        //MapNode.EventNodeType nowEventType = SaveDataBuffer.Instance.Data.MapData.Nodes[nowPlayerMapPos.y]
        //    .Find(node => node.XPos == nowPlayerMapPos.x).EventNodeType;

        // PlaceCards(nowEventType, cardDatas.Count, Random.Range(1, 5), cardDatas);
        m_isStartBattle = false;

        // �ӽ� �ڵ�
        PlaceCards(MapNode.EventNodeType.Combat_Common, playerTestCount, enemyTestCount, cardDatas);
   
    }

    /// <summary>
    /// ���� ������ ī�� GameObject�� ��ġ�ϰ� �����ϴ� ���� ������ �Լ�
    /// </summary>
    private void PlaceCards(MapNode.EventNodeType _eventType, int _friendlyCardCount, int _enemyCardCount, List<CardData> _playerCardDatas)
    {             
        if (_friendlyCardCount <= 0 || _enemyCardCount <= 0) return;

        // �÷��̾� ī�� ���� --> ���Ƿ� ������ ���� �� 
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
    /// UI ��ư Ŭ�� �� ������ �����ϴ� ���� �޼���
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
    /// �ڵ� ������ �����ϴ� ���� �ڷ�ƾ
    /// </summary>
    private IEnumerator StartCombat()
    {
        // ������ ���� ������ (���� ������ ��� ������ ������) �ݺ�
        while (m_friendlyCardList.Count > 0 && m_enemyCardList.Count > 0)
        {            
            List<BattleCard> nowBattleCard = new List<BattleCard>();
            nowBattleCard.AddRange(m_friendlyCardList);
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
                if (m_friendlyCardList.Count == 0 || m_enemyCardList.Count == 0)
                {
                    break;
                }
            }
            
            // �� ���� ���� �� ��� ���
            yield return new WaitForSeconds(0.5f);
        }
        
        Debug.Log("���� ����!");        
        // TODO:
        // ���� ��� â ǥ��, ���� ���� ��
    }
  
    /// <summary>
    /// ���� ī���� ���� �ִϸ��̼ǰ� ������ ó���ϴ� �ڷ�ƾ
    /// </summary>
    /// <param name="attacker">�����ϴ� ī��</param>
    private IEnumerator AttackSequence(BattleCard attacker)
    {
        // ���� ��� ����
        bool isFriendly = m_friendlyCardList.Contains(attacker);
        List<BattleCard> targetList = isFriendly ? m_enemyCardList : m_friendlyCardList;

        if (targetList.Count == 0) yield break;

        BattleCard target = targetList[Random.Range(0, targetList.Count)];
        Vector3 originalPosition = attacker.transform.position;
        Vector3 originalScale = attacker.transform.localScale;
        Vector3 targetPosition = target.transform.position;

        // �ִϸ��̼� ������ ���� ������ 
        float scaleAmount = 1.2f;         // �󸶳� Ŀ����
        float retreatDistance = 0.5f;     // ���� �Ÿ� 
        float retreatSpeed = 5f;
        float scaleUpDuration = 0.15f;    // Ŀ���� �� �ɸ��� �ð�
        float attackDashDuration = 0.2f;  // ������ �����ϴ� �ð�
        float impactPauseDuration = 0.1f; // ���� �ε����� �� ��� ���ߴ� �ð�
        float returnDuration = 0.4f;     // ���� �ڸ��� ���ƿ��� �ð�

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

        // ���� ī�� ó�� (�ּ� ���� �ʿ�)
        if (target.IsDead())
        {
            targetList.Remove(target);
            Destroy(target.gameObject); 
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