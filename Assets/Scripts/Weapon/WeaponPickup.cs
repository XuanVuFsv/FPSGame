using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public RectTransform messageWeaponPrefab, messageWeapon;
    public WeaponStats weaponStats;
    public Vector3 viewPortPoint;
    public bool noParent = true;

    public bool isCollideWithPlayer;
    public string collideName;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && noParent)
        {
            collideName = other.tag;

            isCollideWithPlayer = true;

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
        else if (!isCollideWithPlayer && noParent)
        {
            Debug.Log(transform.parent.gameObject.name + ": " + other.name + " " + other.tag);
            isCollideWithPlayer = false;
        }
        //isCollideWithPlayer = false;

        //if (messageWeapon) messageWeapon.gameObject.SetActive(false);
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
        if (messageWeapon)
        {
            messageWeapon.gameObject.SetActive(true);
            return;
        }
        messageWeapon = Instantiate(messageWeaponPrefab, transform.parent);
        messageWeapon.GetChild(0).GetComponent<WeaponUI>().message.text += weaponStats.name;
    }
}
