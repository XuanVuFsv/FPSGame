using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public WeaponStats weaponStats;
    public bool noParent = true;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && noParent)
        {
            ShowWeaponStats();
            if (Input.GetKeyDown(KeyCode.H) || !ActiveWeapon.Instance.isHoldWeapon)
            {
                ActiveWeapon newActiveWeapon = other.GetComponent<ActiveWeapon>();
                newActiveWeapon.DropWeapon();
                newActiveWeapon.EquipWeapon(transform);
                newActiveWeapon.SetupNewWeapon(weaponStats);
            }
        }
    }
    
    public void ShowWeaponStats()
    {
        //Debug.Log(weaponStats.name);
    }
}
