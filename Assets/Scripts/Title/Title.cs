using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [SerializeField] private GameObject CheckWindow; // Ȯ�� â
    [SerializeField] private TMP_Text WindowText;
    [SerializeField] private Button LoadButton;

    private int SelectButton = 0; // ���õ� ��ư (1: NewGame, 2: LoadGame, 3: Exit)

    void Awake()
    {
        CheckWindow.SetActive(false); // ���� �� Ȯ�� â ��Ȱ��ȭ

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
            WindowText.text = "���ο� ������ �����ϸ� ���� ���� �����Ͱ� �����˴ϴ�.\n����Ͻðڽ��ϱ�?";
            CheckWindow.SetActive(true); // Ȯ�� â Ȱ��ȭ
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
        Debug.Log("���� ����");
        Application.Quit();
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
            SaveDataBuffer.Instance.ClearSaveData();
            SceneManager.LoadScene("Map");
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
