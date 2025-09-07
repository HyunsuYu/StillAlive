using CommonUtilLib.ThreadSafe;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Explolor : SingleTonForGameObject<Explolor>
{
    [Header("Windows")]
    [SerializeField] private GameObject SelecterWindow; // ���� â
    [SerializeField] private GameObject CheckWindow; // Ȯ�� â
    [SerializeField] private GameObject EventWindow; // �̺�Ʈ â
    [SerializeField] private GameObject ItemWindow;
    [SerializeField] private GameObject ColleagueWindow;

    [Header("Objects")]
    [SerializeField] private GameObject[] ItemBox;
    [SerializeField] private GameObject[] ColleagueBox;
    [SerializeField] private GameObject[] MyTeamBox;

    [Header("Text")]
    [SerializeField] private TMP_Text m_text_DPlusDay;

    public List<CardData> ColleaugueCards;

    public enum ExplolorState
    {
        None = 0,
        Selecter = 1,
        ItemEvent = 2,
        ColleagueEvent = 3
    }


    public ExplolorState exState = ExplolorState.None;
    static bool curCompleteWork = true;

    void Awake()
    {
        SetInstance(this);
    }

    public void OnItemButtonClick() //������ Ž�� ��ư�� ������ ��
    {
        exState = ExplolorState.ItemEvent;
        ItemCatch();
        CheckWindow.SetActive(false); // Ȯ�� â ��Ȱ��ȭ
        SelecterWindow.SetActive(false); // ���� â ��Ȱ��ȭ
        exState = ExplolorState.None; // ���õ� ��ư �ʱ�ȭ
    }

    public void OnColleagueButtonClick() //���� Ž�� ��ư�� ������ ��
    {
        exState = ExplolorState.ColleagueEvent;
        colleagueCatch();
        CheckWindow.SetActive(false); // Ȯ�� â ��Ȱ��ȭ
        SelecterWindow.SetActive(false); // ���� â ��Ȱ��ȭ
        exState = ExplolorState.None; // ���õ� ��ư �ʱ�ȭ
    }


    public void OnCancelButtonClick()  //��� ��ư�� ������ ��
    {
        CheckWindow.SetActive(false); // Ȯ�� â ��Ȱ��ȭ
        exState = ExplolorState.None; // ���õ� ��ư �ʱ�ȭ
    }

    public void ItemCatch() //������ �߰� �Լ�
    {
        if(exState != ExplolorState.ItemEvent)
        {
            Debug.LogWarning("ItemCatch ȣ�� ����: ���� ���°� ItemEvent�� �ƴմϴ�.");

            return;
        }
        int itemCount = ProbabilityUtillity.GetCount(SaveDataBuffer.Instance.Data.DPlusDay);

        if (itemCount > 0)
        {
            Debug.Log($"������ {itemCount}�� �߰�!");
            for (int i = 0; i < itemCount; i++)
            {
                ItemBox[i].SetActive(true);
            }

            EventWindow.SetActive(true); // �̺�Ʈ â Ȱ��ȭ
            ItemWindow.SetActive(true);

        }
        else
        {
            Debug.Log("������ �߰� ����");
        }
    }

    public void colleagueCatch() //���� �߰� �Լ�
    {
        if (exState != ExplolorState.ColleagueEvent)
        {
            Debug.LogWarning("colleagueCatch ȣ�� ����: ���� ���°� ColleagueEvent�� �ƴմϴ�.");
            return;
        }
        int ColleagueCount = ProbabilityUtillity.GetCount(SaveDataBuffer.Instance.Data.DPlusDay);
        SpawnCards(ColleagueCount);
    }

    private void SpawnCards(int count)
    {
        // ��� ī�� UI�� �̸� ��Ȱ��ȭ
        foreach (var box in ColleagueBox) box.SetActive(false);
        foreach (var box in MyTeamBox) box.SetActive(false);

        if (count > 0)
        {
            Debug.Log($"���� {count}�� �߰�!");
            // 1. ���� �߰��� ���� (�ӽ� ������) ǥ��
            for (int i = 0; i < count; i++)
            {
                CardData newColleague = new CardData();
                newColleague.Status = CardData.DefaultStatus();
                newColleague.NPCLookTable = new Dictionary<CardData.NPCLookPartType, int>();
                newColleague.NPCLookTable[CardData.NPCLookPartType.Top] = Random.Range(0, 3);
                // To-Do: ���� �����͵� ���⼭ �����ϸ� �����ϴ�.
                // newColleague.NPCLookTable = ...;

                GameObject colleagueGO = ColleagueBox[i];
                GameObject cardObj = Resources.Load<GameObject>("Prefabs/BattleScene/BattleCard_UI");
                BattleCard card  = Instantiate(cardObj.gameObject, colleagueGO.transform)?.GetComponent<BattleCard>();
                if (card != null)
                {
                    card.Init(newColleague, true);
                    colleagueGO.SetActive(true);
                }
            }
        }
        else
        {
            Debug.Log("���� �߰� ����");
        }

        // 2. ���� �� �� (���� ������) ǥ��
        var currentTeam = SaveDataBuffer.Instance.Data.CardDatas;
        for (int j = 0; j < currentTeam.Count && j < MyTeamBox.Length; j++)
        {
            GameObject myTeamGO = MyTeamBox[j];
            BattleCard card = myTeamGO.GetComponent<BattleCard>();
            if (card != null)
            {
                card.Init(currentTeam[j]);
                myTeamGO.SetActive(true);
            }
        }

        EventWindow.SetActive(true);
        ColleagueWindow.SetActive(true);
        exState = ExplolorState.None; // ���� �ʱ�ȭ
    }

    internal void Render()
    {
        m_text_DPlusDay.text = "D+" + SaveDataBuffer.Instance.Data.DPlusDay.ToString();
        if (curCompleteWork)
        {
            CheckWindow.SetActive(false); // ���� �� Ȯ�� â ��Ȱ��ȭ
            SelecterWindow.SetActive(true); // ���� �� �̺�Ʈ â Ȱ��ȭ
            EventWindow.SetActive(false); // ���� �� �̺�Ʈ â ��Ȱ��ȭ
            ItemWindow.SetActive(false);
            ColleagueWindow.SetActive(false);
            for (int i = 0; i < ItemBox.Length; i++)
            {
                ItemBox[i].SetActive(false);
            }
            for (int i = 0; i < ColleagueBox.Length; i++)
            {
                ColleagueBox[i].SetActive(false);
            }
            for (int i = 0; i < MyTeamBox.Length; i++)
            {
                MyTeamBox[i].SetActive(false);
            }
            exState = ExplolorState.None; // ���� �ʱ�ȭ     
        }
    }


    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}

