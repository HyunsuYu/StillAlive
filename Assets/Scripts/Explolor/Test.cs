using System;
using System.Collections.Generic;

public class ItemDropSimulator
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

    // 난수 생성을 위한 객체
    private Random random;

    public ItemDropSimulator()
    {
        this.random = new Random();
    }

    private float Lerp(float start, float end, float t)
    {
        return start + (end - start) * t;
    }

    /// <summary>
    /// 특정 날짜를 기준으로 아이템 드랍 개수를 결정합니다.
    /// </summary>
    /// <param name="currentDay">현재 날짜 (D+x)</param>
    /// <returns>획득한 아이템 개수 (0, 1, 2, 또는 3)</returns>
    public int GetItemCount(int currentDay)
    {
        // 1. 현재 날짜가 전체 기간 중 어디쯤인지 계산 (0.0 ~ 1.0 사이의 값)
        // currentDay가 90을 넘으면 progress는 1.0으로 고정됨
        float progress = Math.Clamp(currentDay / PROBABILITY_CHANGE_DURATION, 0.0f, 1.0f);

        // 2. Lerp 함수로 '오늘'의 경계선 Y값들을 계산
        float todayBoundary_3_2 = Lerp(startBoundary_3_2, endBoundary_3_2, progress);
        float todayBoundary_2_1 = Lerp(startBoundary_2_1, endBoundary_2_1, progress);
        float todayBoundary_1_0 = Lerp(startBoundary_1_0, endBoundary_1_0, progress);

        // 3. 0.0과 1.0 사이의 난수를 생성
        float randomValue = (float)random.NextDouble();

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

// --- 테스트를 위한 실행 코드 ---
public class Program
{
    public static void Main(string[] args)
    {
        ItemDropSimulator simulator = new ItemDropSimulator();

        Console.WriteLine("---- 초반 (1일차) 드랍 시뮬레이션 (5회) ----");
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine($"시도 {i + 1}: 아이템 {simulator.GetItemCount(1)}개 획득");
        }

        Console.WriteLine("\n---- 확률 고정 시점 (90일차) 드랍 시뮬레이션 (5회) ----");
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine($"시도 {i + 1}: 아이템 {simulator.GetItemCount(90)}개 획득");
        }

        // 90일 이후에도 확률이 동일한지 확인
        Console.WriteLine("\n---- 90일 이후 (120일차) 드랍 시뮬레이션 (5회) ----");
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine($"시도 {i + 1}: 아이템 {simulator.GetItemCount(120)}개 획득");
        }
    }
}