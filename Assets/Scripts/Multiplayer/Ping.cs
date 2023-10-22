using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Scripts.Managers.UI {
	public class Ping : MonoBehaviour
	{
        [SerializeField] private TMPro.TMP_Text _text;

        private void Start()
        {
            InvokeRepeating(nameof(GetPing), 0f, 0.1f);
        }

        public void GetPing()
        {
            _text.text = PhotonNetwork.GetPing().ToString() + " ms";
        }
    }
}
