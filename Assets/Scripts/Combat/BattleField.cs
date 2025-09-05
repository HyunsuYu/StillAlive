using UnityEngine;
using System.Collections.Generic;
using static CardData;

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

    // BattleField ����
    // 0. ���̺� ������ �ε��Ͽ� card, Item(�κ��丮), 
    // 1. ī�� ����
    // 2. UI ����
    public void Awake()
    {
        SaveDataBuffer.Instance.TryLoadData();
        
        m_friendlyCardList  = new List<BattleCard>();
        m_enemyCardList = new List<BattleCard>();

        List<CardData> cardDatas = SaveDataBuffer.Instance.Data.CardDatas;

        PlaceCards(cardDatas.Count, 4, cardDatas);
    }

    /// <summary>
    /// ���� ������ ī�� GameObject�� ��ġ�ϰ� �����ϴ� ���� ������ �Լ�
    /// </summary>
    private void PlaceCards(int _friendlyCardCount, int _enemyCardCount, List<CardData> _playerCardDatas)
    {
        if (_friendlyCardCount <= 0 || _enemyCardCount <= 0) return;

        // �÷��̾� ī�� ����
        float totalWidth = (_friendlyCardCount - 1) * playerCardSpacing;
        Vector3 startPosition = playerFieldAnchor.position - new Vector3(totalWidth / 2f, 0, 0);       

        for (int i = 0; i < _friendlyCardCount; i++)
        {
            Vector3 cardPosition = startPosition + new Vector3(i * playerCardSpacing, 0, 0);
            BattleCard newCard = Instantiate(m_cardPrefab, cardPosition, playerFieldAnchor.rotation).AddComponent<BattleCard>();
            newCard.Init(_playerCardDatas[i]);
            newCard.transform.SetParent(playerFieldAnchor);
            m_friendlyCardList.Add(newCard);
        }

        
        // ���� ī�� ����
        totalWidth = (_enemyCardCount - 1) * enemyCardSpacing;
        startPosition = enemyFieldAnchor.position - new Vector3(totalWidth / 2f, 0, 0);

        int nowDay = SaveDataBuffer.Instance.Data.DPlusDay;

        for (int i = 0; i < _enemyCardCount; i++)
        {
            // ���� ���� ����
            CardData monsterData = new CardData();
            for(int j = 0; j< nowDay; j++)
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


            Vector3 cardPosition = startPosition + new Vector3(i * enemyCardSpacing, 0, 0);
            BattleCard newCard = Instantiate(m_cardPrefab, cardPosition, enemyFieldAnchor.rotation).AddComponent<BattleCard>();
            newCard.Init(monsterData);
            newCard.transform.SetParent(enemyFieldAnchor);
            m_enemyCardList.Add(newCard);
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