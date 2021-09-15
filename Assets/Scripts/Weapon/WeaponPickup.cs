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

    ActiveWeapon activeWeapon;

    static int standardLengthWeponName = 4;
    static float offsetPerOverLetter = 0.5f;
    

    private void Start()
    {
        if (noParent)
        {
            CreateWeaponUI();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && noParent)
        {
            viewPortPoint = Camera.main.WorldToViewportPoint(transform.position);
            if (activeWeapon == null) activeWeapon = other.GetComponent<ActiveWeapon>();

            if (WeaponInPickupViewPort())
            {
                if (!weaponUI)
                {
                    CreateWeaponUI();
                }

                if (!canPickup)
                {
                    if (viewPortPoint.z < activeWeapon.minDistanceToWeapon || activeWeapon.countWeponInArea == 0)
                    {
                        //Debug.Log(viewPortPoint.z + weaponStats.name);
                        canPickup = true;
                        activeWeapon.minDistanceToWeapon = viewPortPoint.z;
                        activeWeapon.countWeponInArea++;
                    }
                    else
                    {
                        weaponUI.gameObject.SetActive(false);
                    }

                    ShowWeaponStats();
                }
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
            if (activeWeapon.countWeponInArea > 0) activeWeapon.countWeponInArea--;
            activeWeapon = null;

            //Debug.Log("Exit" + weaponStats.name);
            weaponUI.gameObject.SetActive(false);
            canPickup = false;
        }
    }

    bool WeaponInPickupViewPort()
    {
        if (viewPortPoint.z < 2.5f && Mathf.Abs(viewPortPoint.x - 0.5f) < 0.2f && Mathf.Abs(viewPortPoint.y - 0.5f) < 0.2f)
        {
            return true;
        }
        return false;
    }

    public void ShowWeaponStats()
    {
        weaponUI.gameObject.SetActive(true);
        weaponUI.GetChild(0).GetComponent<WeaponUI>().weaponName.text = weaponStats.name;
    }

    public void CreateWeaponUI()
    {
        //Debug.Log("CREATE WEAPON UI FOR " + weaponStats.name);
        weaponUI = Instantiate(weaponUIPrefab, transform.parent);
        weaponUI.localScale = CalcualteLocalScale(0.19f, 0.19f, 0.19f, transform.parent.localScale);
        int multiplier = weaponStats.name.Length - standardLengthWeponName;
        if (multiplier > 0) weaponUI.GetChild(0).GetComponent<WeaponUI>().panel.localPosition -= offsetPerOverLetter * multiplier * Vector3.right;

        weaponUI.gameObject.SetActive(false);
    }

    Vector3 CalcualteLocalScale(float x, float y, float z, Vector3 parentScale)
    {
        return new Vector3(x / parentScale.x * weaponUI.localScale.x,
                           y / parentScale.y * weaponUI.localScale.y,
                           z / parentScale.z * weaponUI.localScale.z);
    }
}
