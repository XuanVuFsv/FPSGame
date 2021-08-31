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
        if (currentWeapon.GetComponent<Rigidbody>()) Destroy(currentWeapon.GetComponent<Rigidbody>());

        isHoldWeapon = true;
        handIk.weight = 1.0f;

        SetupNewWeapon();
    }

    public void DropWeapon()
    {
        currentWeapon.gameObject.AddComponent<Rigidbody>().AddForce(currentWeapon.forward * 0.05f, ForceMode.VelocityChange);
        Destroy(currentWeapon.GetComponent<Rigidbody>(), 10);
        currentWeapon.parent = null;
        currentWeapon = null;

        isHoldWeapon = false;
        handIk.weight = 0.0f;
    }

    public void SetupNewWeapon()
    {

    }
}