// ������ ��� Ȯ���� ����ϴ� ��ƿ��Ƽ Ŭ����
public static class ProbabilityUtillity
{
    // --- Ȯ�� �� ������ ---

    // Ȯ�� ��ȭ�� �Ͼ�� �� �Ⱓ�� 90�Ϸ� ����
    private const float PROBABILITY_CHANGE_DURATION = 90.0f;

    // ���� (Day 0) ������ ��輱 Y��
    private const float startBoundary_1_0 = 0.05f; // 0���� 1�� ���
    private const float startBoundary_2_1 = 0.2f; // 1���� 2�� ���
    private const float startBoundary_3_2 = 0.4f; // 2���� 3�� ���

    // ������ (Day 90) ������ ��輱 Y��
    private const float endBoundary_1_0 = 0.55f;
    private const float endBoundary_2_1 = 0.8f;
    private const float endBoundary_3_2 = 0.95f;


    // Ư�� ��¥�� �������� Ȯ���� ���Ѵ�.
    public static int GetCount(int currentDay)
    {
        // 1. ���� ��¥�� ��ü �Ⱓ �� ��������� ��� (0.0 ~ 1.0 ������ ��)
        // currentDay�� 90�� ������ progress�� 1.0���� ������
        float progress = Mathf.Clamp01(currentDay / PROBABILITY_CHANGE_DURATION);

        // 2. Lerp �Լ��� '����'�� ��輱 Y������ ���
        float todayBoundary_3_2 = Mathf.Lerp(startBoundary_3_2, endBoundary_3_2, progress);
        float todayBoundary_2_1 = Mathf.Lerp(startBoundary_2_1, endBoundary_2_1, progress);
        float todayBoundary_1_0 = Mathf.Lerp(startBoundary_1_0, endBoundary_1_0, progress);

        // 3. 0.0�� 1.0 ������ ������ ����
        float randomValue = Random.value;

        // 4. ������ ��� ������ ���ϴ��� ���������� Ȯ��
        if (randomValue >= todayBoundary_3_2)
        {
            return 3;
        }
        else if (randomValue >= todayBoundary_2_1)
        {
            return 2;
        }
        else if (randomValue >= todayBoundary_1_0)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}
