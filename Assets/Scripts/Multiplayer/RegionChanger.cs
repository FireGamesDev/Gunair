using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FireGames.Managers;

public class RegionChanger : MonoBehaviourPunCallbacks
{
    private bool isFirst = true;
    [SerializeField] private UnityEngine.UI.Dropdown dropdown;
    [SerializeField] private MultiplayerManager multiplayerManager;

    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void ChangeRegion(int regionIndex)
    {
        if (!isFirst)
        {
            multiplayerManager.Disconnect();
        }
        isFirst = false;
        ConnectToRegion(regionIndex);
    }

    public void ConnectToRegion(int regionIndex)
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        PlayerPrefs.SetInt("Region", regionIndex);

        dropdown.value = regionIndex;

        RegionDropdown.isDefault = false;

        switch (regionIndex)
        {
            case 0:
                PhotonNetwork.ConnectToRegion("eu");
                break;
            case 1:
                PhotonNetwork.ConnectToRegion("us");
                break;
            case 2:
                PhotonNetwork.ConnectToRegion("asia");
                break;
            case 3:
                PhotonNetwork.ConnectToRegion("au");
                break;
            default:
                break;
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        if (isFirst)
        {
            ChangeRegion(PlayerPrefs.GetInt("Region"));

        }
        
    }

    public void SetRegion()
    {
        if (PlayerPrefs.GetInt("Region", -1) == -1)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }
}
