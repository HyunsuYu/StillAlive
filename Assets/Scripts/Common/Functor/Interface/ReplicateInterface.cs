using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;


public sealed class ReplicateInterface : MonoBehaviour
{
    private struct ReplicateOutput
    {
        public string completed_at;
        public string created_at;
        public bool data_removed;
        public string id;
        public object input;
        public string logs;
        public object metrics;
        public string[] output;
        public string started_at;
        public string status;
        public object urls;
        public string version;
    }


    private bool m_bisResponsed = false;
    private string m_llmResponse = string.Empty;

    private string m_systemPrompt = "너는 한 명의 등장인물이 되어서 주어진 프롬프트의 설정에 따라 대사를 출력해야만 해. 우선, 세계관은 아포칼립스 이후의 세계관이먀, 변형된 감염체들이 동료 사이에 섞여있을 수 있어. 감염체는 겉보기에는 일반적인 인간과 차이가 나지 않아 동료 중 한 명으로 숨어들어와있을 수 있는 거지.";

    internal void TryGetSpeakText(in CardData targetCardData)
    {
        m_bisResponsed = false;
        StartCoroutine(CallLLM(targetCardData));
    }

    private IEnumerator CallLLM(CardData targetCardData)
    {
        string prompt = string.Empty;
        prompt += $"당신의 역할 : {(targetCardData.BIsTraitor ? "감염체" : "일반적인 인간")}\n";
        prompt += $"당신의 성격 : {targetCardData.Personality.ToString()}\n";
        prompt += $"당신의 몸상태 : {targetCardData.Diseases.ToString()}\n";
        prompt += $"당신의 지난 밤 행적 : ";

        CardData? attacher = null;
        List<CardData> aliveCards = SaveDataInterface.GetAliveCardInfos();
        foreach(CardData cardData in aliveCards)
        {
            if(cardData.BIsTraitor)
            {
                attacher = cardData;
                break;
            }
        }

        switch (targetCardData.LastNight)
        {
            case CardData.LastNightState.Peace:
                prompt += $"당신은 지난 밤 어떤 일이 일어났는지 전혀 목격하지 못했습니다. 따라서 본인의 몸 상태에 특이점이 있다면 그것에 대해 말하거나 혹은 그냥 본인의 성격에 맞게끔 하고 싶은 말을 하면 됩니다";
                break;

            case CardData.LastNightState.Attacker:
                prompt += $"당신은 동료로 숨어들어와있는 감염체로서 정체를 들켜선 안 됩니다. 당신은 아무 것도 목격하지 않은 척 해야만 합니다";
                break;

            case CardData.LastNightState.AttackedPerson:
                //prompt += $"당신은 지난 밤에 공격받은 인간으로서, {attacher.Value.loo}";
                break;

            case CardData.LastNightState.Witness:

                break;
        }

        var input = new
        {
            input = new
            {
                top_p = 1,
                prompt = prompt,
                image_input = new int[0],
                temperature = 1,
                system_prompt = m_systemPrompt,
                presence_penalty = 0,
                frequency_penalty = 0,
                max_completion_tokens = 4096
            }
        };
        string json = JsonConvert.SerializeObject(input);

        using (UnityWebRequest request = UnityWebRequest.Put("https://api.replicate.com/v1/models/openai/gpt-4o/predictions", json))
        {
            request.method = "POST";
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", "Bearer " + "r8_aBiyRX5JEUiP8e6xpIr8c49vpbcIzyJ1QFHGP");
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Prefer", "wait");

            yield return request.SendWebRequest();

            switch (request.result)
            {
                case UnityWebRequest.Result.Success:
                    using (DownloadHandlerBuffer downloadHandler = request.downloadHandler as DownloadHandlerBuffer)
                    {
                        string jsonResponse = downloadHandler.text;
                        //Debug.Log("Response: " + jsonResponse);

                        ReplicateOutput output = JsonConvert.DeserializeObject<ReplicateOutput>(jsonResponse);

                        m_llmResponse = output.output[0];
                        m_bisResponsed = true;
                    }
                    break;

                default:
                    Debug.LogError("Error: " + request.error);
                    Debug.LogError("Response Code: " + request.responseCode);
                    Debug.LogError("Response: " + request.downloadHandler.text);
                    var headers = request.GetResponseHeaders();
                    string log = default;
                    foreach (var header in headers)
                    {
                        log += header.Key + ": " + header.Value + "\n";
                    }
                    Debug.LogError(log);
                    Debug.LogError(request.url);
                    break;
            }
        }
    }
}