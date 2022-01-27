using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private int damage_to_deal = 20;
    [SerializeField] private float lunch_force = 10f;
    [SerializeField] private float life_time = 5f;

    void Start()
    {
        rb.velocity = transform.forward * lunch_force;
    }

    public override void OnStartServer()
    {
        Invoke(nameof(destorySelf), life_time);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<NetworkIdentity>(out NetworkIdentity identity))
        {
            if(identity.connectionToClient == connectionToClient)
            {
                return;
            }
        }

        if (other.TryGetComponent<Health>(out Health health))
        {
            health.dealDamage(damage_to_deal);
        }

        destorySelf();
    }

    [Server]
    private void destorySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
