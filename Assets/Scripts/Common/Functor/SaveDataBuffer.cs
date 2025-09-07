using System;
using System.IO;
using System.Collections.Generic;

using UnityEngine;

using CommonUtilLib.ThreadSafe;

using Newtonsoft.Json;


public sealed class SaveDataBuffer : SingleTonForGameObject<SaveDataBuffer>
{
    private SaveData? m_curSaveData;


    public void Awake()
    {
        SetInstance(this);

        TryLoadData();
    }

    internal SaveData Data
    {
        get
        {
            if(m_curSaveData == null)
            {
                throw new Exception("Cur Save Data is Empty");
            }

            return m_curSaveData.Value;
        }
    }
    internal bool BHasValue
    {
        get
        {
            return m_curSaveData.HasValue;
        }
    }

    private SaveData DefaultSaveData
    {
        get
        {
            return new SaveData()
            {
                CardDatas = new System.Collections.Generic.List<CardData>(),
                ItemAmountTable = new System.Collections.Generic.Dictionary<int, int>(),
                //MapData = 
                //CurPlayerMapPos = 
                DPlusDay = 0,
                LightActionAppliedDay = -1,
                //Money = UnityEngine.Random.Range(0, 31),
                PassedWays = new List<Vector2Int>(),
                IntelInfos = new SaveData.IntelnfoData()
                {
                    ConversationInfos = new List<SaveData.IntelnfoData.SingleConversationInfo>(),
                    RadioInfos = new List<string>()
                },
                CurPlayerMapPos = new Vector2Int(7, 0),
                LastCombatEnemys = new List<CardData>(),
                MapData = null
            };
        }
    }

    private string SaveDataPath
    {
        get
        {
            return Path.Combine(Application.streamingAssetsPath, "SaveData.json");
        }
    }

    internal bool TryLoadData()
    {
        if (!File.Exists(SaveDataPath))
        {
            return false;
        }

        try
        {
            string jsonData = File.ReadAllText(SaveDataPath);
            m_curSaveData = JsonConvert.DeserializeObject<SaveData>(jsonData);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return false;
        }

        return true;
    }

    internal bool TrySaveData()
    {
        if (m_curSaveData == null)
        {
            Debug.LogError("Cur Save Data is Empty");
            return false;
        }

        try
        {
            string jsonData = JsonConvert.SerializeObject(m_curSaveData);

            string directory = Path.GetDirectoryName(SaveDataPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(SaveDataPath, jsonData);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return false;
        }

        return true;
    }
    internal void ClearSaveData()
    {
        m_curSaveData = DefaultSaveData;
        TrySaveData();
    }

    public bool TrySetData(in SaveData targetSaveData)
    {
        m_curSaveData = targetSaveData;

        return true;
    }

    protected override void Dispose(bool bisDisposing)
    {
        m_curSaveData = null;
    }
}