using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using System.Collections;
using UnityEngine;
using Utilities;

public class PhotonClient : SingletonPunBehaviour<PhotonClient>
{
    public bool isConnectedAndReady { get; private set; } = false;

    public bool isJoinedRoom { get; private set; } = false;

    public bool isCreatedRoom { get; private set; } = false;

    public bool isOpponentJoinedRoom { get; private set; } = false;

    public bool cancelCreateOrJoin { get; private set; } = false;

    public bool gameStarted { get; private set; } = false;

    public PhotonPlayer opponent { get; private set; }

    [SerializeField] private string queueName = "Queue";

    private string ticketId;
    private string matchId;

    private int roomCount = 0;

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings("1.0");
        StartMatchmaking();

        //InvokeRepeating("PollMatchmakingTicket", 5, 5);
    }

    public void StartMatchmaking()
    {
        var request = new CreateMatchmakingTicketRequest
        {
            Creator = new MatchmakingPlayer
            {
                Entity = new PlayFab.MultiplayerModels.EntityKey
                {
                    Id = PlayFabSettings.staticPlayer.EntityId,
                    Type = PlayFabSettings.staticPlayer.EntityType
                }
            },
            AuthenticationContext = new PlayFabAuthenticationContext
            {
                EntityType = PlayFabSettings.staticPlayer.EntityType,
                EntityId = PlayFabSettings.staticPlayer.EntityId,
                EntityToken = PlayFabSettings.staticPlayer.EntityToken,
            },
            GiveUpAfterSeconds = 60,
            QueueName = queueName
        };

        PlayFabMultiplayerAPI.CreateMatchmakingTicket(request, OnMatchmakingTicketCreated, OnMatchmakingTicketFailed);
    }

    void OnMatchmakingTicketCreated(CreateMatchmakingTicketResult result)
    {
        ticketId = result.TicketId;

        Debug.Log("Succesfully created matchmaking ticket: " + ticketId);

        InvokeRepeating("PollMatchmakingTicket", 5, 5);
    }

    void OnMatchmakingTicketFailed(PlayFabError error)
    {
        Disconnected();

        Debug.LogError("Failed to create matchmaking ticket: " + error.GenerateErrorReport());
    }

    void PollMatchmakingTicket()
    {
        var request = new GetMatchmakingTicketRequest { TicketId = ticketId, QueueName = queueName };
        PlayFabMultiplayerAPI.GetMatchmakingTicket(request, OnGetMatchmakingTicketSuccess, OnMatchmakingTicketFailed);
    }

    void OnGetMatchmakingTicketSuccess(GetMatchmakingTicketResult result)
    {
        if (result.Status == "Matched")
        {
            CancelInvoke("PollMatchmakingTicket");

            matchId = result.MatchId;

            StartCoroutine(JoinLobbyWhenReady());

            //PhotonNetwork.JoinRoom(result.MatchId);
            //gameStarted = true;

            //GameManager.Instance.InGameScene();
        }
    }


    public override void OnConnectedToPhoton()
    {
        Debug.Log("Connected to server.");

        //StartCoroutine(JoinLobbyWhenReady());
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        RoomCreateOrJoinFailed(codeAndMsg.ToStringFull());
    }

    public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        RoomCreateOrJoinFailed(codeAndMsg.ToStringFull());
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined to lobby.");

        isConnectedAndReady = true;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room.");

        isJoinedRoom = true;
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created room.");

        isCreatedRoom = true;
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        RoomCreateOrJoinFailed(codeAndMsg.ToStringFull());
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log("Opponent is joined " + newPlayer.ID);

        opponent = newPlayer;

        isOpponentJoinedRoom = true;
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        CancelJoinOrCreate();
    }

    public override void OnDisconnectedFromPhoton()
    {
        Debug.Log("Disconnected");

        Disconnected();
    }

    public override void OnConnectionFail(DisconnectCause cause)
    {
        Debug.Log("Connection failed " + cause.ToString());

        Disconnected();
    }

    public void CancelJoinOrCreate()
    {
        cancelCreateOrJoin = true;
        roomCount = 0;

        if (PhotonNetwork.connected)
        {
            PhotonNetwork.Disconnect();
            Disconnected();
        }
    }

    internal void Disconnected()
    {
        isConnectedAndReady = false;
        isJoinedRoom = false;
        isCreatedRoom = false;
        cancelCreateOrJoin = false;
        isOpponentJoinedRoom = false;

        GameManager.Instance.SetGameState(GameStates.InLobby);
    }

    internal void RoomCreateOrJoinFailed(string failMsg)
    {
        if (cancelCreateOrJoin) return;

        Debug.Log("Room Create or Join failed " + failMsg);

        //roomCount++;

        QuickMatch(createRoom: true);
    }

    internal void QuickMatch(bool createRoom)
    {
        if (!PhotonNetwork.insideLobby) return;

        //PhotonNetwork.JoinOrCreateRoom(PhotonNetwork.AuthValues.AuthGetParameters)

        //if (createRoom)
        //    PhotonNetwork.JoinOrCreateRoom("room" + roomCount.ToString(), new RoomOptions { MaxPlayers = 2, IsOpen = true, IsVisible = true }, null);
        //else
        //    PhotonNetwork.JoinRandomRoom();
    }

    IEnumerator JoinLobbyWhenReady()
    {
        yield return new WaitUntil(() => PhotonNetwork.connectedAndReady);

        PhotonNetwork.JoinLobby();

        yield return new WaitUntil(() => isConnectedAndReady);

        //QuickMatch(false);

        //yield return new WaitUntil(() => (isJoinedRoom || isCreatedRoom) && (isOpponentJoinedRoom || PhotonNetwork.room.PlayerCount >= 2));

        PhotonNetwork.JoinOrCreateRoom(matchId, new RoomOptions { MaxPlayers = 2, IsOpen = true, IsVisible = true }, null);
        //PhotonNetwork.JoinRoom(matchId);

        yield return new WaitUntil(() => (isJoinedRoom || isCreatedRoom) && (isOpponentJoinedRoom || PhotonNetwork.room.PlayerCount >= 2));

        gameStarted = true;

        GameManager.Instance.InGameScene();

        yield return null;
    }
}
