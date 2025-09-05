using UnityEngine;
using System.Collections.Generic;

public class BattleField : MonoBehaviour
{
    [Header("카드 프리팹")]
    [SerializeField] private GameObject m_cardPrefab;

    [Header("카드 필드 리스트")]
    private List<GameObject> m_friendlyCardList = new List<GameObject>();
    private List<GameObject> m_enemyCardList = new List<GameObject>();

    [Header("플레이어 진영 설정")]
    [SerializeField] private Transform playerFieldAnchor;
    [SerializeField] private float playerCardSpacing = 1.5f;

    [Header("적 진영 설정")]
    [SerializeField] private Transform enemyFieldAnchor;
    [SerializeField] private float enemyCardSpacing = 1.5f;


    /// <summary>
    /// 월드 공간에 카드 GameObject를 배치하고 정렬하는 재사용 가능한 함수
    /// </summary>
    private void PlaceCards(int friendlyCardCount, int enemyCardCount)
    {
        if (friendlyCardCount <= 0 || enemyCardCount <= 0) return;

        float totalWidth = (friendlyCardCount - 1) * playerCardSpacing;
        Vector3 startPosition = playerFieldAnchor.position - new Vector3(totalWidth / 2f, 0, 0);

        for (int i = 0; i < friendlyCardCount; i++)
        {
            Vector3 cardPosition = startPosition + new Vector3(i * playerCardSpacing, 0, 0);
            GameObject newCard = Instantiate(m_cardPrefab, cardPosition, playerFieldAnchor.rotation);
            newCard.transform.SetParent(playerFieldAnchor);
            m_friendlyCardList.Add(newCard);
        }

        
        totalWidth = (enemyCardCount - 1) * enemyCardSpacing;
        startPosition = enemyFieldAnchor.position - new Vector3(totalWidth / 2f, 0, 0);

        for (int i = 0; i < enemyCardCount; i++)
        {
            Vector3 cardPosition = startPosition + new Vector3(i * enemyCardSpacing, 0, 0);
            GameObject newCard = Instantiate(m_cardPrefab, cardPosition, enemyFieldAnchor.rotation);
            newCard.transform.SetParent(enemyFieldAnchor);
            m_enemyCardList.Add(newCard);
        }

    }

    void ExitBattleField()
    {
        foreach (GameObject card in m_friendlyCardList)
        {
            Destroy(card);
        }
        foreach (GameObject card in m_enemyCardList)
        {
            Destroy(card);
        }
        m_friendlyCardList.Clear();
        m_enemyCardList.Clear();
    }    

    public void BattleFieldStart()
    {
        // 예시 -> 아군 카드 3장, 적군 카드 4장으로 필드 구성 한다고 가정
        PlaceCards(3, 4);
    }
}