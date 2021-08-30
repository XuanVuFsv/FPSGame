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
    int layerMask;

    private void Awake()
    {
        layerMask = ~(1 << LayerMask.NameToLayer("Ignore Raycast"));
    }

    public void StartFiring()
    {
        muzzleFlash.Emit(1);

        //Spawn casing prefab at spawnpoint
        Instantiate(BulletController.Instance.casingPrefab,
            casingSpawnPoint.position,
            casingSpawnPoint.rotation);

        //Spawn bullet from bullet spawnpoint
        //Instantiate(bulletPrefab, bulletSpawnPoint.transform.position, Quaternion.LookRotation(fpsCameraTransform.transform.forward));

        var tracer = Instantiate(BulletController.Instance.bulletTracer, raycastOrigin.position, Quaternion.identity);
        tracer.AddPosition(bulletSpawnPoint.position);

        if (Physics.Raycast(raycastOrigin.position, fpsCameraTransform.forward, out hit, range, layerMask))
        {
            BulletController.Instance.hitEffectPrefab.transform.position = hit.point;
            BulletController.Instance.hitEffectPrefab.transform.forward = hit.normal;
            BulletController.Instance.hitEffectPrefab.Emit(10);

            tracer.transform.position = hit.point;
            //Debug.Log(hit.point + " " + hit.transform.name);           
        }
        else tracer.transform.position += fpsCameraTransform.forward * range;
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
