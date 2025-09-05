using UnityEngine;

public class Explolor : MonoBehaviour
{
    [SerializeField] private GameObject SelecterWindow; // 선택 창
    [SerializeField] private GameObject CheckWindow; // 확인 창

    
    private int selectedButton = 0; // 선택된 버튼 (1: 아이템, 2: 동료)


    public int Day = 0; // 현재 날짜

    void Start()
    {
        CheckWindow.SetActive(false); // 시작 시 확인 창 비활성화
        SelecterWindow.SetActive(true); // 시작 시 이벤트 창 비활성화
    }

    public void OnItemButtonClick() //아이템 탐사 버튼을 눌렀을 때
    {
        selectedButton = 1;
        CheckWindow.SetActive(true);
    }
    public void OnColleagueButtonClick() //동료 탐사 버튼을 눌렀을 때
    {
        selectedButton = 2;
        CheckWindow.SetActive(true);
    }

    public void OnConfirmButtonClick() //확인 버튼을 눌렀을 때
    {
        if (selectedButton == 1)
        {
            ItemCatch();
        }
        else if (selectedButton == 2)
        {
            colleagueCatch();
        }
        CheckWindow.SetActive(false); // 확인 창 비활성화
        SelecterWindow.SetActive(false); // 선택 창 비활성화
        selectedButton = 0; // 선택된 버튼 초기화
    }

    public void OnCancelButtonClick()  //취소 버튼을 눌렀을 때
    {
        CheckWindow.SetActive(false); // 확인 창 비활성화
        selectedButton = 0; // 선택된 버튼 초기화
    }

    public void ItemCatch() //아이템 발견 함수
    {
        int itemCount = ProbabilityUtillity.GetCount(Day);
        
        if (itemCount > 0)
        {
            Debug.Log($"아이템 {itemCount}개 발견!");
            ItemTypes[] item = new ItemTypes[itemCount];

            for (int i = 0; i < itemCount; i++)
            {
                
            }
        }
        else
        {
            Debug.Log("아이템 발견 못함");
        }
    }

    public void colleagueCatch() //동료 발견 함수
    {
        int ColleagueCount = ProbabilityUtillity.GetCount(Day);
        if (ColleagueCount > 0)
        {
            Debug.Log($"동료 {ColleagueCount}명 발견!");
            for (int i = 0; i < ColleagueCount; i++)
            {
                // 동료 발견 시 추가 행동을 여기에 작성
                Debug.Log($"동료{i} 획득!");
            }
        }
        else
        {
            Debug.Log("동료 발견 못함");
        }
    }
}

// 아이템 드롭 확률을 계산하는 유틸리티 클래스
public static class ProbabilityUtillity
{
    // --- 확률 모델 설정값 ---

    // 확률 변화가 일어나는 총 기간을 90일로 수정
    private const float PROBABILITY_CHANGE_DURATION = 90.0f;

    // 시작 (Day 0) 지점의 경계선 Y값
    private const float startBoundary_1_0 = 0.05f; // 0개와 1개 경계
    private const float startBoundary_2_1 = 0.2f; // 1개와 2개 경계
    private const float startBoundary_3_2 = 0.4f; // 2개와 3개 경계

    // 마지막 (Day 90) 지점의 경계선 Y값
    private const float endBoundary_1_0 = 0.55f;
    private const float endBoundary_2_1 = 0.8f;
    private const float endBoundary_3_2 = 0.95f;


    // 특정 날짜를 기준으로 확률을 정한다.
    public static int GetCount(int currentDay)
    {
        // 1. 현재 날짜가 전체 기간 중 어디쯤인지 계산 (0.0 ~ 1.0 사이의 값)
        // currentDay가 90을 넘으면 progress는 1.0으로 고정됨
        float progress = Mathf.Clamp01(currentDay / PROBABILITY_CHANGE_DURATION);

        // 2. Lerp 함수로 '오늘'의 경계선 Y값들을 계산
        float todayBoundary_3_2 = Mathf.Lerp(startBoundary_3_2, endBoundary_3_2, progress);
        float todayBoundary_2_1 = Mathf.Lerp(startBoundary_2_1, endBoundary_2_1, progress);
        float todayBoundary_1_0 = Mathf.Lerp(startBoundary_1_0, endBoundary_1_0, progress);

        // 3. 0.0과 1.0 사이의 난수를 생성
        float randomValue = Random.value;

        // 4. 난수가 어느 영역에 속하는지 위에서부터 확인
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
