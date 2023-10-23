using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

namespace FireGames.Managers
{
    public class Lobby : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject _startGameButton;
        [SerializeField] private GameObject _waitForOtherPlayersMsg;
        [SerializeField] private PlayerListing _playerPrefab;
        [SerializeField] private Transform _players;
        [SerializeField] private TMP_Text _roomName;

        private List<PlayerListing> _listings = new List<PlayerListing>();

        [SerializeField] private PhotonView _photonView;

        private int minPlayersCount = 2;

        private void Awake()
        {
            PhotonNetwork.EnableCloseConnection = true;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            StartCoroutine(WaitToConnectPhoton());
        }

        private IEnumerator WaitToConnectPhoton()
        {
            yield return new WaitForSeconds(0.1f);
            PhotonNetwork.IsMessageQueueRunning = true;
            PhotonNetwork.EnableCloseConnection = true;

            SetRoomName();

            if (PhotonNetwork.LocalPlayer.IsLocal)
            {
                SpawnPlayer();
            }
        }

        private void SetRoomName()
        {
            _roomName.text = "Code: " + PhotonNetwork.CurrentRoom.Name;
            _roomName.gameObject.SetActive(true);
        }

        [PunRPC]
        private void AddPlayerListing(Player player, string rankName)
        {
            GameObject playerObject = (GameObject)player.TagObject;
            PlayerListing listing = playerObject.GetComponent<PlayerListing>();

            if (listing != null)
            {
                listing.SetPlayerInfo(player, rankName);
                _listings.Add(listing);
            }

            playerObject.transform.SetParent(_players);

            playerObject.transform.localScale = Vector3.one;

            if (PhotonNetwork.LocalPlayer.IsMasterClient && !player.IsMasterClient)
            {
                listing.ShowKickButton();
            }

            CheckIfCanStart();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers)
                {
                    PhotonNetwork.CurrentRoom.IsOpen = true;
                }

                _photonView.RPC("RemovePlayer", RpcTarget.All, otherPlayer);

                CheckIfCanStart();
            }
        }

        public void CheckIfCanStart()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            int playerReadyCounter = 0;

            foreach (var listing in _listings)
            {
                if (listing.isReady)
                {
                    playerReadyCounter++;
                }
            }

            if (playerReadyCounter < minPlayersCount)
            {
                _startGameButton.SetActive(false);
                _waitForOtherPlayersMsg.SetActive(true);
                return;
            }

            _startGameButton.SetActive(playerReadyCounter == PhotonNetwork.CurrentRoom.PlayerCount);

            _waitForOtherPlayersMsg.SetActive(playerReadyCounter != PhotonNetwork.CurrentRoom.PlayerCount);
        }

        [PunRPC]
        private void RemovePlayer(Player otherPlayer)
        {
            int index = _listings.FindIndex(x => x.Player == otherPlayer);

            if (index != -1)
            {
                Destroy(_listings[index].gameObject);
                _listings.RemoveAt(index);
            }

            PhotonNetwork.OpCleanActorRpcBuffer(otherPlayer.ActorNumber);
            PhotonNetwork.OpRemoveCompleteCacheOfPlayer(otherPlayer.ActorNumber);

            CheckIfCanStart();
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            SceneManager.LoadScene("Multiplayer");
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer == newMasterClient)
            {
                foreach (var listing in _listings)
                {
                    if (listing.Player == newMasterClient)
                    {
                        listing._crown.SetActive(true);
                    }
                    else
                    {
                        listing.ShowKickButton();
                    }
                }
            }
        }

        public void StartGame()
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            _startGameButton.SetActive(false);
            _photonView.RPC(nameof(LoadMultiplayerScene), RpcTarget.AllViaServer);
        }

        [PunRPC]
        private void LoadMultiplayerScene()
        {
            PhotonNetwork.IsMessageQueueRunning = false;
            ClayWarsGameManager.playerCount = PhotonNetwork.PlayerList.Length;
            PhotonNetwork.LoadLevel(PlayerPrefs.GetString("currentMap", "Desert") + "Multiplayer");
        }

        private void SpawnPlayer()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }

            PhotonNetwork.Instantiate(_playerPrefab.name, transform.position, Quaternion.identity);

            if (PhotonNetwork.LocalPlayer.IsLocal)
            {
                _photonView.RPC(nameof(AddPlayerListing), RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer, EloRankingSystem.rankName);
            }
        }
    }
}
