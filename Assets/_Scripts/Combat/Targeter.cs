using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    private Targetable target;

    public Targetable getTarget()
    {
        return target;
    }

    #region Server
    [Command]
    public void cmdSetTarget(GameObject target_obj)
    {
        if (target_obj.TryGetComponent<Targetable>(out Targetable target))
        {
            this.target = target;
        }
    }

    [Server]
    public void clearTarget()
    {
        target = null;
    }
    #endregion

    #region Client

    #endregion
}
