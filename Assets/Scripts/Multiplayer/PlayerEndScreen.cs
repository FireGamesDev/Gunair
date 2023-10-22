using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Managers.UI {
	public class PlayerEndScreen : MonoBehaviour
	{
		[SerializeField] private TMPro.TMP_Text _name;
		[SerializeField] private UnityEngine.UI.Image _image;
		[SerializeField] private GameObject _mvp;

		[SerializeField] private bool canMove = false;

        public string owner { get; private set; }

		public void SetStartingPlayer(string name, Sprite image)
        {
			_name.text = name;
			_image.sprite = image;

            owner = name;
        }

        private void Update()
        {
            if (!canMove) return;
            var pos = transform.localPosition;
            pos.y = Mathf.Sin(Time.time) * 10;
            transform.localPosition = pos;
        }

        public void SetMVP()
        {
            _mvp.SetActive(true);
        }
    }
}
