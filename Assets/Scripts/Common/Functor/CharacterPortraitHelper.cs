using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// CardData와 NPCLookPart를 기반으로 캐릭터 초상화 UI를 생성하고 설정하는 헬퍼 클래스
/// </summary>
public static class CharacterPortraitHelper
{
    private const string PORTRAIT_PREFAB_PATH = "Prefabs/Portrait";

    /// <summary>
    /// CardData를 기반으로 초상화 UI를 생성
    /// </summary>
    public static GameObject CreatePortrait(CardData cardData, Transform parent = null)
    {
        GameObject portraitPrefab = Resources.Load<GameObject>(PORTRAIT_PREFAB_PATH);

        if (portraitPrefab == null)
        {
            Debug.LogError($"CharacterPortraitHelper: Resources에서 '{PORTRAIT_PREFAB_PATH}' 프리팹을 찾을 수 없습니다.");
            return null;
        }

        GameObject portraitInstance = Object.Instantiate(portraitPrefab, parent);

        if (portraitInstance == null)
        {
            Debug.LogError("초상화 인스턴스 생성에 실패");
            return null;
        }

        NPCPortrait npcPortrait = portraitInstance.GetComponent<NPCPortrait>();
        if (npcPortrait == null)
        {
            npcPortrait = portraitInstance.AddComponent<NPCPortrait>();
        }

        npcPortrait.Init(cardData);

        return portraitInstance;
    }

    /// <summary>
    /// NPCPortrait 컴포넌트를 통해 특정 부위의 색상을 변경
    /// </summary>
    public static void ApplyPartColor(GameObject portraitInstance, CardData.NPCLookPartType partType,CardData _cardData)
    {
        NPCPortrait npcPortrait = portraitInstance.GetComponent<NPCPortrait>();
        if (npcPortrait != null)
        {
            NPCLookPart lookPartData = npcPortrait.GetLookPartData;           
            if (lookPartData != null && _cardData.ColorPalleteIndex >= 0 && _cardData.ColorPalleteIndex < lookPartData.ColorPalettes.Length)
            {
                NPCLookPart.ColorPalette palette = lookPartData.ColorPalettes[_cardData.ColorPalleteIndex];
                NPCLookPart.ColorPalette.LookPartColors? colors = GetColorsByPartType(palette, partType);
                if (colors.HasValue)
                {
                    npcPortrait.ApplyPartColor(partType, colors.Value);
                }
            }
        }
    }

    /// <summary>
    /// 부위 타입에 따라 해당하는 색상 구조체를 반환합니다.
    /// </summary>
    private static NPCLookPart.ColorPalette.LookPartColors? GetColorsByPartType(NPCLookPart.ColorPalette palette, CardData.NPCLookPartType partType)
    {
        switch (partType)
        {
            case CardData.NPCLookPartType.Top: return palette.TopColors;
            case CardData.NPCLookPartType.Face: return palette.FacesColors;
            case CardData.NPCLookPartType.FrontHair: return palette.FrontHairsColors;
            case CardData.NPCLookPartType.BackHair: return palette.BackHairsColors;
            case CardData.NPCLookPartType.Eye: return palette.EyesColors;
            case CardData.NPCLookPartType.Mouth: return palette.MouthsColors;
            case CardData.NPCLookPartType.Glasses: return palette.GlassesColors;
            case CardData.NPCLookPartType.Cap: return palette.CapsColors;
            default: return null;
        }
    }

    /// <summary>
    /// 특정 Portrait의 부위 설명을 가져옵니다.
    /// </summary>
    /// <param name="portrait">NPCPortrait 인스턴스</param>
    /// <param name="cardData">카드 데이터</param>
    /// <param name="partType">부위 타입</param>
    /// <returns>해당 부위의 Description</returns>
    public static string GetPartDescription(NPCPortrait portrait, CardData cardData, CardData.NPCLookPartType partType)
    {
        if (portrait == null) return null;
        return portrait.GetPartDescription(cardData, partType);
    }

    /// <summary>
    /// 특정 Portrait의 모든 부위 설명을 가져옵니다.
    /// </summary>
    /// <param name="portrait">NPCPortrait 인스턴스</param>
    /// <param name="cardData">카드 데이터</param>
    /// <returns>부위별 Description Dictionary</returns>
    public static Dictionary<CardData.NPCLookPartType, string> GetAllPartDescriptions(NPCPortrait portrait, CardData cardData)
    {
        if (portrait == null) return new Dictionary<CardData.NPCLookPartType, string>();
        return portrait.GetAllPartDescriptions(cardData);
    }

    /// <summary>
    /// Portrait와 CardData의 매핑 정보를 담는 구조체
    /// </summary>
    [System.Serializable]
    public struct PortraitData
    {
        public NPCPortrait Portrait;
        public CardData CardData;
        public string Name; // 선택적 이름

        public PortraitData(NPCPortrait portrait, CardData cardData, string name = "")
        {
            Portrait = portrait;
            CardData = cardData;
            Name = name;
        }
    }
}