using UnityEngine;

public class Explolor : MonoBehaviour
{
    [SerializeField] private GameObject SelecterWindow; // ���� â
    [SerializeField] private GameObject CheckWindow; // Ȯ�� â

    
    private int selectedButton = 0; // ���õ� ��ư (1: ������, 2: ����)


    public int Day = 0; // ���� ��¥

    void Start()
    {
        CheckWindow.SetActive(false); // ���� �� Ȯ�� â ��Ȱ��ȭ
        SelecterWindow.SetActive(true); // ���� �� �̺�Ʈ â ��Ȱ��ȭ
    }

    public void OnItemButtonClick() //������ Ž�� ��ư�� ������ ��
    {
        selectedButton = 1;
        CheckWindow.SetActive(true);
    }
    public void OnColleagueButtonClick() //���� Ž�� ��ư�� ������ ��
    {
        selectedButton = 2;
        CheckWindow.SetActive(true);
    }

    public void OnConfirmButtonClick() //Ȯ�� ��ư�� ������ ��
    {
        if (selectedButton == 1)
        {
            ItemCatch();
        }
        else if (selectedButton == 2)
        {
            colleagueCatch();
        }
        CheckWindow.SetActive(false); // Ȯ�� â ��Ȱ��ȭ
        SelecterWindow.SetActive(false); // ���� â ��Ȱ��ȭ
        selectedButton = 0; // ���õ� ��ư �ʱ�ȭ
    }

    public void OnCancelButtonClick()  //��� ��ư�� ������ ��
    {
        CheckWindow.SetActive(false); // Ȯ�� â ��Ȱ��ȭ
        selectedButton = 0; // ���õ� ��ư �ʱ�ȭ
    }

    public void ItemCatch() //������ �߰� �Լ�
    {
        int itemCount = ProbabilityUtillity.GetCount(Day);
        
        if (itemCount > 0)
        {
            Debug.Log($"������ {itemCount}�� �߰�!");
            ItemTypes[] item = new ItemTypes[itemCount];

            for (int i = 0; i < itemCount; i++)
            {
                
            }
        }
        else
        {
            Debug.Log("������ �߰� ����");
        }
    }

    public void colleagueCatch() //���� �߰� �Լ�
    {
        int ColleagueCount = ProbabilityUtillity.GetCount(Day);
        if (ColleagueCount > 0)
        {
            Debug.Log($"���� {ColleagueCount}�� �߰�!");
            for (int i = 0; i < ColleagueCount; i++)
            {
                // ���� �߰� �� �߰� �ൿ�� ���⿡ �ۼ�
                Debug.Log($"����{i} ȹ��!");
            }
        }
        else
        {
            Debug.Log("���� �߰� ����");
        }
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
