using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private Unit unit_prefab = null;
    [SerializeField] private Transform spawn_point = null;
    [SerializeField] private TMP_Text remaining_units_text = null;
    [SerializeField] private Image unit_progress_image = null;
    [SerializeField] private int max_unit_queue = 5;
    [SerializeField] private float spawn_move_range = 7f;
    [SerializeField] private float unit_spawn_duration = 5f;

    [SyncVar(hook = nameof(handleClientQueueUnitsUpdated))] private int queue_units;
    [SyncVar] private float unit_timer;

    private float progress_velocity;

    private void Update()
    {
        /* 若是 host(server + client) 則兩者都執行，因此不使用 else if */

        if (isServer)
        {
            produceUnits();
        }

        if (isClient)
        {
            updateTimerDisplay();
        }
    }

    #region Server
    public override void OnStartServer()
    {
        health.onServerDie += handleServerDie;
    }

    public override void OnStopServer()
    {
        health.onServerDie -= handleServerDie;
    }

    [Server]
    private void handleServerDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Server]
    private void produceUnits()
    {
        if(queue_units == 0)
        {
            return;
        }

        unit_timer += Time.deltaTime;

        if(unit_timer < unit_spawn_duration)
        {
            return;
        }

        // queue_units > 0 && unit_timer >= unit_spawn_duration

        GameObject unit = Instantiate(unit_prefab.gameObject,
                                      spawn_point.position,
                                      spawn_point.rotation);

        // connectionToClient: 當前連線的 Client
        NetworkServer.Spawn(unit, connectionToClient);

        Vector3 spawn_offset = Random.insideUnitSphere * spawn_move_range;
        //spawn_offset.y = spawn_point.position.y;
        spawn_offset.y = 0f;

        UnitMovement movement = unit.GetComponent<UnitMovement>();
        movement.serverMove(spawn_point.position + spawn_offset);

        queue_units--;
        unit_timer = 0f; 
    }

    [Command]
    private void cmdSpawnUnit()
    {
        if(queue_units == max_unit_queue)
        {
            return;
        }

        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();
        int resources = player.getResources();

        if(resources < unit_prefab.getResourceCost())
        {
            return;
        }

        queue_units++;
        player.setResources(resources - unit_prefab.getResourceCost());
    }
    #endregion

    #region Client
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button.Equals(PointerEventData.InputButton.Left))
        {
            if (!hasAuthority)
            {
                return;
            }

            cmdSpawnUnit();
        }
    }

    private void handleClientQueueUnitsUpdated(int origin_number, int new_number)
    {
        remaining_units_text.text = new_number.ToString();
    }

    private void updateTimerDisplay()
    {
        float progress = unit_timer / unit_spawn_duration;
        
        if(progress < unit_progress_image.fillAmount)
        {
            unit_progress_image.fillAmount = progress;
        }
        else
        {
            unit_progress_image.fillAmount = Mathf.SmoothDamp(
                unit_progress_image.fillAmount,
                progress,
                ref progress_velocity,
                0.1f
            );
        }
    }
    #endregion
}
