using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class ActiveWeapon : MonoBehaviour
{
    private static ActiveWeapon instance;

    ShootController shootController;
    public UnityEngine.Animations.Rigging.Rig handIk;
    public Transform weaponPivot;
    public Transform currentWeapon;
    public Transform rightHandHolder, leftHandHolder;
    public AnimationClip weaponAnimation;
    public bool isHoldWeapon = false;

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
        shootController = GetComponent<ShootController>();
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
        if (currentWeapon.gameObject.GetComponent<Rigidbody>()) Destroy(currentWeapon.GetComponent<Rigidbody>());

        currentWeapon.parent = weaponPivot;
        currentWeapon.gameObject.GetComponent<WeaponPickup>().noParent = false;

        currentWeapon.localPosition = Vector3.zero;
        currentWeapon.localEulerAngles = new Vector3(272f, 155f, 180);

        isHoldWeapon = true;
        handIk.weight = 1.0f;
    }

    public void DropWeapon()
    {
        if (!isHoldWeapon) return;

        currentWeapon.parent = null;
        currentWeapon.gameObject.GetComponent<WeaponPickup>().noParent = true;

        currentWeapon.gameObject.AddComponent<Rigidbody>().AddForce(currentWeapon.forward * 5, ForceMode.VelocityChange);
        Destroy(currentWeapon.gameObject.GetComponent<Rigidbody>(), 5);

        isHoldWeapon = false;
        handIk.weight = 0.0f;

        currentWeapon = null;
    }

    public void SetupNewWeapon(WeaponStats weaponStats)
    {
        shootController.fireRate = weaponStats.fireRate;
        shootController.raycastWeapon = currentWeapon.GetComponent<RaycastWeapon>();
    }

    [ContextMenu("Save Weapon Pose")]
    void SaveWeaponPose()
    {
        GameObjectRecorder recorder = new GameObjectRecorder(gameObject);
        recorder.BindComponentsOfType<Transform>(weaponPivot.gameObject, false);
        recorder.BindComponentsOfType<Transform>(leftHandHolder.gameObject, false);
        recorder.BindComponentsOfType<Transform>(rightHandHolder.gameObject, false);
        recorder.TakeSnapshot(0.0f);
        recorder.SaveToClip(weaponAnimation);
    }
}