using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// CardData와 NPCLookPart를 기반으로 캐릭터 초상화 UI를 생성하고 설정하는 헬퍼 클래스
/// </summary>
public static class CharacterPortraitHelper
{
    // Resources 폴더에서 로드할 에셋들의 경로
    private const string PORTRAIT_PREFAB_PATH = "Portrait";

    /// <summary>
    /// CardData와 외형 정보(NPCLookPart)를 기반으로 초상화 UI를 생성 (NPCPortrait 컴포넌트 사용)
    /// </summary>
    /// <param name="cardData">설정할 외형 정보가 담긴 CardData</param>
    /// <param name="lookPartData">외형 스프라이트와 색상 팔레트 정보가 담긴 ScriptableObject</param>
    /// <param name="parent">생성된 초상화가 자식으로 속하게 될 부모 Transform</param>
    /// <returns>완성된 초상화 GameObject (NPCPortrait 컴포넌트 포함)</returns>
    public static GameObject CreatePortrait(CardData cardData, Transform parent = null)
    {
        GameObject portraitPrefab = Resources.Load<GameObject>(PORTRAIT_PREFAB_PATH);
        
        if (portraitPrefab == null)
        {
            Debug.LogError($"CharacterPortraitHelper: Resources에서 '{PORTRAIT_PREFAB_PATH}' 프리팹을 찾을 수 없습니다.");
            return null;
        }

        GameObject portraitInstance = null;
        if (parent != null)
            portraitInstance = Object.Instantiate(portraitPrefab, parent);
        else
            portraitInstance = Object.Instantiate(portraitPrefab);

        if (portraitInstance == null)
        {
            Debug.LogError("초상화 인스턴스 생성에 실패");
            return null;
        }

        // NPCPortrait 컴포넌트를 찾거나 추가
        NPCPortrait npcPortrait = portraitInstance.GetComponent<NPCPortrait>();
        if (npcPortrait == null)
        {
            npcPortrait = portraitInstance.AddComponent<NPCPortrait>();
        }
        // 초상화 설정
        npcPortrait.SetupPortrait(cardData);

        return portraitInstance;
    }

    /// <summary>
    /// NPCPortrait 컴포넌트를 통해 특정 부위의 색상을 변경합니다.
    /// </summary>
    /// <param name="portraitInstance">초상화 GameObject</param>
    /// <param name="partType">부위 타입</param>
    public static void ApplyPartColor(GameObject portraitInstance, CardData.NPCLookPartType partType)
    {
        NPCPortrait npcPortrait = portraitInstance.GetComponent<NPCPortrait>();
        if (npcPortrait != null)
        {
            // NPCLookPart 데이터를 다시 로드하여 색상 적용
            NPCLookPart lookPartData = Resources.Load<NPCLookPart>("NPCLookPart");
            if (lookPartData != null)
            {
                CardData cardData = npcPortrait.GetCardData;
                if (cardData.ColorPalleteIndex >= 0 && cardData.ColorPalleteIndex < lookPartData.ColorPalettes.Length)
                {
                    NPCLookPart.ColorPalette palette = lookPartData.ColorPalettes[cardData.ColorPalleteIndex];
                    NPCLookPart.ColorPalette.LookPartColors colors = GetColorsByPartType(palette, partType);
                    npcPortrait.ApplyPartColor(partType, colors);
                }
            }
        }
    }

    /// <summary>
    /// 부위 타입에 따라 해당하는 색상을 반환합니다.
    /// </summary>
    private static NPCLookPart.ColorPalette.LookPartColors GetColorsByPartType(NPCLookPart.ColorPalette palette, CardData.NPCLookPartType partType)
    {
        switch (partType)
        {
            case CardData.NPCLookPartType.Top: 
                return palette.TopColors;
            case CardData.NPCLookPartType.Face: 
                return palette.FacesColors;
            case CardData.NPCLookPartType.FrontHair: 
                return palette.FrontHairsColors;
            case CardData.NPCLookPartType.BackHair: 
                return palette.BackHairsColors;
            case CardData.NPCLookPartType.Eye: 
                return palette.EyesColors;
            case CardData.NPCLookPartType.Mouth:
                return palette.MouthsColors;
            case CardData.NPCLookPartType.Glasses: 
                return palette.GlassesColors;
            case CardData.NPCLookPartType.Cap:
                return palette.CapsColors;
            default: 
                return palette.TopColors;
        }
    }
}
