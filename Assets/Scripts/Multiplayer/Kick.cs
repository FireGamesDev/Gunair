using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace FireGames.Managers
{
    public class Kick : MonoBehaviour
    {
        public void KickOtherPlayer(PlayerListing playerListing)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    if (!player.IsMasterClient && player.NickName.Equals(playerListing._username.text)) PhotonNetwork.CloseConnection(player);
                }

                GameObject.Find("LobbyManager").GetComponent<Lobby>().CheckIfCanStart();
            }
        }
    }
}