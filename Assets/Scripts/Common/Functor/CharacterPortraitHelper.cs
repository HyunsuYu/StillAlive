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
}