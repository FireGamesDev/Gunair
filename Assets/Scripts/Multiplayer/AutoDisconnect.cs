 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Managers.UI {
	public class AutoDisconnect : MonoBehaviour
	{
        [SerializeField] private TMPro.TMP_Text _timerText;
        [SerializeField] private UnityEvent backToMainMenu;

        private void OnEnable()
        {
            StartCoroutine(Countdown());
        }

        private IEnumerator Countdown()
        {
            for (int i = 30; i > 0; i--)
            {
                _timerText.text = "Disconnecting in " + i.ToString();
                yield return new WaitForSeconds(1);
            }

            backToMainMenu.Invoke();
        }
    }
}
