using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public WeaponRecoil recoil;

    public Transform raycastOrigin, raycastDestination;
    public Transform fpsCameraTransform;

    public Transform bulletSpawnPoint, casingSpawnPoint;

    public ParticleSystem hitEffectPrefab;
    public ParticleSystem muzzleFlash;

    public AnimationClip weaponAnimation;

    public float range = 300f;

    WeaponStats weaponStats;
    RaycastHit hit;
    int layerMask;

    [Header("Weapon Sway")]
    //Enables weapon sway
    public bool weaponSway;
    public float swayAmount = 0.02f;
    public float maxSwayAmount = 0.06f;
    public float swaySmoothValue = 4.0f;

    private Vector3 initialSwayPosition;

    private void Start()
    {
        recoil = GetComponent<WeaponRecoil>();
        weaponStats = GetComponent<WeaponPickup>().weaponStats;
        fpsCameraTransform = Camera.main.transform;
        initialSwayPosition = transform.localPosition;
        layerMask = ~((1 << LayerMask.NameToLayer("Ignore Raycast")) << LayerMask.NameToLayer("Only Player"));

        hitEffectPrefab = Instantiate(weaponStats.hitEffectPrefab, transform);
        hitEffectPrefab.gameObject.layer = LayerMask.NameToLayer("Default");
    }

    public void StartFiring()
    {
        muzzleFlash.Emit(1);
        recoil.GenerateRecoil();

        //Spawn casing prefab at spawnpoint
        //Instantiate(weaponStats.casingPrefab,
        //    casingSpawnPoint.position,
        //    casingSpawnPoint.rotation);

        //Spawn bullet from bullet spawnpoint
        //Instantiate(bulletPrefab, bulletSpawnPoint.transform.position, Quaternion.LookRotation(fpsCameraTransform.transform.forward));

        //var tracer = Instantiate(weaponStats.bulletTracer, raycastOrigin.position, Quaternion.identity);
        //tracer.AddPosition(bulletSpawnPoint.position);

        if (Physics.Raycast(raycastOrigin.position, fpsCameraTransform.forward, out hit, range, layerMask))
        {
            hitEffectPrefab.transform.position = hit.point;
            hitEffectPrefab.transform.forward = hit.normal;
            hitEffectPrefab.Emit(5);

            PoolingManager.Instance.UseOneHItEffect(hit);
            //tracer.transform.position = hit.point;
            //Debug.Log(hit.point + " " + hit.transform.name);           
        }
        //else tracer.transform.position += fpsCameraTransform.forward * range;
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
