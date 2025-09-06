using UnityEngine;
using System.Collections.Generic;
using static CardData;
using UnityEditor.Experimental.GraphView;
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

        // PlaceCards(nowEventType, playerTestCount, enemyTestCount, cardDatas);

        // �ӽ� �ڵ�
        PlaceCards(MapNode.EventNodeType.Combat_Common, playerTestCount, enemyTestCount, cardDatas);
   
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
        
        _playerCardDatas = new List<CardData>();

        for (int i = 0; i < _friendlyCardCount; i++)
        {
            CardData cd = new CardData();
            _playerCardDatas.Add(cd);
            cd.NPCLookTable = new Dictionary<NPCLookPartType, int>();

            _playerCardDatas[i].NPCLookTable[NPCLookPartType.Face] = 0;
            _playerCardDatas[i].NPCLookTable[NPCLookPartType.Eye] = 0;
            _playerCardDatas[i].NPCLookTable[NPCLookPartType.Glasses] = 0;


            Vector3 cardPosition = startPosition + new Vector3(i * playerCardSpacing, 0, 0);
            GameObject newCardObject = Instantiate(m_cardPrefab, cardPosition, playerFieldAnchor.rotation);
            BattleCard newCard = newCardObject.AddComponent<BattleCard>();
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
                BattleCard newCard = newCardObject.AddComponent<BattleCard>();
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
        Debug.Log("���� ���� ��ư Ŭ����!");
        StartCoroutine(StartCombat());
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
        // ���� ��� ����Ʈ ����
        bool isFriendly = m_friendlyCardList.Contains(attacker);
        List<BattleCard> targetList = isFriendly ? m_enemyCardList : m_friendlyCardList;

        // ������ ����� ������ ��� ����
        if (targetList.Count == 0)
        {
            yield break;
        }

        // ���� Ÿ�� ����
        BattleCard target = targetList[Random.Range(0, targetList.Count)];
        Vector3 originalPosition = attacker.transform.position;
        Vector3 targetPosition = target.transform.position;

        // ���� �ִϸ��̼�
        float moveSpeed = 5f;
        float retreatDistance = 0.5f;

        // �ڷ� ��¦ ��������
        Vector3 retreatPosition = originalPosition - (targetPosition - originalPosition).normalized * retreatDistance;
        while (Vector3.Distance(attacker.transform.position, retreatPosition) > 0.01f)
        {
            attacker.transform.position = Vector3.Lerp(attacker.transform.position, retreatPosition, Time.deltaTime * moveSpeed);
            yield return null;
        }

        // Ÿ���� ���� ����
        while (Vector3.Distance(attacker.transform.position, targetPosition) > 0.5f) // �浹 �Ÿ� ����
        {
            attacker.transform.position = Vector3.Lerp(attacker.transform.position, targetPosition, Time.deltaTime * moveSpeed);
            yield return null;
        }

        // ������ ó��
        Debug.Log($"{attacker.name}�� {target.name}���� ������");
        target.TakeDamage(attacker.GetAttackPower()); 
        
        // TODO: ������ �ؽ�Ʈ ǥ��, �ǰ� ȿ����, Ÿ�ݽ�, ����ŷ�� ���̴�ȿ��

        // ���� ��ġ�� ����
        while (Vector3.Distance(attacker.transform.position, originalPosition) > 0.01f)
        {
            attacker.transform.position = Vector3.Lerp(attacker.transform.position, originalPosition, Time.deltaTime * moveSpeed);
            yield return null;
        }
        attacker.transform.position = originalPosition; // ��Ȯ�� ��ġ ����

        // ���� ī�� ó��
        // if (target.IsDead())
        // {
        //     targetList.Remove(target);
        //     target.gameObject.SetActive(false);
        // }
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