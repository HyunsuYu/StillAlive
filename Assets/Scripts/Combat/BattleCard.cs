using UnityEngine;

public class BattleCard : MonoBehaviour
{
    private CardData m_myData;

    public void Init(CardData _data)
    {
        m_myData = _data;
    }

    public int GetSpeed()
    {
        return m_myData.Status.Speed;
    }

    public int GetAttackPower()
    {
        return m_myData.Status.AttackPower;
    }

    public void TakeDamage(int damage)
    {
        // TODO: 방어력, 버프/디버프 등 추가 데미지 계산 로직
        int finalDamage = damage - m_myData.Status.DefencePower; 
        m_myData.Status.CurHP -= finalDamage;
        
        if (m_myData.Status.CurHP < 0)
        {
            m_myData.Status.CurHP = 0;
        }
        Debug.Log($"{name}이(가) {finalDamage}의 피해를 입었습니다. 남은 체력: {m_myData.Status.CurHP}");
    }

    public bool IsDead()
    {
        return m_myData.Status.CurHP <= 0;
    }
}
