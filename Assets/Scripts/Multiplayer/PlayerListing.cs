using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace FireGames.Managers
{
    public class PlayerListing : MonoBehaviour, IPunInstantiateMagicCallback
    {
        [SerializeField] private GameObject _readyButton;
        [SerializeField] private GameObject _kickButton;
        [SerializeField] private PhotonView photonView;

        public TMPro.TMP_Text _username;
        public TMPro.TMP_Text _ranking;
        public GameObject _crown;

        public Photon.Realtime.Player Player { get; private set; }
        public bool isReady { get; private set; } = false;

        private Lobby lobby;

        private void Start()
        {
            lobby = GameObject.Find("LobbyManager").GetComponent<Lobby>();
        }

        public void SetPlayerInfo(Photon.Realtime.Player player, string rankName)
        {
            Player = player;
            _username.text = player.NickName;
            _ranking.text = rankName;

            if (photonView.IsMine)
            {
                _readyButton.GetComponent<Button>().interactable = true;
            }

            if (photonView.Owner.IsMasterClient)
            {
                _crown.SetActive(true);
            }

            gameObject.SetActive(true);
        }

        public void ReadyButtonPress()
        {
            photonView.RPC("ButtonPress", RpcTarget.AllBuffered, isReady);
        }

        public void ShowKickButton()
        {
            _kickButton.SetActive(true);
        }

        [PunRPC]
        private void ButtonPress(bool ready)
        {
            isReady = !ready;
            if (isReady)
            {
                _readyButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Ready";
                _readyButton.GetComponentInChildren<TMPro.TMP_Text>().color = Color.white;
                Color _orange = new Color(1.0f, 0.64f, 0.0f);
                _readyButton.GetComponent<Image>().color = _orange;
            }
            else
            {
                _readyButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Not Ready";
                _readyButton.GetComponentInChildren<TMPro.TMP_Text>().color = Color.white;
                _readyButton.GetComponent<Image>().color = Color.black;
            }

            if (lobby == null)
            {
                lobby = GameObject.Find("LobbyManager").GetComponent<Lobby>();
            }
            lobby.CheckIfCanStart();
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            // e.g. store this gameobject as this player's charater in Player.TagObject
            info.Sender.TagObject = gameObject;
        }
    }
}
