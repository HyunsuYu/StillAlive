using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    [SerializeField] private GameObject CheckWindow; // 확인 창
    [SerializeField] private TextMeshProUGUI WindowText;

    SaveData saveData = SaveDataBuffer.Instance.Data;
    int SelectButton = 0; // 선택된 버튼 (1: NewGame, 2: LoadGame, 3: Exit)

    void Start()
    {
        CheckWindow.SetActive(false); // 시작 시 확인 창 비활성화
    }

    public void OnNewGameClicked()
    {
        SelectButton = 1;
        WindowText.text = "새로운 게임을 시작하시겠습니까?";
        CheckWindow.SetActive(true); // 확인 창 활성화
    }

    public void OnLoadGameClicked()
    {
        SelectButton = 2;
        WindowText.text = "저장된 게임을 불러오시겠습니까?";
        CheckWindow.SetActive(true); // 확인 창 활성화
    }

    public void OnExitClicked()
    {
        SelectButton = 3;
        WindowText.text = "게임을 종료하시겠습니까?";
        CheckWindow.SetActive(true); // 확인 창 활성화
    }

    public void onSettingClicked()
    {
        SceneManager.LoadScene("Setting");
    }

    public void OnConfirmButtonClick() //확인 버튼을 눌렀을 때
    {
        if (SelectButton == 1)
        {
            // 새로운 게임 시작 로직 추가
            Debug.Log("새로운 게임 시작");
            // 예: SceneManager.LoadScene("GameScene");
        }
        else if (SelectButton == 2)
        {
            SaveDataBuffer.Instance.TryLoadData();
        }
        else if (SelectButton == 3)
        {
            // 게임 종료 로직 추가
            Debug.Log("게임 종료");
            Application.Quit();
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
