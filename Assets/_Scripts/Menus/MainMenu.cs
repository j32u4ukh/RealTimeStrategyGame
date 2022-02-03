using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject landing_page_panel = null;

    public void hostLobby()
    {
        landing_page_panel.SetActive(false);

        NetworkManager.singleton.StartHost();
    }
}
