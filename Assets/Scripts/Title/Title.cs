using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [SerializeField] private GameObject CheckWindow; // 확인 창
    [SerializeField] private TMP_Text WindowText;
    [SerializeField] private Button LoadButton;

    private int SelectButton = 0; // 선택된 버튼 (1: NewGame, 2: LoadGame, 3: Exit)

    void Awake()
    {
        CheckWindow.SetActive(false); // 시작 시 확인 창 비활성화

        if (SaveDataBuffer.Instance.BHasValue == false)
        {
            LoadButton.interactable = false;
        }
    }

    public void OnNewGameClicked()
    {
        if (SaveDataBuffer.Instance.BHasValue)
        {
            SelectButton = 1; // New Game
            WindowText.text = "새로운 게임을 시작하면 기존 저장 데이터가 삭제됩니다.\n계속하시겠습니까?";
            CheckWindow.SetActive(true); // 확인 창 활성화
            return;
        }
        else
        {
            SaveDataBuffer.Instance.ClearSaveData();
            SceneManager.LoadScene("Map");
        }

    }

    public void OnLoadGameClicked()
    {
        SaveDataBuffer.Instance.TryLoadData();
        SceneManager.LoadScene("Map");
    }

    public void OnExitClicked()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }

    public void onSettingClicked()
    {
        SceneManager.LoadScene("Setting");
    }

    public void OnConfirmButtonClick() //확인 버튼을 눌렀을 때
    {
        if (SelectButton == 1)// New Game 
        {
            // 새로운 게임 시작 로직 추가
            Debug.Log("새로운 게임 시작");
            SaveDataBuffer.Instance.ClearSaveData();
            SceneManager.LoadScene("Map");
        }
        CheckWindow.SetActive(false); // 확인 창 비활성화
        SelectButton = 0; // 선택된 버튼 초기화
    }

    public void OnCancelButtonClick()  //취소 버튼을 눌렀을 때
    {
        CheckWindow.SetActive(false); // 확인 창 비활성화
        SelectButton = 0; // 선택된 버튼 초기화
    }


}
