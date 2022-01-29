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

    private void Update()
    {
        // TODO: Build player object at the scene before current scene to avoid NullReferenceException.
        if (player == null)
        {
            try
            {
                player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

                if (player != null)
                {
                    handleClientResourcesUpdated(player.getResources());
                    player.onClientResourcesUpdated += handleClientResourcesUpdated;
                }
            }
            catch (NullReferenceException)
            {

            }
        }
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
