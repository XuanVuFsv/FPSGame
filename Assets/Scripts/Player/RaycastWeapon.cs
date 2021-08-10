using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    public GameObject muzzleFlash;
    public Transform raycastOrigin;

    private Ray ray;
    private RaycastHit hitInfo;

    public void StartFiring()
    {
        muzzleFlash.SetActive(true);

        ray.origin = raycastOrigin.position;
        ray.direction = raycastOrigin.forward;

        if (Physics.Raycast(ray, out hitInfo))
        {
            Debug.DrawLine(ray.origin, hitInfo.point, Color.green, 20);
        }
    }

    public void StopFiring()
    {
        muzzleFlash.SetActive(false);
    }
}
