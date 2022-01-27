using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : NetworkBehaviour
{
    [SerializeField] private Transform aim_point = null;

    public Transform getAimPoint()
    {
        return aim_point;
    }
}
