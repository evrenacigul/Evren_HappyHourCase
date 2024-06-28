using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

public class PlayFabData : SingletonMonoBehaviour<PlayFabData>
{
    private Dictionary<string, string> lastPlayerData = new();

    public Dictionary<string, string> GetLastPlayerData { get { return lastPlayerData; } }

    public UnityEvent<Dictionary<string, string>> OnDataUpdate;

    bool isValuesUpdated = false;

    public void WriteSingleData(string key, string data)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { key, data }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, null, errorCallback =>
        { Debug.LogError("Error updating player data = " + errorCallback.ErrorMessage); });
    }

    public void ReadPlayerData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnGetResult, errorCallback => 
        { Debug.LogError("Error reading player data = " + errorCallback.ErrorMessage); });
    }

    public string GetValue(string key)
    {
        string value = string.Empty;

        if (!isValuesUpdated)
            return value;
        
        if (lastPlayerData.ContainsKey(key))
            value = lastPlayerData[key];

        isValuesUpdated = false;

        return value;
    }

    void OnGetResult(GetUserDataResult result)
    {
        Debug.Log("result = " + result.ToString());

        if (result.Data == null) return;

        if (lastPlayerData == null) lastPlayerData = new();
        else lastPlayerData.Clear();

        foreach (var data in result.Data)
        {
            lastPlayerData.Add(data.Key, data.Value.Value);
        }

        OnDataUpdate?.Invoke(lastPlayerData);

        isValuesUpdated = true;
    }
}
