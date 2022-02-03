using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject landing_page_panel = null;
    [SerializeField] private TMP_InputField address_input = null;
    [SerializeField] private Button join_button = null;

    private void OnEnable()
    {
        RTSNetworkManager.onClientConnected += handleClientConnected;
        RTSNetworkManager.onClientDisconnected += handleClientDisconnected;
    }

    private void OnDisable()
    {
        RTSNetworkManager.onClientConnected -= handleClientConnected;
        RTSNetworkManager.onClientDisconnected -= handleClientDisconnected;
    }

    public void join() 
    {
        string address = address_input.text;

        NetworkManager.singleton.networkAddress = address;
        NetworkManager.singleton.StartClient();

        join_button.interactable = false;
    }

    private void handleClientConnected()
    {
        join_button.interactable = true;

        gameObject.SetActive(false);
        landing_page_panel.SetActive(false);
    }

    private void handleClientDisconnected()
    {
        join_button.interactable = true;
    }
}
