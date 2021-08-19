using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    public GameObject muzzleFlash;
    public Transform raycastOrigin;
    public Transform fpsCameraTransform;
    public float range = 100f;

    RaycastHit hit;

    public void StartFiring()
    {
        muzzleFlash.SetActive(true);

        if (Physics.Raycast(raycastOrigin.position, fpsCameraTransform.forward, out hit, range, -1))
        {
            Debug.Log(hit.point + " " + hit.transform.name);
            //Instantiate()
        }
    }

    public void StopFiring()
    {
        muzzleFlash.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(raycastOrigin.position, fpsCameraTransform.forward);
    }
}
