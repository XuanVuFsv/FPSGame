//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using System.Linq;
//using Photon;
//using TMPro;

//public class MultiPlayerManager : Photon.MonoBehaviour
//{
//    #region variable
//    public PlayFabAuth auth;
//    public GameObject[] enableGameOblects;
//    public GameObject[] disableGameObjects;
//    public GameObject[] disableGameObjectsOnJoinRoom;
//    public GameObject[] enableGameObjectsOnJoinRoom;
//    public GameObject defaultCharacter;
//    public GameObject defaultCamera;
//    //public Text message;

//    public string playerName;
//    public TMP_Text connectState;
//    public string gameVersion;

//    #endregion

//    // Start is called before the first frame update
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }

//    private void FixedUpdate()
//    {
//        connectState.text = PhotonNetwork.connectionStateDetailed.ToString();
//    }

//    public void ConnectToMaster()
//    {
//        PhotonNetwork.ConnectUsingSettings(gameVersion);

//        foreach (GameObject enableGameObject in enableGameOblects)
//        {
//            enableGameObject.SetActive(true);
//        }
//        foreach (GameObject disableGameObject in disableGameObjects)
//        {
//            disableGameObject.SetActive(false);
//        }

//        try
//        {
//            PhotonNetwork.JoinRandomRoom();
//        }
//        catch
//        { }
//    }

//    public virtual void OnConnectedToMaster()
//    {
//        auth.HideLoginUI();
//        //Destroy(message.gameObject);
//    }

//    public void CreateOrJoin()
//    {
//        PhotonNetwork.JoinRandomRoom();
//    }

//    public virtual void OnPhotonRandomJoinFailed()
//    {
//        RoomOptions room = new RoomOptions
//        {
//            MaxPlayers = 5,
//            IsVisible = true
//        };
//        System.Guid roomID = System.Guid.NewGuid();
//        //int roomID = Random.Range(0, 3000);
//        PhotonNetwork.CreateRoom("Default ID: " + roomID.ToString(), room, TypedLobby.Default);
//    }



//    public virtual void OnJoinedRoom()
//    {
//        //defaultCamera.SetActive(true);
//        foreach (GameObject disableGameObject in disableGameObjectsOnJoinRoom)
//        {
//            disableGameObject.SetActive(false);
//        }
//        foreach (GameObject enableGameObject in enableGameObjectsOnJoinRoom)
//        {
//            enableGameObject.SetActive(true);
//        }
//        InstantiateGameObject();
//    }

//    public void InstantiateGameObject()
//    {
//        GameObject player = PhotonNetwork.Instantiate("Cube", new Vector3(250, 25, 200), Quaternion.identity, 0);
//    }


//    public virtual void OnPhotonSerializeView(PhotonStream photonStream, PhotonMessageInfo info)
//    {
//        if (photonStream.isWriting)
//        {
//        }
//        else if (photonStream.isReading)
//        {
//        }
//    }
//}
