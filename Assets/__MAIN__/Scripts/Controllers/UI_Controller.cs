using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Utilities;

namespace Controllers
{
    public class UI_Controller : SingletonMonoBehaviour<UI_Controller>
    {
        [Header("Settings")]
        [SerializeField] GameObject lobbyCanvas;
        [SerializeField] Button quickMatchButton;
        [SerializeField] Button cancelButton;
        [SerializeField] TMP_Text waitForOpponentText;

        private void Start()
        {
            quickMatchButton?.onClick.AddListener(() => WaitForOpponent(true));
        }

        public void InLobby()
        {
            lobbyCanvas?.SetActive(true);
            WaitForOpponent(false);
        }

        public void InGame()
        {
            lobbyCanvas?.SetActive(false);
            WaitForOpponent(false);
        }

        public void WaitForOpponent(bool setActive)
        {
            waitForOpponentText.gameObject?.SetActive(setActive);
            cancelButton.gameObject?.SetActive(setActive);
        }
    }
}