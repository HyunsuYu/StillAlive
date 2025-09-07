using TMPro;
using UnityEngine;

public class BattleResult : MonoBehaviour
{
    [SerializeField] private GameObject m_battleResultScreenObj;
    [SerializeField] private TMP_Text m_resultText;

    private void Start()
    {
        if (m_battleResultScreenObj != null)
            m_battleResultScreenObj.SetActive(false);
        else
            Debug.LogError("배틀결과창이 없습니다.");
    }

    public void BattleFinished(string _text)
    {
        if (BattleField.IsBattleEnd)
        {
            m_battleResultScreenObj.gameObject.SetActive(true);
            m_resultText.text = _text;
        }
    }
}
