using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;


// 수정이 필요한 시점
// NPCLookPartType이 늘어나면, Switch문을 변경해야 함.
// Material은 인스턴스로 생성하고 있음 --> 성능을 잡아먹을 수 있음

/// <summary>
/// 동적 머티리얼에 텍스처를 수동 할당하고 있음 
/// </summary>
public class NPCPortrait : MonoBehaviour
{
    private const string MATERIAL_PATH = "LoadMaterials/PortraitMaterial";
    // private const string ENEMY_MATERIAL_PATH = "LoadMaterials/EnemyPortraitMaterial";

    [Header("부위별 Image 설정")]
    [SerializeField] private Image topImage;
    [SerializeField] private Image faceImage;
    [SerializeField] private Image frontHairImage;
    [SerializeField] private Image backHairImage;
    [SerializeField] private Image eyeImage;
    [SerializeField] private Image mouthImage;
    [SerializeField] private Image glassesImage;
    [SerializeField] private Image capImage;

    [Header("외형 데이터")]
    [SerializeField] private NPCLookPart m_lookPartData;
    public NPCLookPart GetLookPartData => m_lookPartData;

    private Dictionary<CardData.NPCLookPartType, Material> m_partMaterials;

    public void Init(CardData cardData)
    {
        CreatePartMaterials();
        SetupAllPartSprites(cardData);
    }

    private void SetupAllPartSprites(CardData _cardData)
    {
        if (_cardData.ColorPalleteIndex < 0 || _cardData.ColorPalleteIndex >= m_lookPartData.ColorPalettes.Length) return;

        foreach (var partInfo in _cardData.NPCLookTable)
        {
            SetPartSprite(partInfo.Key, partInfo.Value);
        }

        NPCLookPart.ColorPalette palette = m_lookPartData.ColorPalettes[_cardData.ColorPalleteIndex];

        ApplyPartColor(CardData.NPCLookPartType.Top, palette.TopColors);
        ApplyPartColor(CardData.NPCLookPartType.Face, palette.FacesColors);
        ApplyPartColor(CardData.NPCLookPartType.FrontHair, palette.FrontHairsColors);
        ApplyPartColor(CardData.NPCLookPartType.BackHair, palette.BackHairsColors);
        ApplyPartColor(CardData.NPCLookPartType.Eye, palette.EyesColors);
        ApplyPartColor(CardData.NPCLookPartType.Mouth, palette.MouthsColors);
        ApplyPartColor(CardData.NPCLookPartType.Glasses, palette.GlassesColors);
        ApplyPartColor(CardData.NPCLookPartType.Cap, palette.CapsColors);
    }
 
    private void CreatePartMaterials()
    {
        string materialPath = MATERIAL_PATH;
        Material originalMaterial = Resources.Load<Material>(materialPath);
        if (originalMaterial == null) return;

        m_partMaterials = new Dictionary<CardData.NPCLookPartType, Material>();

        foreach (CardData.NPCLookPartType partType in System.Enum.GetValues(typeof(CardData.NPCLookPartType)))
        {
            Image targetImage = GetImageByPartType(partType);
            if (targetImage != null)
            {
                Material partMaterial = Instantiate(originalMaterial);
                targetImage.material = partMaterial;
                m_partMaterials[partType] = partMaterial;
            }
        }
    }

    public void SetPartSprite(CardData.NPCLookPartType partType, int spriteIndex)
    {
        Image targetImage = GetImageByPartType(partType);
        Sprite[] spriteArray = GetSpriteArrayByPartType(partType);

        if (targetImage != null && spriteArray != null && spriteIndex >= 0 && spriteIndex < spriteArray.Length)
        {
            targetImage.sprite = spriteArray[spriteIndex];
            targetImage.enabled = targetImage.sprite != null;
      
            if (m_partMaterials.TryGetValue(partType, out Material partMaterial) && targetImage.sprite != null)
            {
                partMaterial.SetTexture("_MainTex", targetImage.sprite.texture);
            }
        }
        else if (targetImage != null)
        {
            targetImage.enabled = false;
        }
    }

    // 각각 파츠마다 컬러 부여
    public void ApplyPartColor(CardData.NPCLookPartType partType, NPCLookPart.ColorPalette.LookPartColors colors)
    {
        if (m_partMaterials != null && m_partMaterials.TryGetValue(partType, out Material partMaterial))
        {
            partMaterial.SetColor("_ColorR", colors.RedRegionColor);
            partMaterial.SetColor("_ColorG", colors.GreenRegionColor);
            partMaterial.SetColor("_ColorB", colors.BlueRegionColor);
            partMaterial.SetColor("_ColorRG", colors.RedGreenRegionColor);
            partMaterial.SetColor("_ColorRB", colors.RedBlueRegionColor);
            partMaterial.SetColor("_ColorGB", colors.GreenBlueRegionColor);
            partMaterial.SetColor("_ColorRGB", colors.RedGreenBlueRegionColor);
        }
    }

