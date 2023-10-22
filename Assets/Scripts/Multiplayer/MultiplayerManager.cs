using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

namespace FireGames.Managers
{
    public class MultiplayerManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject _loadingScreen;
        [SerializeField] private GameObject _content;
        //[SerializeField] private GameObject _nicknameScreen;
        [SerializeField] private GameObject _createButton;
        [SerializeField] private GameObject _joinButton;
        [SerializeField] private GameObject _privacyToggleX;

        [SerializeField] private RegionChanger regionChanger;

        [SerializeField] private RoomList _roomList;

        [SerializeField] private TMP_InputField _gameNameInput;
        [SerializeField] private TMP_Text _errorText;

        private bool isRoomPrivate = false;
        private int maxPlayers = 6;

        private Coroutine errorMsgRoutine;

        private void Start()
        {
            //PhotonNetwork.ConnectUsingSettings();

            if (PlayerPrefs.GetString("Nickname", "").Equals(""))
            {
                //_nicknameScreen.SetActive(true);
            }

            DisconnectIfConnectedAtStart();

            if (!PhotonNetwork.IsConnected)
            {
                regionChanger.SetRegion();
            }
        }

        private void OnApplicationQuit()
        {
            PhotonNetwork.Disconnect();
        }

        private void DisconnectIfConnectedAtStart()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }
            if (!PhotonNetwork.IsMessageQueueRunning)
            {
                PhotonNetwork.IsMessageQueueRunning = true;
            }
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby(TypedLobby.Default);
            Debug.Log("Connected");

            _roomList.DisplayRooms(true);

            _loadingScreen.SetActive(false);
            PhotonNetwork.NickName = PlayerPrefs.GetString("Nickname", "Anonymus");
        }

        public void CreateGame()
        {
            /*
            if (!isRoomPrivate)
            {
                if (string.IsNullOrEmpty(_gameNameInput.text))
                {
                    ErrorMessage("Room Creation Failed: ", "Room name is empty!");
                    return;
                }
            }*/

            _roomList.SetWaitingText("Creating", true);

            RoomOptions roomOptions = new RoomOptions()
            {
                IsVisible = !isRoomPrivate,
                IsOpen = true,
                MaxPlayers = (byte)maxPlayers,
                CleanupCacheOnLeave = true,
                BroadcastPropsChangeToAll = true,
                PublishUserId = true
            };

            if (isRoomPrivate)
            {
                string roomName = GenerateRoomName();
                PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);
            }
            else
            {
                string roomName = GenerateRoomName();
                PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);
            }
        }

        private string GenerateRoomName()
        {
            string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            string result = "";
            int charactersLength = characters.Length;
            for (int i = 0; i < 5; i++)
            {
                result += characters[Random.Range(0, charactersLength)];
            }

            return result;
        }

        public void QuickMatch()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        private void JoinSpecialRoom()
        {
            PhotonNetwork.JoinRandomRoom(new ExitGames.Client.Photon.Hashtable(), (byte)maxPlayers);
        }

        public void SetRoomPrivacy(bool isPrivate)
        {
            _privacyToggleX.SetActive(isPrivate);
            isRoomPrivate = isPrivate;
        }

        public void JoinGame()
        {
            if (string.IsNullOrEmpty(_gameNameInput.text))
            {
                ErrorMessage("Failed to Join: ", "Room name is empty!");
                return;
            }
            _roomList.SetWaitingText("Joining", true);
            PhotonNetwork.JoinRoom(_gameNameInput.text.ToUpper());
        }

        public override void OnJoinedRoom()
        {
            PhotonNetwork.IsMessageQueueRunning = false;
            SceneManager.LoadScene("Lobby");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            _gameNameInput.text = "";
            if (message.Equals("Game closed"))
            {
                message = "Game already started";
            }
            ErrorMessage("Failed to Join: ", message);

            _roomList.SetWaitingText("Connecting", false);

            //_joinButton.SetActive(true);
        } 
        
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            CreateGame();

            _gameNameInput.text = "";
            if (message.Equals("Game closed"))
            {
                message = "Game already started";
            }
            ErrorMessage("Failed to Join: ", message);

            _roomList.SetWaitingText("Connecting", false);

            //_joinButton.SetActive(true);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            _gameNameInput.text = "";
            ErrorMessage("Room Creation Failed: ", message);

            _roomList.SetWaitingText("Connecting", false);

            _createButton.SetActive(true);
        }

        private void ErrorMessage(string errorType, string message)
        {
            _errorText.text = errorType + message;
            _errorText.gameObject.SetActive(true);

            if (errorMsgRoutine != null) StopCoroutine(errorMsgRoutine);
            errorMsgRoutine = StartCoroutine(HideErrorMessage());
        }

        IEnumerator HideErrorMessage()
        {
            yield return new WaitForSeconds(5f);
            _errorText.gameObject.SetActive(false);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            if (!PhotonNetwork.IsConnected)
            {
                regionChanger.SetRegion();
            }
        }

        public void Disconnect()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();

                _createButton.SetActive(false);
                _joinButton.SetActive(false);

                _loadingScreen.SetActive(true);

                _errorText.gameObject.SetActive(false);

                DeleteRoomDisplays();
            }

        }

        private void DeleteRoomDisplays()
        {
            foreach (Transform room in _content.transform)
            {
                Destroy(room.gameObject);
            }
        }
    }
}
