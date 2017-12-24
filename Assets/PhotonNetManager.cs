using Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonNetManager : PunBehaviour
{
    [SerializeField]
    PhotonView[] players;
    public override void OnJoinedLobby()
    {
        Debug.Log("conected to lobby");
        PhotonNetwork.JoinOrCreateRoom("test", new RoomOptions(), new TypedLobby("test", LobbyType.Default));
    }
    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (!players[i].isMine)
            {
                players[i].TransferOwnership(newPlayer.ID);
            }
        }
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("<b><color=red><size=20>FIGHT!! AND DANCE WITH THE DEVIL</size></color></b>");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].owner == null)
            {
                players[i].ownershipTransfer = OwnershipOption.Request;
                players[i].RequestOwnership();
                break;
            }
        }
    }
    public override void OnLeftRoom()
    {
        Debug.Log("left room");
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("connected to master");
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnDisconnectedFromPhoton()
    {
        Debug.Log("disconnected ");
    }
    public override void OnConnectedToPhoton()
    {
        Debug.Log("connected to cloud");
        PhotonNetwork.ConnectToBestCloudServer(Application.version);
    }

    private void Awake()
    {
        PhotonNetwork.ConnectToRegion(CloudRegionCode.eu, Application.version);

    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}