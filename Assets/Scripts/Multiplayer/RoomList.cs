using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

namespace FireGames.Managers
{
    public class RoomList : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject content;
        [SerializeField] private GameObject roomPrefab;
        [SerializeField] private GameObject _noRooms;
        [SerializeField] private TMP_Text _waitingText;
        private List<RoomInfo> roomList; 

        private void ClearRoomList()
        {
            if (content)
            {
                Transform contents = content.transform;
                foreach (Transform go in contents) Destroy(go.gameObject);
            }
        }

        public override void OnRoomListUpdate(List<RoomInfo> p_List)
        {
            roomList = p_List;
            ClearRoomList();

            if (!content) return;

            Transform contents = content.transform;

            foreach (RoomInfo room in roomList)
            {
                RoomPrefab newRoomButton = Instantiate(roomPrefab, contents).GetComponent<RoomPrefab>();
                newRoomButton._name.text = room.Name;
                newRoomButton._players.text = room.PlayerCount + " / " + room.MaxPlayers;
                if (room.IsOpen)
                {
                    newRoomButton._status.text = "Join";
                }
                else newRoomButton._status.text = "Running";

                newRoomButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { JoinRoom(newRoomButton); SetWaitingText("Joining", true); });
            }

            _noRooms.SetActive(contents.childCount == 0);

            base.OnRoomListUpdate(roomList);
        }

        private void JoinRoom(RoomPrefab room)
        {
            string roomName = room._name.text;
            PhotonNetwork.JoinRoom(roomName);
        }

        public void SetWaitingText(string text, bool isActive)
        {
            _waitingText.text = text;
            _waitingText.gameObject.SetActive(isActive);
            _noRooms.SetActive(!isActive);
            content.SetActive(!isActive);
        }

        public void DisplayRooms(bool toShow)
        {
            content.SetActive(toShow);
        }
    }
}
