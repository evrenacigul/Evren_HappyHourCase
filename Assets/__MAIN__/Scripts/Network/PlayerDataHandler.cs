using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utilities;

public class PlayerDataHandler : SingletonMonoBehaviourDestroyable<PlayerDataHandler>
{
    [SerializeField] TMP_Text woodText;
    [SerializeField] string woodCountKeyName = "WoodCount";
    [SerializeField] float saveDelay = 2f;

    PlayFabData playfabData;

    int woodCount = 0;

    bool saveData = false;

    private void Start()
    {
        playfabData = PlayFabData.Instance;

        playfabData.OnDataUpdate.AddListener(OnDataUpdate);

        RefreshWoodUI();

        ReadWoodCount();
    }

    private void OnDestroy()
    {
        saveData = false;
        playfabData.OnDataUpdate.RemoveListener(OnDataUpdate);
    }

    IEnumerator SaveData()
    {
        saveData = true;

        while (saveData)
        {
            yield return new WaitForSeconds(saveDelay);

            playfabData.WriteSingleData(woodCountKeyName, woodCount.ToString());
        }
    }

    public void AddWood(int wood = 1)
    {
        woodCount += wood;

        RefreshWoodUI();
    }

    public void ReadWoodCount()
    {
        playfabData.ReadPlayerData();
    }

    public void RefreshWoodUI()
    {
        woodText.text = "Wood " + woodCount.ToString();
    }

    void OnDataUpdate(Dictionary<string, string> data)
    {
        if (data == null) return;
        if (!data.ContainsKey(woodCountKeyName))
        {
            Debug.LogError("Data has no key " + woodCountKeyName);
            Debug.LogWarning("Data = " + data.ToStringFull());
            return;
        }

        woodCount = int.Parse(data[woodCountKeyName]);

        if(!saveData)
            StartCoroutine(SaveData());

        RefreshWoodUI();
    }
}