    private Image GetImageByPartType(CardData.NPCLookPartType partType)
    {
        switch (partType)
        {
            case CardData.NPCLookPartType.Top: return topImage;
            case CardData.NPCLookPartType.Face: return faceImage;
            case CardData.NPCLookPartType.FrontHair: return frontHairImage;
            case CardData.NPCLookPartType.BackHair: return backHairImage;
            case CardData.NPCLookPartType.Eye: return eyeImage;
            case CardData.NPCLookPartType.Mouth: return mouthImage;
            case CardData.NPCLookPartType.Glasses: return glassesImage;
            case CardData.NPCLookPartType.Cap: return capImage;
            default: return null;
        }
    }

    private Sprite[] GetSpriteArrayByPartType(CardData.NPCLookPartType partType)
    {
        NPCLookPart.PartData[] partDataArray = null;
        
        switch (partType)
        {
            case CardData.NPCLookPartType.Top: partDataArray = m_lookPartData.Tops; break;
            case CardData.NPCLookPartType.Face: partDataArray = m_lookPartData.Faces; break;
            case CardData.NPCLookPartType.FrontHair: partDataArray = m_lookPartData.FrontHairs; break;
            case CardData.NPCLookPartType.BackHair: partDataArray = m_lookPartData.BackHairs; break;
            case CardData.NPCLookPartType.Eye: partDataArray = m_lookPartData.Eyes; break;
            case CardData.NPCLookPartType.Mouth: partDataArray = m_lookPartData.Mouths; break;
            case CardData.NPCLookPartType.Glasses: partDataArray = m_lookPartData.Glasses; break;
            case CardData.NPCLookPartType.Cap: partDataArray = m_lookPartData.Caps; break;
            default: return null;
        }
        
        if (partDataArray == null) return null;
        
        // PartData[]에서 Sprite[] 추출
        Sprite[] spriteArray = new Sprite[partDataArray.Length];
        for (int i = 0; i < partDataArray.Length; i++)
        {
            spriteArray[i] = partDataArray[i].PartSprite;
        }
        
        return spriteArray;
    }

    private NPCLookPart.PartData[] GetPartDataArrayByType(CardData.NPCLookPartType partType)
    {
        switch (partType)
        {
            case CardData.NPCLookPartType.Top:
                return m_lookPartData.Tops;

            case CardData.NPCLookPartType.Face:
                return m_lookPartData.Faces;

            case CardData.NPCLookPartType.FrontHair:
                return m_lookPartData.FrontHairs;

            case CardData.NPCLookPartType.BackHair:
                return m_lookPartData.BackHairs;

            case CardData.NPCLookPartType.Eye:
                return m_lookPartData.Eyes;

            case CardData.NPCLookPartType.Mouth:
                return m_lookPartData.Mouths;

            case CardData.NPCLookPartType.Glasses:
                return m_lookPartData.Glasses;

            case CardData.NPCLookPartType.Cap:
                return m_lookPartData.Caps;

            default:
                return null;
        }
    }

    /// <summary>
    /// 특정 부위의 Description을 가져옵니다.
    /// </summary>
    /// <param name="cardData">카드 데이터</param>
    /// <param name="partType">부위 타입</param>
    /// <returns>해당 부위의 Description, 없으면 null</returns>
    public string GetPartDescription(CardData cardData, CardData.NPCLookPartType partType)
    {
        if (cardData.NPCLookTable == null || m_lookPartData == null)
            return null;

        // NPCLookTable에서 해당 부위의 인덱스 가져오기
        if (!cardData.NPCLookTable.TryGetValue(partType, out int partIndex))
            return null;

        // 해당 부위의 PartData 배열 가져오기
        NPCLookPart.PartData[] partDataArray = GetPartDataArrayByType(partType);
        if (partDataArray == null || partIndex < 0 || partIndex >= partDataArray.Length)
            return null;

        return partDataArray[partIndex].Description;
    }

    // 히트 이펙트
    public IEnumerator PlayHitEffect(float duration = 0.15f)
    {
        if (m_partMaterials != null)
        {
            foreach (var material in m_partMaterials.Values)
            {
                if (material != null && material.HasProperty("_Color"))
                {
                    material.SetColor("_Color", Color.red);
                }
            }
            yield return new WaitForSeconds(duration);
            foreach (var material in m_partMaterials.Values)
            {
                if (material != null && material.HasProperty("_Color"))
                {
                    material.SetColor("_Color", Color.white);
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (m_partMaterials != null)
        {
            foreach (var material in m_partMaterials.Values)
            {
                if (material != null) Destroy(material);
            }
            m_partMaterials.Clear();
        }
    }
}