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
    public Cinemachine.CinemachineVirtualCamera playerCamera;
    public WeaponPickup defaultWeapon;
    public WeaponPickup[] equippedWeapon = new WeaponPickup[3];
    public Transform[] weaponActivateSlots;
    public Transform weaponPivot;
    public Transform rightHandHolder, leftHandHolder;
    public Transform gunCamera;
    public float minDistanceToWeapon = 5, countWeponInArea = 0;
    public int activeWeaponIndex = 3;
    public bool isHoldWeapon = false;

    [SerializeField]
    Animator rigController;
    [SerializeField]
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
        EquipWeapon(WeaponAction.Pickup, defaultWeapon);
        SetupNewWeapon(defaultWeapon.weaponStats);

        {
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

    void SwitchWeapon(WeaponPickup activateWeapon)
    {
        //only run this funtion when player swich to a weapon not null and different weapon type
        if (activateWeapon)
        {
            if (activeWeaponIndex == (int)activateWeapon.weaponSlot) return;
        }
        else return;
        DropWeapon(ActiveWeapon.WeaponAction.Switch, (int)equippedWeapon[activeWeaponIndex].weaponSlot);
        EquipWeapon(WeaponAction.Switch, activateWeapon);
        SetupNewWeapon(activateWeapon.weaponStats);
    }

    public void EquipWeapon(WeaponAction action, WeaponPickup newWeapon)
    {
        int weaponSlotIndex = (int)newWeapon.weaponSlot;
        bool isExistWeaponSlot = GetWeapon(weaponSlotIndex);

        //current WeaponSlot or WeaponSlot will be replace by new WeaponSlot
        //WeaponSlot prevWeaponSlot = WeaponSlot.Melee;
        //if (equippedWeapon[activeWeaponIndex]) prevWeaponSlot = equippedWeapon[activeWeaponIndex].weaponSlot;

        if (action == WeaponAction.Switch) newWeapon.transform.parent.gameObject.SetActive(true);

        if (action == WeaponAction.Pickup)
        {
            //Set current weapon and its parent
            newWeapon.GetComponent<WeaponRecoil>().SetUpWeaponRecoilForNewWeapon(playerCamera, rigController);
            equippedWeapon[weaponSlotIndex] = newWeapon;
            equippedWeaponParent[weaponSlotIndex] = equippedWeapon[weaponSlotIndex].transform.parent;

            //UI, Layer, Physic
            if (newWeapon.weaponUI) newWeapon.weaponUI.gameObject.SetActive(false);
            SetupUtilities.SetLayers(equippedWeaponParent[weaponSlotIndex], "Local Player", "Local Player", "Effect");
            equippedWeaponParent[weaponSlotIndex].gameObject.GetComponent<Rigidbody>().isKinematic = true;

            //Set parent for weapon, change noParent value and run animation
            newWeapon.noParent = false;
        }

        if (activeWeaponIndex > 1 || isExistWeaponSlot) //Equip when current weapon is melee, don't hold a weapon, change a weapon or don't pickup new weapon type 
        {
            activeWeaponIndex = weaponSlotIndex;
            equippedWeaponParent[weaponSlotIndex].parent = weaponPivot;
            SetWeaponAnimation();
        }
        else
        {
            //Enable new weapon depend on situation player pickup (pick up to change exsiting weapon or equip new weapon or pickup to switch from melee to primary or sidearm weapon)
            newWeapon.transform.parent.gameObject.SetActive(false);
        }
        equippedWeaponParent[weaponSlotIndex].parent = weaponActivateSlots[weaponSlotIndex];

        //if (!isExistWeaponSlot && isHoldWeapon && activeWeaponIndex != 2) newWeapon.transform.parent.gameObject.SetActive(false);
        //activeWeaponIndex = weaponSlotIndex;
        isHoldWeapon = true;
    }

    public void DropWeapon(WeaponAction action, int weaponSlotIndex)
    {
        //Don't run this function when don't hold any weapon or throw a null weapon
        //if (!isHoldWeapon) return;
        if (!equippedWeapon[weaponSlotIndex])
        {
            if (activeWeaponIndex == 2) equippedWeaponParent[activeWeaponIndex].gameObject.SetActive(false);
            return;
        }
        else
        {
            //Switch
            if (action == WeaponAction.Switch)
            {
                rigController.SetFloat("multiplier", -1);
                rigController.Play("Base Layer.Equip " + equippedWeapon[activeWeaponIndex].weaponStats.name, 0, 0f);
                rigController.SetFloat("multiplier", 1);
                equippedWeaponParent[activeWeaponIndex].gameObject.SetActive(false);
                return;
            }
            //Throw or Pickup
            //Set parent for weapon and change noParent value, layers
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
        }

        if (action == WeaponAction.Throw)
        {
            rigController.Play("Base Layer.UnArmed", 0, 0f);
            //Set current weapon, weapon pickup and its parent
            equippedWeaponParent[weaponSlotIndex] = null;
            equippedWeapon[weaponSlotIndex] = null;
        }
        else if (action == WeaponAction.Pickup)
        {
            equippedWeaponParent[activeWeaponIndex].gameObject.SetActive(false);
        }
    }

    public void SetupNewWeapon(WeaponStats weaponStats)
    {
        shootController.fireRate = weaponStats.fireRate;
        shootController.autoReloadDelay = weaponStats.reloadSpeed;
        shootController.raycastWeapon = equippedWeapon[activeWeaponIndex].GetComponent<RaycastWeapon>();
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