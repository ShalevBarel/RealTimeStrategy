using Mirror;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject bulletPrefab = null;
    [SerializeField] private Transform bulletSpawnPoint = null;
    [SerializeField] private float fireRange = 5f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float rotationSpeed = 20f;

    private Targetable target;
    private float lastFireTime;

    private void Update() // "update" method is being called every frame. this is where we do our constant checking if the player has a target that can be shot at
    {
        target = targeter.GetTarget();

        if(target == null) { return; }

        if (!CanFireAtTarget()) { return; }

        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);              //
                                                                                                                          // checking if needs to be rotated towards enemy
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);//

        if(Time.time > (1/fireRate) + lastFireTime)
        {
            Quaternion bulletRotation = Quaternion.LookRotation(target.GetAimAtPoint().position - bulletSpawnPoint.position);   //
                                                                                                                                //
            GameObject bulletInstance = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletRotation);                   //
                                                                                                                                //shooting 
            NetworkServer.Spawn(bulletInstance, connectionToClient);                                                            //
                                                                                                                                //
            lastFireTime = Time.time;                                                                                           //
        }
    }

    private bool CanFireAtTarget() // checking if at firing range
    {
        return Vector3.Distance(transform.position, target.transform.position) <= fireRange;
    }
}
