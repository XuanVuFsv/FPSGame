using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public RectTransform weaponUIPrefab, weaponUI;
    public WeaponStats weaponStats;
    public Vector3 viewPortPoint;
    public bool noParent = true;
    public bool canPickup = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && noParent)
        {
            viewPortPoint = Camera.main.WorldToViewportPoint(transform.position);

            if (WeaponInPickupViewPort())
            {
                canPickup = true;
                ShowWeaponStats();
            }

            if (Input.GetKeyDown(KeyCode.H) && canPickup)
            {
                ActiveWeapon newActiveWeapon = other.GetComponent<ActiveWeapon>();
                newActiveWeapon.DropWeapon();
                newActiveWeapon.EquipWeapon(transform);
                newActiveWeapon.SetupNewWeapon(weaponStats);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && noParent)
        {
            if (weaponUI) weaponUI.gameObject.SetActive(false);
            canPickup = false;
        }
    }

    bool WeaponInPickupViewPort()
    {
        if (viewPortPoint.z < 2.5f && Mathf.Abs(viewPortPoint.x - 0.5f) < 0.4f && Mathf.Abs(viewPortPoint.y - 0.5f) < 0.4f)
        {
            return true;
        }
        return false;
    }

    public void ShowWeaponStats()
    {
        if (weaponUI)
        {
            weaponUI.gameObject.SetActive(true);
            return;
        }
        weaponUI = Instantiate(weaponUIPrefab, transform.parent);
        weaponUI.localScale = CalcualteLocalScale(0.19f, 0.19f, 0.19f, transform.parent.localScale);
        weaponUI.GetChild(0).GetComponent<WeaponUI>().message.text += weaponStats.name;
    }

    Vector3 CalcualteLocalScale(float x, float y, float z, Vector3 parentScale)
    {
        return new Vector3(x / parentScale.x * weaponUI.localScale.x,
                           y / parentScale.y * weaponUI.localScale.y,
                           z / parentScale.z * weaponUI.localScale.z);
    }
}
