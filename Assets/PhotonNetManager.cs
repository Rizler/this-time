using Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonNetManager : PunBehaviour {
    public override void OnJoinedLobby()
    {
        Debug.Log("conected to lobby");
        PhotonNetwork.JoinOrCreateRoom("test", new RoomOptions(), new TypedLobby("test", LobbyType.Default));
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("<b><color><size=20>FIGHT!! AND DANCE WITH THE DEVIL</size></color></b>");
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
        Debug.Log("connected to cloud" );
        PhotonNetwork.ConnectToBestCloudServer(Application.version);
    }

    private void Awake()
    {
        PhotonNetwork.ConnectToRegion(CloudRegionCode.eu, Application.version);

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
