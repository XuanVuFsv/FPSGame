using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public RectTransform weaponUIPrefab, weaponUI;
    public WeaponStats weaponStats;
    public ActiveWeapon.WeaponSlot weaponSlot;
    public Vector3 viewPortPoint;
    public bool noParent = true;
    public bool canPickup = false;

    public bool inWeaponViewport;
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

    private void Update()
    {
        if (inWeaponViewport && weaponUI)
        {
            if (weaponSlot == ActiveWeapon.WeaponSlot.Primary)
            {
                weaponUI.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, -transform.parent.localEulerAngles.z);
            }
            else
            {
                weaponUI.GetComponent<RectTransform>().localEulerAngles = new Vector3(-transform.parent.localEulerAngles.x, 0, 0);
            }
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player") && noParent)
    //    {
    //        if (activeWeapon == null) activeWeapon = other.GetComponent<ActiveWeapon>();
    //        activeWeapon.countWeponInArea++;
    //    }
    //}

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(Camera.main.transform.position, transform.position);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && noParent)
        {
            viewPortPoint = Camera.main.WorldToViewportPoint(transform.position);
            if (activeWeapon == null) activeWeapon = other.GetComponent<ActiveWeapon>();

            inWeaponViewport = WeaponInPickupViewPort();

            if (inWeaponViewport)
            {
                if (!weaponUI)
                {
                    CreateWeaponUI();
                }

                if (activeWeapon.triggerWeaponList.Count == 1)
                {
                    canPickup = true;
                    ShowWeaponStats();
                }
                else
                {
                    //if (viewPortPoint.z < activeWeapon.minDistanceToWeapon)
                    //{
                    //    activeWeapon.minDistanceToWeapon = viewPortPoint.z;

                    //    if (!canPickup)
                    //    {
                    //        //Debug.Log("Show" + transform.parent.name);
                    //        canPickup = true;
                    //    }
                    //    else ShowWeaponStats();
                    //}
                    //else
                    //{
                    //    canPickup = false;
                    //    weaponUI.gameObject.SetActive(false);
                    //}
                    if (this == activeWeapon.GetNearestWeapon())
                    {
                        canPickup = true;
                        ShowWeaponStats();
                    }
                    else
                    {
                        canPickup = false;
                        weaponUI.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                //activeWeapon.minDistanceToWeapon = 5;
                canPickup = false;
                weaponUI.gameObject.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.H) && canPickup)
            {
                //ActiveWeapon newActiveWeapon = other.GetComponent<ActiveWeapon>();
                //Debug.Log(newActiveWeapon == activeWeapon);
                activeWeapon.triggerWeaponList.Remove(this);
                bool isExistWeaponSlot = ActiveWeapon.GetWeapon((int)weaponSlot);
                activeWeapon.DropWeapon(ActiveWeapon.WeaponAction.Pickup, (int)weaponSlot);
                activeWeapon.EquipWeapon(ActiveWeapon.WeaponAction.Pickup, this, false, isExistWeaponSlot);
                activeWeapon.SetupNewWeapon(weaponStats);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && noParent)
        {
            //if (activeWeapon.countWeponInArea > 0) activeWeapon.countWeponInArea--;
            activeWeapon = null;

            weaponUI.gameObject.SetActive(false);
            canPickup = false;
        }
    }

    bool WeaponInPickupViewPort()
    {
        if (viewPortPoint.z < 3.5f && Mathf.Abs(viewPortPoint.x - 0.5f) < 0.35f && Mathf.Abs(viewPortPoint.y - 0.5f) < 0.35f)
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
