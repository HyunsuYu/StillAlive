using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

using CommonUtilLib.ThreadSafe;


public sealed class ReplicateInterface : SingleTonForGameObject<ReplicateInterface>
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


    [SerializeField] private NPCLookPart m_npcLookPark;

    private bool m_bisRequestProcessing = false;
    private bool m_bisResponsed = false;
    private string m_llmResponse = string.Empty;

    private string m_systemPrompt = "너는 한 명의 등장인물이 되어서 주어진 프롬프트의 설정에 따라 대사를 출력해야만 해. 우선, 세계관은 아포칼립스 이후의 세계관이먀, 변형된 감염체들이 동료 사이에 섞여있을 수 있어. 감염체는 겉보기에는 일반적인 인간과 차이가 나지 않아 동료 중 한 명으로 숨어들어와있을 수 있는 거지.";

    [SerializeField] private UnityEvent m_responseArrived;


    public void Awake()
    {
        SetInstance(this);
    }
    public void FixedUpdate()
    {
        if(m_bisRequestProcessing && m_bisResponsed)
        {
            m_responseArrived.Invoke();
            m_bisRequestProcessing = false;
            m_bisResponsed = false;
        }
    }

    internal string Output
    {
        get
        {
            return m_llmResponse;
        }
    }

    internal void TryGetSpeakText(in CardData targetCardData)
    {
        m_bisRequestProcessing = true;
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

        CardData.NPCLookPartType selectedPart = (CardData.NPCLookPartType)UnityEngine.Random.Range(0, System.Enum.GetNames(typeof(CardData.NPCLookPartType)).Length);

        switch (targetCardData.LastNight)
        {
            case CardData.LastNightState.Peace:
                prompt += $"당신은 지난 밤 어떤 일이 일어났는지 전혀 목격하지 못했습니다. 어쩌면 진짜 아무런 일도 없었을 지도 모릅니다. 따라서 본인의 몸 상태에 특이점이 있다면 그것에 대해 말하거나 혹은 그냥 본인의 성격에 맞게끔 하고 싶은 말을 하면 됩니다";
                break;

            case CardData.LastNightState.Attacker:
                prompt += $"당신은 동료로 숨어들어와있는 감염체로서 정체를 들켜선 안 됩니다. 당신은 아무 것도 목격하지 않은 척 해야만 합니다";
                break;

            case CardData.LastNightState.AttackedPerson:
                prompt += $"당신은 지난 밤에 공격받은 인간으로서, 피의자가 ";
                switch(selectedPart)
                {
                    case CardData.NPCLookPartType.Top:
                        prompt += $"{m_npcLookPark.Tops[attacher.Value.NPCLookTable[CardData.NPCLookPartType.Top]].Description}을(를) 가지고 있는 것을 보았다";
                        break;

                    case CardData.NPCLookPartType.Face:
                        prompt += $"{m_npcLookPark.Faces[attacher.Value.NPCLookTable[CardData.NPCLookPartType.Face]].Description}을(를) 가지고 있는 것을 보았다";
                        break;

                    case CardData.NPCLookPartType.Eye:
                        prompt += $"{m_npcLookPark.Eyes[attacher.Value.NPCLookTable[CardData.NPCLookPartType.Eye]].Description}을(를) 가지고 있는 것을 보았다";
                        break;

                    case CardData.NPCLookPartType.Mouth:
                        prompt += $"{m_npcLookPark.Mouths[attacher.Value.NPCLookTable[CardData.NPCLookPartType.Mouth]].Description}을(를) 가지고 있는 것을 보았다";
                        break;

                    case CardData.NPCLookPartType.Glasses:
                        prompt += $"{m_npcLookPark.Glasses[attacher.Value.NPCLookTable[CardData.NPCLookPartType.Glasses]].Description}을(를) 가지고 있는 것을 보았다";
                        break;

                    case CardData.NPCLookPartType.Cap:
                        prompt += $"{m_npcLookPark.Caps[attacher.Value.NPCLookTable[CardData.NPCLookPartType.Cap]].Description}을(를) 가지고 있는 것을 보았다";
                        break;

                    case CardData.NPCLookPartType.FrontHair:
                        prompt += $"{m_npcLookPark.FrontHairs[attacher.Value.NPCLookTable[CardData.NPCLookPartType.FrontHair]].Description}을(를) 가지고 있는 것을 보았다";
                        break;

                    case CardData.NPCLookPartType.BackHair:
                        prompt += $"{m_npcLookPark.BackHairs[attacher.Value.NPCLookTable[CardData.NPCLookPartType.BackHair]].Description}을(를) 가지고 있는 것을 보았다";
                        break;
                }
                break;

            case CardData.LastNightState.Witness:
                prompt += $"당신은 지난 밤에 누군가가 공격받는 것을 목격한 목격자로서, 피의자가 ";
                switch (selectedPart)
                {
                    case CardData.NPCLookPartType.Top:
                        prompt += $"{m_npcLookPark.Tops[attacher.Value.NPCLookTable[CardData.NPCLookPartType.Top]].Description}을(를) 가지고 있는 것을 보았다";
                        break;

                    case CardData.NPCLookPartType.Face:
                        prompt += $"{m_npcLookPark.Faces[attacher.Value.NPCLookTable[CardData.NPCLookPartType.Face]].Description}을(를) 가지고 있는 것을 보았다";
                        break;

                    case CardData.NPCLookPartType.Eye:
                        prompt += $"{m_npcLookPark.Eyes[attacher.Value.NPCLookTable[CardData.NPCLookPartType.Eye]].Description}을(를) 가지고 있는 것을 보았다";
                        break;

                    case CardData.NPCLookPartType.Mouth:
                        prompt += $"{m_npcLookPark.Mouths[attacher.Value.NPCLookTable[CardData.NPCLookPartType.Mouth]].Description}을(를) 가지고 있는 것을 보았다";
                        break;

                    case CardData.NPCLookPartType.Glasses:
                        prompt += $"{m_npcLookPark.Glasses[attacher.Value.NPCLookTable[CardData.NPCLookPartType.Glasses]].Description}을(를) 가지고 있는 것을 보았다";
                        break;

                    case CardData.NPCLookPartType.Cap:
                        prompt += $"{m_npcLookPark.Caps[attacher.Value.NPCLookTable[CardData.NPCLookPartType.Cap]].Description}을(를) 가지고 있는 것을 보았다";
                        break;

                    case CardData.NPCLookPartType.FrontHair:
                        prompt += $"{m_npcLookPark.FrontHairs[attacher.Value.NPCLookTable[CardData.NPCLookPartType.FrontHair]].Description}을(를) 가지고 있는 것을 보았다";
                        break;

                    case CardData.NPCLookPartType.BackHair:
                        prompt += $"{m_npcLookPark.BackHairs[attacher.Value.NPCLookTable[CardData.NPCLookPartType.BackHair]].Description}을(를) 가지고 있는 것을 보았다";
                        break;
                }
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
            request.SetRequestHeader("Authorization", "Bearer " + "r8_JRKzchgHmJn2sSewlnGKGBGBmWeZHi54Mghvm");
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

                        foreach(string singleOutput in output.output)
                        {
                            m_llmResponse += singleOutput;
                        }
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

    protected override void Dispose(bool bisDisposing)
    {
        throw new System.NotImplementedException();
    }
}