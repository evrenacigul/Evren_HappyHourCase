using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Utilities;
using Controllers;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using PlayFab.CloudScriptModels;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    PhotonClient client;
    UI_Controller ui_controller;

    public TMP_Text launchCount_text;

    public Button quickMatchButton;

    public UnityEvent<GameStates> OnGameStateChanged;
    public GameStates gameState { get; private set; }

    IEnumerator Start()
    {
        if (client == null)
            client = PhotonClient.Instance;

        if(ui_controller == null)
            ui_controller = UI_Controller.Instance;

        SetGameState(GameStates.InLobby);

        yield return new WaitUntil(() => PlayFabClientAPI.IsClientLoggedIn());

        ProcessLaunchCount();
    }

    public void ProcessLaunchCount()
    {
        var request = new ExecuteFunctionRequest()
        {
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
                Id = PlayFabSettings.staticPlayer.EntityId,
                Type = PlayFabSettings.staticPlayer.EntityType,
            },
            FunctionName = "LaunchCount"
        };

        PlayFabCloudScriptAPI.ExecuteFunction(request, OnExecuteSuccess, OnExecuteFailure);
    }

    private void OnExecuteSuccess(ExecuteFunctionResult result)
    {
        launchCount_text.text = "Launch Count is " + result.FunctionResult.ToString();
        launchCount_text.gameObject.SetActive(true);

        quickMatchButton.interactable = true;
    }

    private void OnExecuteFailure(PlayFabError error)
    {
        Debug.Log("Execute function fail");

        quickMatchButton.interactable = false;
    }

    public void SetGameState(GameStates gameState)
    {
        this.gameState = gameState;

        switch(gameState) 
        {
            case GameStates.InLobby:
                LobbyScene();
                break;
            case GameStates.InGame:
                InGameScene();
                break;
        }

        OnGameStateChanged?.Invoke(gameState);
    }

    internal void LobbyScene()
    {
        SceneManager.LoadScene("Lobby");
        ui_controller.InLobby();
    }

    internal void InGameScene()
    {
        if (!client.gameStarted)
        {
            SetGameState(GameStates.InLobby);
            return;
        }

        SceneManager.LoadScene("InGame");
        ui_controller.InGame();
    }
}

public enum GameStates
{
    InLobby,
    InGame
}
