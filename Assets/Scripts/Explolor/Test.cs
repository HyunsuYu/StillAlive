using System;
using System.Collections.Generic;

public class ItemDropSimulator
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

    // ���� ������ ���� ��ü
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
    /// Ư�� ��¥�� �������� ������ ��� ������ �����մϴ�.
    /// </summary>
    /// <param name="currentDay">���� ��¥ (D+x)</param>
    /// <returns>ȹ���� ������ ���� (0, 1, 2, �Ǵ� 3)</returns>
    public int GetItemCount(int currentDay)
    {
        // 1. ���� ��¥�� ��ü �Ⱓ �� ��������� ��� (0.0 ~ 1.0 ������ ��)
        // currentDay�� 90�� ������ progress�� 1.0���� ������
        float progress = Math.Clamp(currentDay / PROBABILITY_CHANGE_DURATION, 0.0f, 1.0f);

        // 2. Lerp �Լ��� '����'�� ��輱 Y������ ���
        float todayBoundary_3_2 = Lerp(startBoundary_3_2, endBoundary_3_2, progress);
        float todayBoundary_2_1 = Lerp(startBoundary_2_1, endBoundary_2_1, progress);
        float todayBoundary_1_0 = Lerp(startBoundary_1_0, endBoundary_1_0, progress);

        // 3. 0.0�� 1.0 ������ ������ ����
        float randomValue = (float)random.NextDouble();

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

// --- �׽�Ʈ�� ���� ���� �ڵ� ---
public class Program
{
    public static void Main(string[] args)
    {
        ItemDropSimulator simulator = new ItemDropSimulator();

        Console.WriteLine("---- �ʹ� (1����) ��� �ùķ��̼� (5ȸ) ----");
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine($"�õ� {i + 1}: ������ {simulator.GetItemCount(1)}�� ȹ��");
        }

        Console.WriteLine("\n---- Ȯ�� ���� ���� (90����) ��� �ùķ��̼� (5ȸ) ----");
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine($"�õ� {i + 1}: ������ {simulator.GetItemCount(90)}�� ȹ��");
        }

        // 90�� ���Ŀ��� Ȯ���� �������� Ȯ��
        Console.WriteLine("\n---- 90�� ���� (120����) ��� �ùķ��̼� (5ȸ) ----");
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine($"�õ� {i + 1}: ������ {simulator.GetItemCount(120)}�� ȹ��");
        }
    }
}