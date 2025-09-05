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
                Money = UnityEngine.Random.Range(0, 31)
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
            m_curSaveData = DefaultSaveData;
            TrySaveData();

            return true;
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

    public bool TrySetData(in SaveData targetSaveData)
    {
        throw new NotImplementedException();
    }

    protected override void Dispose(bool bisDisposing)
    {
        m_curSaveData = null;
    }
}