using UnityEngine;

public class BattleCard : MonoBehaviour
{
    // NPCLookPart 에셋을 연결하기 위한 public 변수
    public NPCLookPart lookPartData;

    private CardData m_myData;
    private Renderer m_renderer;

    private void Awake()
    {
        m_renderer = GetComponent<Renderer>();
        if (m_renderer == null)
        {
            m_renderer = GetComponentInChildren<Renderer>();
        }
    }

    public void Init(CardData _data)
    {
        m_myData = _data;


        ApplyColorPalette(partType);

    }   

    /// <summary>
    /// 지정된 부위의 색상을 마스킹 셰이더에 적용합니다.
    /// 팔레트 인덱스는 m_myData에서 가져옵니다.
    /// </summary>
    /// <param name="_partType">색상을 가져올 부위 (상의, 얼굴 등)</param>
    public void ApplyColorPalette(CardData.NPCLookPartType _partType)
    {
        int paletteIndex = m_myData.ColorPalleteIndex;

        if (paletteIndex < 0 || paletteIndex >= lookPartData.ColorPalettes.Length)
        {
            Debug.LogError($"유효하지 않은 팔레트 인덱스({paletteIndex})입니다.");
            return;
        }

        // ScriptableObject에서 팔레트 데이터 가져오기
        NPCLookPart.ColorPalette palette = lookPartData.ColorPalettes[paletteIndex];
        int partIndex = m_myData.NPCLookTable[_partType];


        // 중요: .sharedMaterial 대신 .material을 사용해야 이 오브젝트에만 변경사항이 적용됩니다.
        Material mat = m_renderer.material;
        mat.SetColor("_ColorR", partColors.RedRegionColor);
        mat.SetColor("_ColorG", partColors.GreenRegionColor);
        mat.SetColor("_ColorB", partColors.BlueRegionColor);
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
}
