using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : NetworkBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private int resources_per_interval = 10;
    [SerializeField] private float interval = 2f;

    private float timer;
    private RTSPlayer player;

    public override void OnStartServer()
    {
        timer = interval;
        player = connectionToClient.identity.GetComponent<RTSPlayer>();

        health.onServerDie += handleServerDie;
        GameOverHandler.onServerGameOver += handleServerGameOver;
    }

    [ServerCallback]
    private void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            player.setResources(resources: player.getResources() + resources_per_interval);
            timer += interval;
        }
    }

    public override void OnStopServer()
    {
        health.onServerDie -= handleServerDie;
        GameOverHandler.onServerGameOver -= handleServerGameOver;
    }

    private void handleServerDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    private void handleServerGameOver()
    {
        enabled = false;
    }
}
