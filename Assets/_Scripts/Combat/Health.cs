using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int max_health = 100;
    [SyncVar(hook = nameof(handleHealthUpdated))] private int current_health;

    public event Action onServerDie;
    public event Action<int, int> onClientHealthUpdated;

    #region Server
    public override void OnStartServer()
    {
        current_health = max_health;
    }

    [Server]
    public void dealDamage(int damage)
    {
        if(current_health == 0)
        {
            return;
        }

        current_health = Mathf.Max(current_health - damage, 0);

        if (current_health == 0)
        {
            onServerDie?.Invoke();            
        }
    }
    #endregion

    #region Client
    private void handleHealthUpdated(int origin_health, int new_health)
    {
        onClientHealthUpdated?.Invoke(new_health, max_health);
    }
    #endregion
}
