using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using TMPro;
using UnityEngine.Events;

//SIMPLE PHOTON SCRIPT TO HANDLE ROOMS

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public int onlineSceneIndex;
    public int offlineSceneIndex;
    public bool isConnectedToMaster = false;

    public UnityEvent<bool> onConnectToMaster;
    public bool desktopOveride;

    private void OnGUI()
    {
        if (desktopOveride && isConnectedToMaster) 
        {
            if (GUILayout.Button("Connect")) 
            {
                Connect();
            }
        }
    }

    private void Start()
    {
        //Connect to the master server
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnConnectedToMaster()
    {
        isConnectedToMaster = true;
        onConnectToMaster.Invoke(isConnectedToMaster);
        Debug.Log("Connected to master!");
    }

    public void Connect() 
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Failed to join room : {returnCode} , {message}");
        PhotonNetwork.CreateRoom(Random.Range(0, 100000000).ToString(), new RoomOptions { MaxPlayers = 6});
    }

    public override void OnJoinedRoom()
    {      
        Debug.Log("Joined Room");
        Debug.Log(PhotonNetwork.CurrentRoom.Name);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(onlineSceneIndex);
        }
    }

    public override void OnLeftRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(offlineSceneIndex);
        }
    }

    /// <summary>
    /// Makes the button visible
    /// </summary>
    public void OnMasterConnect()
    {
    
    }
}
