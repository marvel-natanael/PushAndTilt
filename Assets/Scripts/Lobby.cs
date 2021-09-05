using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
public class Lobby : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject unMenu;
    [SerializeField] private GameObject connectPanel;

    [SerializeField] private TMP_InputField unInput;
    [SerializeField] private TMP_InputField cgInput;
    [SerializeField] private TMP_InputField joinInput;
    [SerializeField] private GameObject startButton;

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Start()
    {
        unMenu.SetActive(true);
        //Screen.SetResolution(1024, 768, false);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public void changeunInput()
    {
        if (unInput.text.Length >= 1)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }
    }

    public void setUname()
    {
        unMenu.SetActive(false);
        connectPanel.SetActive(true);
        PhotonNetwork.NickName = unInput.text;
    }

    public void createGame()
    {
        PhotonNetwork.CreateRoom(cgInput.text, new RoomOptions() { MaxPlayers = 2 }, null);
    }

    public void joinGame()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom(joinInput.text, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Main Scene");
    }
}