using UnityEngine;
using System.Collections.Generic;

public class BattleField : MonoBehaviour
{
    [Header("카드 프리팹")]
    [SerializeField] private GameObject m_cardPrefab;

    [Header("카드 필드 리스트")]
    private List<BattleCard> m_friendlyCardList;
    private List<BattleCard> m_enemyCardList;

    [Header("플레이어 진영 설정")]
    [SerializeField] private Transform playerFieldAnchor;
    [SerializeField] private float playerCardSpacing = 1.5f;

    [Header("적 진영 설정")]
    [SerializeField] private Transform enemyFieldAnchor;
    [SerializeField] private float enemyCardSpacing = 1.5f;

    // BattleField 세팅
    // 0. 세이브 데이터 로드하여 card, Item(인벤토리), 
    // 1. 카드 세팅
    // 2. UI 세팅
    public void Awake()
    {
        SaveDataBuffer.Instance.TryLoadData();
        
        m_friendlyCardList  = new List<BattleCard>();
        m_enemyCardList = new List<BattleCard>();

        List<CardData> cardDatas = SaveDataBuffer.Instance.Data.CardDatas;

        PlaceCards(cardDatas.Count, 4, cardDatas);
    }

    /// <summary>
    /// 월드 공간에 카드 GameObject를 배치하고 정렬하는 재사용 가능한 함수
    /// </summary>
    private void PlaceCards(int _friendlyCardCount, int _enemyCardCount, List<CardData> _playerCardDatas)
    {
        if (_friendlyCardCount <= 0 || _enemyCardCount <= 0) return;

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

        
        totalWidth = (_enemyCardCount - 1) * enemyCardSpacing;
        startPosition = enemyFieldAnchor.position - new Vector3(totalWidth / 2f, 0, 0);

        for (int i = 0; i < _enemyCardCount; i++)
        {
            CardData monsterData = new CardData();
            monsterData.Status = CardData.DefaultStatus();

            Vector3 cardPosition = startPosition + new Vector3(i * enemyCardSpacing, 0, 0);
            BattleCard newCard = Instantiate(m_cardPrefab, cardPosition, enemyFieldAnchor.rotation).AddComponent<BattleCard>();
            newCard.Init(monsterData);
            newCard.transform.SetParent(enemyFieldAnchor);
            m_enemyCardList.Add(newCard);
        }

    }

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