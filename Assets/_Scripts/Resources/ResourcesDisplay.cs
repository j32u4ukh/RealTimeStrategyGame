using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using System;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text resources_text = null;
    private RTSPlayer player;

    private void Start()
    {
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        handleClientResourcesUpdated(player.getResources());
        player.onClientResourcesUpdated += handleClientResourcesUpdated;
    }

    private void OnDestroy()
    {
        player.onClientResourcesUpdated -= handleClientResourcesUpdated;
    }

    private void handleClientResourcesUpdated(int resources)
    {
        resources_text.text = $"Resources: {resources}";
    }
}
