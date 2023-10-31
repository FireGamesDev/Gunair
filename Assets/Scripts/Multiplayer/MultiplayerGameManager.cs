using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private ClayWarsScoreCounter scoreCounter;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private TMPro.TMP_Text winnerScoreText;
    [SerializeField] private TMPro.TMP_Text endGameText;

    public static int playerCount = 1;
    public static MultiplayerGameManager Instance;

    [Header("End")]
    [SerializeField] private PhotonView _pv;
    [SerializeField] private GameObject waitForHost;
    [SerializeField] private GameObject lobbyBtn;

    public bool isEnded { get; private set; } = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PhotonNetwork.OpCleanActorRpcBuffer(PhotonNetwork.LocalPlayer.ActorNumber);
    }

    private void Update()
    {
        if (isEnded)
        {
            if (!Cursor.visible)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void UpdateScore(int scoresToAdd)
    {
        if (PhotonNetwork.InRoom)
        {
            _pv.RPC(nameof(SynchScoresRPC), RpcTarget.All, scoresToAdd);
        }
        else
        {
            scoreCounter.UpdatePlayerScore(ClayWarsDiscSpawner.Instance.currentPlayerIndexToGiveScoreTo, scoresToAdd);
        }
            
    }

    [PunRPC]
    private void SynchScoresRPC(int scoresToAdd)
    {
        scoreCounter.UpdatePlayerScore(ClayWarsDiscSpawner.Instance.currentPlayerIndexToGiveScoreTo, scoresToAdd);
    }

    private IEnumerator ScoreDisplayRoutine()
    {
        for (int i = 0; i < playerCount; i++)
        {
            int scoresToAdd = (int)scoreCounter.GetAccuracyOfPlayer(i);

            if (PhotonNetwork.InRoom)
            {
                if (GetLocalPlayerIndex() == i)
                {
                    _pv.RPC(nameof(SnychExtraScores), RpcTarget.All, i, scoresToAdd);
                }
            }
            else
            {
                scoreCounter.EndExtraScore(i, scoresToAdd);
            }

            yield return new WaitForSeconds(3);
        }

        yield return new WaitForSeconds(1);

        DisplayWinner();
    }

    public static int GetLocalPlayerIndex()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].IsLocal)
            {
                return i;
            }
        }

        return -1;
    }

    [PunRPC]
    private void SnychExtraScores(int i, int scoresToAdd)
    {
        scoreCounter.EndExtraScore(i, scoresToAdd);
    }

    private void DisplayWinner()
    {
        int winnerScore = ClayWarsScoreCounter.Instance.GetHighestScore();
        string winnerName = ClayWarsScoreCounter.Instance.GetPlayerWithHighestScore();
        winnerScoreText.text = winnerScore.ToString();

        if (playerCount > 1)
        {
            endGameText.text = "The winner is: " + winnerName;
        }

        if (PhotonNetwork.LocalPlayer.NickName == winnerName)
        {
            PlayfabLeaderboard.SubmitScore(winnerName, winnerScore, true);
        }
    }

    public void LeaveRoom()
    {
        MenuManager.isClaywarsActive = true;
        MenuManager.isMinigamesActive = false;

        SceneManager.LoadScene("Menu");

        PhotonNetwork.OpCleanActorRpcBuffer(PhotonNetwork.LocalPlayer.ActorNumber);
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Menu");
    }

    [PunRPC]
    private void EnableLobby()
    {
        waitForHost.SetActive(false);
        lobbyBtn.SetActive(true);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.PlayerList.Length == 1)
        {
            SceneManager.LoadScene("Lobby");
            PhotonNetwork.SetMasterClient(PhotonNetwork.PlayerList[0]);
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }
        else
        {
            if (!PhotonNetwork.PlayerList[0].IsMasterClient)
            {
                PhotonNetwork.SetMasterClient(PhotonNetwork.PlayerList[0]);
            }
        }
    }

    public void Lobby()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _pv.RPC(nameof(EnableLobby), RpcTarget.Others);
        }

        SceneManager.LoadScene("Lobby");

        PhotonNetwork.CurrentRoom.IsOpen = true;
    }


    public void EndGame()
    {
        _pv.RPC(nameof(EndGameRPC), RpcTarget.All);
    }

    [PunRPC]
    private void EndGameRPC()
    {
        if (!isEnded)
        {
            isEnded = true;

            endScreen.SetActive(true);

            ClayWarsScoreCounter.Instance.parent.gameObject.SetActive(false);

            if (PhotonNetwork.IsMasterClient)
            {
                lobbyBtn.SetActive(true);
            }
            else
            {
                waitForHost.SetActive(true);
            }

            StartCoroutine(ScoreDisplayRoutine());
        }
    }
}
