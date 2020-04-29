using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class CLobbyManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "1";

    public Button joinButton;

    public InputField nameInputField;
    public static string playerName;

    private void Start()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();

        joinButton.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        joinButton.interactable = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        joinButton.interactable = false;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Connect()
    {
        if(nameInputField.text.Equals(string.Empty))
        {
            return;
        }

        playerName = nameInputField.text;
        joinButton.interactable = false;

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 8 });
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
}