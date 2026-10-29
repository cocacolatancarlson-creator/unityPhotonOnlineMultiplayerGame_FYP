using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using Portfolio.Core;

public class Launcher : MonoBehaviourPunCallbacks
{
	public static Launcher Instance;

	// Full room list; Photon only sends changes (see RoomListCache).
	readonly RoomListCache<RoomInfo> roomCache = new RoomListCache<RoomInfo>();

	[SerializeField] TMP_InputField roomNameInputField;
	[SerializeField] TMP_Text errorText;
	[SerializeField] TMP_Text roomNameText;
	[SerializeField] Transform roomListContent;
	[SerializeField] GameObject roomListItemPrefab;
	[SerializeField] Transform playerListContent;
	[SerializeField] GameObject PlayerListItemPrefab;
	[SerializeField] GameObject startGameButton;

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		Debug.Log("Connecting to Master");
		// Restore 2026-10: the Loading panel starts active but its "open" flag is serialized false,
		// so OpenMenu("title") never closed it and "Loading" stayed visible behind the title menu.
		MenuManager.Instance.OpenMenu("loading");
		PhotonNetwork.ConnectUsingSettings();
	}

	public override void OnConnectedToMaster()
	{
		Debug.Log("Connected to Master");
		PhotonNetwork.JoinLobby();
		PhotonNetwork.AutomaticallySyncScene = true;
	}

	public override void OnJoinedLobby()
	{
		roomCache.Clear();
		MenuManager.Instance.OpenMenu("title");
		Debug.Log("Joined Lobby");
		PhotonNetwork.NickName="Player"+ Random.Range(0, 1000).ToString("0000");
	}

	public void CreateRoom()
	{
		if (string.IsNullOrEmpty(roomNameInputField.text))
		{
			return;
		}
		PhotonNetwork.CreateRoom(roomNameInputField.text);
		MenuManager.Instance.OpenMenu("loading");
	}

	public override void OnJoinedRoom()
	{
		MenuManager.Instance.OpenMenu("room");
		roomNameText.text = PhotonNetwork.CurrentRoom.Name;

		Player[] players = PhotonNetwork.PlayerList;
		
		foreach (Transform child in playerListContent)
		{
			Destroy(child.gameObject);
		}

		for (int i = 0; i < players.Count(); i++)
		{
			Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
		}

		startGameButton.SetActive(PhotonNetwork.IsMasterClient);
	}

	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		startGameButton.SetActive(PhotonNetwork.IsMasterClient);
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		errorText.text = "Room Creation Failed: " + message;
		//Debug.LogError("Room Creation Failed: " + message);
		MenuManager.Instance.OpenMenu("error");
	}

	public void StartGame()
	{
		PhotonNetwork.LoadLevel(1);
	}

	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
		MenuManager.Instance.OpenMenu("loading");
	}

	public void JoinRoom(RoomInfo info)
	{
		PhotonNetwork.JoinRoom(info.Name);
		MenuManager.Instance.OpenMenu("loading");
	}

	public override void OnLeftRoom()
	{
		MenuManager.Instance.OpenMenu("title");
	}
	
	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		foreach (RoomInfo info in roomList)
		{
			// Closed, hidden and full rooms can't be joined, so they leave the list too.
			bool gone = info.RemovedFromList || !info.IsOpen || !info.IsVisible
				|| (info.MaxPlayers > 0 && info.PlayerCount >= info.MaxPlayers);
			roomCache.Apply(info.Name, gone, info);
		}

		foreach (Transform trans in roomListContent)
		{
			Destroy(trans.gameObject);
		}

		foreach (RoomInfo info in roomCache.Rooms)
		{
			Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(info);
		}
	}

	public override void OnLeftLobby()
	{
		roomCache.Clear();
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		roomCache.Clear();
		errorText.text = "Disconnected: " + cause;
		MenuManager.Instance.OpenMenu("error");
	}

	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		errorText.text = "Could not join the room: " + message;
		MenuManager.Instance.OpenMenu("error");
	}
	
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
	}
}