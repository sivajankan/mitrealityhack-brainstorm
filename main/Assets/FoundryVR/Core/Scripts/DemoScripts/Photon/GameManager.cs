using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;

using Foundry;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public Transform[] spawnPoints;
    public GetBiosignals biosignals; 

    // Start is called before the first frame update
    void Start()
    {
        var player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints[Random.Range(0, spawnPoints.Length -1)].position, Quaternion.identity);
        biosignals.onMindfulnessChange.AddListener(player.GetComponent<PlayerMindfullness>().updateMindfulness);
    }
}
