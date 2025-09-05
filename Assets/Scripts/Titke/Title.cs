using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    [SerializeField] private GameObject CheckWindow; // Ȯ�� â
    [SerializeField] private TextMeshProUGUI WindowText;

    SaveData saveData = SaveDataBuffer.Instance.Data;
    int SelectButton = 0; // ���õ� ��ư (1: NewGame, 2: LoadGame, 3: Exit)

    void Start()
    {
        CheckWindow.SetActive(false); // ���� �� Ȯ�� â ��Ȱ��ȭ
    }

    public void OnNewGameClicked()
    {
        SelectButton = 1;
        WindowText.text = "���ο� ������ �����Ͻðڽ��ϱ�?";
        CheckWindow.SetActive(true); // Ȯ�� â Ȱ��ȭ
    }

    public void OnLoadGameClicked()
    {
        SelectButton = 2;
        WindowText.text = "����� ������ �ҷ����ðڽ��ϱ�?";
        CheckWindow.SetActive(true); // Ȯ�� â Ȱ��ȭ
    }

    public void OnExitClicked()
    {
        SelectButton = 3;
        WindowText.text = "������ �����Ͻðڽ��ϱ�?";
        CheckWindow.SetActive(true); // Ȯ�� â Ȱ��ȭ
    }

    public void onSettingClicked()
    {
        SceneManager.LoadScene("Setting");
    }

    public void OnConfirmButtonClick() //Ȯ�� ��ư�� ������ ��
    {
        if (SelectButton == 1)
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
