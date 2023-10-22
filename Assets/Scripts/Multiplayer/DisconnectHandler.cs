using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace FireGames.Managers
{
    public class DisconnectHandler : MonoBehaviourPunCallbacks
    {

        private void OnApplicationQuit()
        {
            if (!PhotonNetwork.IsMessageQueueRunning)
            {
                PhotonNetwork.IsMessageQueueRunning = true;
            }
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            PhotonNetwork.OpCleanActorRpcBuffer(PhotonNetwork.LocalPlayer.ActorNumber);

            if (!PhotonNetwork.ReconnectAndRejoin())
            {
                if (CanRecoverFromDisconnect(cause))
                {
                    Recover();
                }
            }
        }

        public void LeaveRoom()
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            if (!PhotonNetwork.IsMessageQueueRunning)
            {
                PhotonNetwork.IsMessageQueueRunning = true;
            }
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        private bool CanRecoverFromDisconnect(DisconnectCause cause)
        {
            switch (cause)
            {
                // the list here may be non exhaustive and is subject to review
                case DisconnectCause.Exception:
                case DisconnectCause.ServerTimeout:
                case DisconnectCause.ClientTimeout:
                case DisconnectCause.DisconnectByServerLogic:
                case DisconnectCause.DisconnectByServerReasonUnknown:
                    return true;
            }
            return false;
        }

        private void Recover()
        {
            if (!PhotonNetwork.ReconnectAndRejoin())
            {
                if (!PhotonNetwork.Reconnect())
                {
                    if (!PhotonNetwork.ConnectUsingSettings())
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                    }
                }
            }
        }

        public void BackToTheLobby()
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.CurrentRoom.IsOpen = true;
                PhotonNetwork.IsMessageQueueRunning = false;
                PhotonNetwork.OpRemoveCompleteCache();
                PhotonNetwork.RemoveBufferedRPCs();
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene("Multiplayer");
        }
    }
}
