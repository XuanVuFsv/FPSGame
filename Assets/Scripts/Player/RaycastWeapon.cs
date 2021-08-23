using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    public GameObject bulletPrefab;

    public Transform raycastOrigin, raycastDestination;
    public Transform fpsCameraTransform;

    public Transform bulletSpawnPoint, casingSpawnPoint;

    public ParticleSystem muzzleFlash;

    public float range = 300f;

    RaycastHit hit;

    public void StartFiring()
    {
        muzzleFlash.Emit(1);

        //Spawn bullet from bullet spawnpoint
        //Instantiate(bulletPrefab, bulletSpawnPoint.transform.position, Quaternion.LookRotation(fpsCameraTransform.transform.forward));

        //Spawn casing prefab at spawnpoint
        Instantiate(BulletController.Instance.casingPrefab,
            casingSpawnPoint.transform.position,
            casingSpawnPoint.transform.rotation);

        if (Physics.Raycast(raycastOrigin.position, fpsCameraTransform.forward, out hit, range, -1))
        {
            //Debug.Log(hit.point + " " + hit.transform.name);           
        }
    }

    public void StopFiring()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(raycastOrigin.position, fpsCameraTransform.forward);
    }
}
