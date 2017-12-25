using Photon;
using Prototype02;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PhotonNetManager : PunBehaviour
{
    [SerializeField]
    PhotonView[] players;
    [SerializeField]
    string name;
    public override void OnJoinedLobby()
    {
        Debug.Log("conected to lobby");
        PhotonNetwork.JoinOrCreateRoom("test", new RoomOptions(), new TypedLobby("test", LobbyType.Default));
    }
    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {

    }
    public override void OnJoinedRoom()
    {
        Debug.Log("<b><color=red><size=20>FIGHT!! AND DANCE WITH THE DEVIL</size></color></b>");
        bool hasAvatar=false;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].owner == null&& !hasAvatar)
            {
                players[i].ownershipTransfer = OwnershipOption.Request;
                players[i].RequestOwnership();
                hasAvatar = true;


            }
            else
            {
                //on local pc hopefully not on remote and the updates will still send
                players[i].GetComponent<CharacterController>().enabled = false;
                players[i].GetComponent<Character>().enabled = false;
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
        PhotonNetwork.playerName = name;
        PhotonNetwork.ConnectToBestCloudServer(Application.version);
    }

    private void Awake()
    {
        try
        {
        name = File.ReadAllText(Application.dataPath + "\\name.txt");

        }
        catch { Debug.LogError("name not found in files . fallback to default"); }
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