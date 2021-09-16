using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class ActiveWeapon : MonoBehaviour
{
    private static ActiveWeapon instance;
    public enum WeaponSlot
    {
        Primary = 0,
        Sidearm = 1,
        Melee = 2
    }

    public enum WeaponAction
    {
        Throw = 0,
        Pickup = 1,
        Switch = 2,
        View = 3
    }

    public UnityEngine.Animations.Rigging.Rig handIk;
    public WeaponPickup defaultWeapon;
    public Transform[] weaponActivateSlots;
    public WeaponPickup[] equippedWeapon = new WeaponPickup[3];
    public Transform weaponPivot;
    public Transform rightHandHolder, leftHandHolder;
    public Transform gunCamera;
    public float minDistanceToWeapon = 5, countWeponInArea = 0;
    public int activeWeaponIndex = 3;
    public bool isHoldWeapon = false;

    [SerializeField]
    Animator rigController;
    Transform[] equippedWeaponParent = new Transform[3];

    ShootController shootController;

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
        shootController = GetComponent<ShootController>();
        EquipWeapon(defaultWeapon);
        SetupNewWeapon(defaultWeapon.weaponStats);

        //foreach (Transform child in weaponPivot.transform)
        //{
        //    if (child.gameObject.tag == "Weapon")
        //    {
        //        EquipWeapon(child.GetChild(0).GetComponent<WeaponPickup>());
        //        SetupNewWeapon(equippedWeapon[activeWeaponIndex].weaponStats);
        //        break;
        //    }
        //}
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            DropWeapon(WeaponAction.Throw, activeWeaponIndex);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(equippedWeapon[0]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(equippedWeapon[1]);

        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon(equippedWeapon[2]);
        }
    }

    WeaponPickup GetWeapon(int index)
    {
        if (index < 0 || index > 2)
        {
            return null;
        }
        return equippedWeapon[index];
    }

    public void EquipWeapon(WeaponPickup newWeapon)
    {
        int weaponSlotIndex = (int)newWeapon.weaponSlot;
        isHoldWeapon = true;
        bool isExistWeaponSlot = GetWeapon(weaponSlotIndex);

        //Set current weapon and its parent
        equippedWeapon[weaponSlotIndex] = newWeapon;
        equippedWeaponParent[weaponSlotIndex] = equippedWeapon[weaponSlotIndex].transform.parent;

        //Ref weaponpickup of wepon and disable UI
        if (newWeapon.weaponUI) newWeapon.weaponUI.gameObject.SetActive(false);

        SetupUtilities.SetLayers(equippedWeaponParent[weaponSlotIndex], "Local Player", "Local Player", "Effect");

        equippedWeaponParent[weaponSlotIndex].gameObject.GetComponent<Rigidbody>().isKinematic = true;

        //Set parent for weapon, change noParent value and run animation
        newWeapon.noParent = false;

        if (activeWeaponIndex > 1 || !isHoldWeapon || isExistWeaponSlot)
        {
            activeWeaponIndex = weaponSlotIndex;
            equippedWeaponParent[weaponSlotIndex].parent = weaponPivot;
            SetWeaponAnimation();
            equippedWeaponParent[weaponSlotIndex].parent = weaponActivateSlots[weaponSlotIndex];
            return;
        }

        equippedWeaponParent[weaponSlotIndex].parent = weaponActivateSlots[weaponSlotIndex];
    }

    public void DropWeapon(WeaponAction action, int weaponSlotIndex)
    {
        if (!isHoldWeapon || !equippedWeapon[weaponSlotIndex]) return;
        if (action == WeaponAction.Switch)
        {
            rigController.speed = -1;
            rigController.Play("Base Layer.Equip " + equippedWeapon[activeWeaponIndex].weaponStats.name, 0, 0f);
            rigController.speed = 1;
            return;
        }
        if (action == WeaponAction.Throw) rigController.Play("Base Layer.UnArmed", 0, 0f);

        //Set parent for weapon and change noParent value
        equippedWeaponParent[weaponSlotIndex].parent = null;
        equippedWeapon[weaponSlotIndex].noParent = true;

        SetupUtilities.SetLayers(equippedWeaponParent[weaponSlotIndex], "Ignore Player", "Only Player", null);

        //parent.localEulerAngles = Vector3.zero;
        equippedWeaponParent[weaponSlotIndex].gameObject.GetComponent<Rigidbody>().isKinematic = false;
        //parent.gameObject.GetComponent<Rigidbody>().AddForce(currentWeapon.forward, ForceMode.Impulse);

        //Animation setup
        isHoldWeapon = false;

        if (!equippedWeapon[weaponSlotIndex].weaponUI)
        {
            equippedWeapon[weaponSlotIndex].CreateWeaponUI();
        }

        //Set current weapon, weapon pickup and its parent
        //equippedWeapon[activeWeaponIndex] = null;
        //parent = null;
        equippedWeapon[weaponSlotIndex] = null;
    }

    public void SetupNewWeapon(WeaponStats weaponStats)
    {
        shootController.fireRate = weaponStats.fireRate;
        shootController.raycastWeapon = equippedWeapon[activeWeaponIndex].GetComponent<RaycastWeapon>();
    }

    void SwitchWeapon(WeaponPickup activateWeapon)
    {
        if (activateWeapon)
        {
            if (activeWeaponIndex == (int)activateWeapon.weaponSlot) return;
        }
        else return;
        DropWeapon(ActiveWeapon.WeaponAction.Switch, (int)equippedWeapon[activeWeaponIndex].weaponSlot);
        EquipWeapon(activateWeapon);
        SetupNewWeapon(activateWeapon.weaponStats);
    }

    void SetWeaponAnimation()
    {
        rigController.Rebind();
        rigController.Play("Base Layer.Equip " + equippedWeapon[activeWeaponIndex].weaponStats.name, 0, 0f);
    }

    void SetAnimationDelayded()
    {

    }

    [ContextMenu("Save Weapon Pose")]
    void SaveWeaponPose()
    {
        GameObjectRecorder recorder = new GameObjectRecorder(transform.GetChild(0).gameObject);
        recorder.BindComponentsOfType<Transform>(weaponPivot.gameObject, false);
        recorder.BindComponentsOfType<Transform>(equippedWeaponParent[activeWeaponIndex].gameObject, false);
        recorder.BindComponentsOfType<Transform>(gunCamera.gameObject, false);
        recorder.BindComponentsOfType<Transform>(leftHandHolder.gameObject, false);
        recorder.BindComponentsOfType<Transform>(rightHandHolder.gameObject, false);
        recorder.TakeSnapshot(0.0f);
        recorder.SaveToClip(equippedWeapon[activeWeaponIndex].GetComponent<RaycastWeapon>().weaponAnimation);
    }
}