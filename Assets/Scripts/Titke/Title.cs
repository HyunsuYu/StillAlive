using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    [SerializeField] private GameObject CheckWindow; // 확인 창
    [SerializeField] private TextMeshProUGUI WindowText;

    private int SelectButton = 0; // 선택된 버튼 (1: NewGame, 2: LoadGame, 3: Exit)

    void Awake()
    {
        CheckWindow.SetActive(false); // 시작 시 확인 창 비활성화
    }

    public void OnNewGameClicked()
    {
        SaveDataBuffer.Instance.TrySetData = 
    }

    public void OnLoadGameClicked()
    {
       
    }

    public void OnExitClicked()
    {
       
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
