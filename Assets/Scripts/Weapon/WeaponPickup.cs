using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public WeaponStats weaponStats;
    public bool noParent = true;
    public Vector3 viewPortPoint;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && noParent)
        {
            viewPortPoint = Camera.main.WorldToViewportPoint(transform.position);

            if (WeaponInPickupViewPort())
            {
                ShowWeaponStats();
                if (Input.GetKeyDown(KeyCode.H))
                {
                    ActiveWeapon newActiveWeapon = other.GetComponent<ActiveWeapon>();
                    newActiveWeapon.DropWeapon();
                    newActiveWeapon.EquipWeapon(transform);
                    newActiveWeapon.SetupNewWeapon(weaponStats);
                }
            }
        }
    }
    
    bool WeaponInPickupViewPort()
    {       
        if (viewPortPoint.z < 2 && Mathf.Abs(viewPortPoint.x - 0.5f) < 0.375f && Mathf.Abs(viewPortPoint.y - 0.5f) < 0.375f)
        {
            return true;
        }
        return false;
    }

    public void ShowWeaponStats()
    {
        //Debug.Log(weaponStats.name);
    }
}
