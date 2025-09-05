using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// NPC 초상화를 관리하는 클래스. 각 부위별 Image를 직접 관리하고 파츠 교체 및 색상 변경 기능을 제공한다.
/// </summary>
public class NPCPortrait : MonoBehaviour
{
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

    private CardData m_cardData;
    public CardData GetCardData => m_cardData;

    /// <summary>
    /// 초상화를 설정합니다.
    /// </summary>
    /// <param name="cardData">설정할 카드 데이터</param>
    public void SetupPortrait(CardData cardData)
    {
        m_cardData = cardData;

        if (this.m_lookPartData == null || m_cardData.NPCLookTable == null)
        {
            Debug.LogError("NPCPortrait: lookPartData가 설정되지 않았습니다.");
            return;
        }

        foreach (var part in m_cardData.NPCLookTable)
        {
            SetPartSprite(part.Key, part.Value);
        }

        NPCLookPart.ColorPalette palette = m_lookPartData.ColorPalettes[m_cardData.ColorPalleteIndex];

        // 각 부위별로 다른 색상 팔레트 적용
        ApplyPartColor(CardData.NPCLookPartType.Top, palette.TopColors);
        ApplyPartColor(CardData.NPCLookPartType.Face, palette.FacesColors);
        ApplyPartColor(CardData.NPCLookPartType.FrontHair, palette.FrontHairsColors);
        ApplyPartColor(CardData.NPCLookPartType.BackHair, palette.BackHairsColors);
        ApplyPartColor(CardData.NPCLookPartType.Eye, palette.EyesColors);
        ApplyPartColor(CardData.NPCLookPartType.Mouth, palette.MouthsColors);
        ApplyPartColor(CardData.NPCLookPartType.Glasses, palette.GlassesColors);
        ApplyPartColor(CardData.NPCLookPartType.Cap, palette.CapsColors);
    }


    /// <summary>
    /// 특정 부위의 스프라이트를 설정합니다.
    /// </summary>
    /// <param name="partType">부위 타입</param>
    /// <param name="spriteIndex">스프라이트 인덱스</param>
    public void SetPartSprite(CardData.NPCLookPartType partType, int spriteIndex)
    {
        Image targetImage = GetImageByPartType(partType);
        Sprite[] spriteArray = GetSpriteArrayByPartType(partType);

        if (targetImage != null && spriteArray != null && spriteIndex >= 0 && spriteIndex < spriteArray.Length)
        {
            targetImage.sprite = spriteArray[spriteIndex];
        }
    }

    /// <summary>
    /// 특정 부위의 색상만 변경합니다.
    /// </summary>
    /// <param name="partType">부위 타입</param>
    /// <param name="colors">적용할 색상</param>
    public void ApplyPartColor(CardData.NPCLookPartType partType, NPCLookPart.ColorPalette.LookPartColors colors)
    {
        Image targetImage = GetImageByPartType(partType);
        
        if (targetImage != null && targetImage.material != null)
        {
            Material mat = targetImage.material;
            mat.SetColor("_ColorR", colors.RedRegionColor);
            mat.SetColor("_ColorG", colors.GreenRegionColor);
            mat.SetColor("_ColorB", colors.BlueRegionColor);
            mat.SetColor("_ColorRG", colors.RedGreenRegionColor);
            mat.SetColor("_ColorRB", colors.RedBlueRegionColor);
            mat.SetColor("_ColorGB", colors.GreenBlueRegionColor);
            mat.SetColor("_ColorRGB", colors.RedGreenBlueRegionColor);
        }
    }

    /// <summary>
    /// 부위 타입에 따라 해당하는 Image를 반환합니다.
    /// </summary>
    private Image GetImageByPartType(CardData.NPCLookPartType partType)
    {
        switch (partType)
        {
            case CardData.NPCLookPartType.Top: 
                return topImage;
            case CardData.NPCLookPartType.Face: 
                return faceImage;
            case CardData.NPCLookPartType.FrontHair: 
                return frontHairImage;
            case CardData.NPCLookPartType.BackHair: 
                return backHairImage;
            case CardData.NPCLookPartType.Eye: 
                return eyeImage;
            case CardData.NPCLookPartType.Mouth: 
                return mouthImage;
            case CardData.NPCLookPartType.Glasses: 
                return glassesImage;
            case CardData.NPCLookPartType.Cap: 
                return capImage;
            default: return null;
        }
    }

    /// <summary>
    /// 부위 타입에 따라 해당하는 스프라이트 배열을 반환합니다.
    /// </summary>
    private Sprite[] GetSpriteArrayByPartType(CardData.NPCLookPartType partType)
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
}
