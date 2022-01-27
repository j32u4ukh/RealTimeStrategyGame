using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject projectile_prefab = null;
    [SerializeField] private Transform projectile_spawn_point = null;

    // �����j�� chasing_distance�A�~���|�b chasing_distance �ɰ��U�ӡA�y���û��L�k�i������ϰ줤
    [SerializeField] private float fire_distance = 5f;

    [SerializeField] private float fire_rate = 1f;
    [SerializeField] private float rotation_speed = 20f;

    private Targetable target;
    private float last_fire_time;

    [ServerCallback]
    private void Update()
    {
        if (canFireAtTarget())
        {
            Quaternion quaternion = Quaternion.LookRotation(target.transform.position - transform.position);

            transform.rotation = Quaternion.RotateTowards(from: transform.rotation, 
                                                          to: quaternion, 
                                                          maxDegreesDelta: rotation_speed * Time.deltaTime);

            if(Time.time > last_fire_time + (1f / fire_rate))
            {
                // ������U������
                Quaternion projectile_rotation = Quaternion.LookRotation(
                                                    target.getAimPoint().position - projectile_spawn_point.position);
                GameObject projectile = Instantiate(original: projectile_prefab, 
                                                    position: projectile_spawn_point.position,
                                                    rotation: projectile_rotation);
                NetworkServer.Spawn(projectile, connectionToClient);
                last_fire_time = Time.time;
            }
        }
    }

    [Server]
    private bool canFireAtTarget()
    {
        target = targeter.getTarget();

        // Chasing movement
        if (target != null)
        {
            if ((target.transform.position - transform.position).sqrMagnitude <= fire_distance * fire_distance)
            {
                return true;
            }
        }

        return false;
    }
}
