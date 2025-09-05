using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    [SerializeField] private GameObject CheckWindow; // Ȯ�� â
    [SerializeField] private TextMeshProUGUI WindowText;

    private int SelectButton = 0; // ���õ� ��ư (1: NewGame, 2: LoadGame, 3: Exit)

    void Awake()
    {
        CheckWindow.SetActive(false); // ���� �� Ȯ�� â ��Ȱ��ȭ
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

    public void OnConfirmButtonClick() //Ȯ�� ��ư�� ������ ��
    {
        if (SelectButton == 1)// New Game 
        {
            // ���ο� ���� ���� ���� �߰�
            Debug.Log("���ο� ���� ����");
            // ��: SceneManager.LoadScene("GameScene");
        }
        else if (SelectButton == 2)
        {
            SaveDataBuffer.Instance.TryLoadData();
        }
        else if (SelectButton == 3)
        {
            // ���� ���� ���� �߰�
            Debug.Log("���� ����");
            Application.Quit();
        }
        CheckWindow.SetActive(false); // Ȯ�� â ��Ȱ��ȭ
        SelectButton = 0; // ���õ� ��ư �ʱ�ȭ
    }

    public void OnCancelButtonClick()  //��� ��ư�� ������ ��
    {
        CheckWindow.SetActive(false); // Ȯ�� â ��Ȱ��ȭ
        SelectButton = 0; // ���õ� ��ư �ʱ�ȭ
    }


}
