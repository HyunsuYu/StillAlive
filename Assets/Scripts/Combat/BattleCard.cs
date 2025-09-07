using UnityEngine;
using UnityEngine.UI;

public class BattleCard : MonoBehaviour
{
    private CardData m_myData;

    public CardData Data
    {
        get { return m_myData; }
    }

    [SerializeField] private Transform portraitPos;
    private NPCPortrait m_portraitInstance; // 생성된 초상화 인스턴스

    public void Init(CardData _data)
    {
        m_myData = _data;

        // 기존 초상화가 있다면 파괴
        if (m_portraitInstance != null)
        {
            Destroy(m_portraitInstance);
        }

        // CharacterPortraitHelper를 사용하여 초상화 생성 (Resources에서 자동 로드)
        m_portraitInstance = CharacterPortraitHelper.CreatePortrait(m_myData).GetComponent<NPCPortrait>();
        
        if (m_portraitInstance != null)
        {
            // 생성된 초상화를 이 오브젝트의 자식으로 설정
            m_portraitInstance.transform.SetParent(portraitPos);
        }
        else
        {
            Debug.LogWarning("BattleCard: 초상화 생성에 실패했습니다. Resources 폴더에 'Portrait' 프리팹과 'NPCLookPart' 에셋이 있는지 확인해주세요.");
        }
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
        int finalDamage = damage - m_myData.Status.DefencePower;
        if (finalDamage < 1) finalDamage = 1; // 최소 1의 데미지 보장
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
   
    /// <summary>
    /// 특정 부위의 색상만 변경합니다.
    /// </summary>
    public void ChangePartColor(CardData.NPCLookPartType partType)
    {
        if (m_portraitInstance != null)
        {
            CharacterPortraitHelper.ApplyPartColor(m_portraitInstance.gameObject, partType);
        }
    }

    private void OnDestroy()
    {
        // 오브젝트가 파괴될 때 초상화도 함께 정리
        if (m_portraitInstance != null)
        {
            Destroy(m_portraitInstance);
        }
    }
}