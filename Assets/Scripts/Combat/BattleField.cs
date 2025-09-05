using UnityEngine;
using System.Collections.Generic;

public class BattleField : MonoBehaviour
{
    [Header("ī�� ������")]
    [SerializeField] private GameObject m_cardPrefab;

    [Header("ī�� �ʵ� ����Ʈ")]
    private List<GameObject> m_friendlyCardList = new List<GameObject>();
    private List<GameObject> m_enemyCardList = new List<GameObject>();

    [Header("�÷��̾� ���� ����")]
    [SerializeField] private Transform playerFieldAnchor;
    [SerializeField] private float playerCardSpacing = 1.5f;

    [Header("�� ���� ����")]
    [SerializeField] private Transform enemyFieldAnchor;
    [SerializeField] private float enemyCardSpacing = 1.5f;


    /// <summary>
    /// ���� ������ ī�� GameObject�� ��ġ�ϰ� �����ϴ� ���� ������ �Լ�
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
        // ���� -> �Ʊ� ī�� 3��, ���� ī�� 4������ �ʵ� ���� �Ѵٰ� ����
        PlaceCards(3, 4);
    }
}