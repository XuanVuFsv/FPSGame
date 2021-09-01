using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    private static ActiveWeapon instance;

    public UnityEngine.Animations.Rigging.Rig handIk;
    public Transform weaponPivot;
    public bool isHoldWeapon = false;

    Transform currentWeapon;

    void MakeInstance()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else instance = this;
    }

    public static ActiveWeapon Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        MakeInstance();
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in weaponPivot.transform)
        {
            if (child.gameObject.tag == "Weapon")
            {
                EquipWeapon(child);
                break;
            }
            else
            {
                handIk.weight = 0.0f;
            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && currentWeapon)
        {
            DropWeapon();
        }
    }

    public void EquipWeapon(Transform newWeapon)
    {
        currentWeapon = newWeapon;
        currentWeapon.gameObject.GetComponent<WeaponPickup>().noParent = false;
        if (currentWeapon.gameObject.GetComponent<Rigidbody>()) Destroy(currentWeapon.GetComponent<Rigidbody>());

        currentWeapon.parent = weaponPivot;
        currentWeapon.localPosition = Vector3.zero;
        currentWeapon.localEulerAngles = new Vector3(272f, 155f, 180);
        isHoldWeapon = true;
        handIk.weight = 1.0f;
    }

    public void DropWeapon()
    {
        currentWeapon.gameObject.AddComponent<Rigidbody>().AddForce(currentWeapon.forward * 5f, ForceMode.VelocityChange);
        Destroy(currentWeapon.gameObject.GetComponent<Rigidbody>(), 5);

        currentWeapon.parent = null;

        isHoldWeapon = false;
        handIk.weight = 0.0f;
    }

    public void SetupNewWeapon(WeaponStats weaponStats)
    {
        ShootController shootController = GetComponent<ShootController>();
        shootController.fireRate = weaponStats.fireRate;
        shootController.raycastWeapon = currentWeapon.GetComponent<RaycastWeapon>();
        currentWeapon.gameObject.GetComponent<WeaponPickup>().noParent = true;
    }
}