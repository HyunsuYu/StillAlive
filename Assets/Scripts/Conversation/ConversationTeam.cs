using NUnit.Framework;
using UnityEngine;

public class ConversationTeam : MonoBehaviour
{
    [Header("플레이어 진영 설정")]
    [SerializeField] private Transform m_teamFieldAnchor;     // 플레이어 진영 카드 중심 앵커값 
    [SerializeField] private float m_teamCardSpacing = 1.5f;  // 카드 범위 넓이


    public void Init()
    {

    }
}
