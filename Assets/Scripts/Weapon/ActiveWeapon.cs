using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor.Animations;

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
    public static WeaponPickup[] equippedWeapon = new WeaponPickup[3];

    public UnityEngine.Animations.Rigging.Rig handIk;
    public Cinemachine.CinemachineVirtualCamera playerCamera;
    public WeaponPickup defaultWeapon1, defaultWeapon2;
    public Transform[] weaponActivateSlots;
    public Transform weaponPivot;
    public Transform rightHandHolder, leftHandHolder;
    public Transform gunCamera;
    public List<WeaponPickup> triggerWeaponList = new List<WeaponPickup>();
    public float minDistanceToWeapon = 5/*, countWeponInArea = 0*/;
    public int activeWeaponIndex = 3;
    public bool isHoldWeapon = false;

    [SerializeField]
    Animator rigController;
    [SerializeField]
    Transform[] equippedWeaponParent = new Transform[3];

    ShootController shootController;
    Transform droppedWeapon;

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
        EquipWeapon(WeaponAction.Pickup, defaultWeapon1, true);
        SetupNewWeapon(defaultWeapon1.weaponStats);
        AttachWeapon(defaultWeapon2, weaponActivateSlots[2], 2);
    }

    // Update is called once per frame
    void Update()
    {
        rigController.SetBool("isHoldWeapon", isHoldWeapon);


        if (Input.GetKeyDown(KeyCode.KeypadEnter)) Time.timeScale = 1;

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (activeWeaponIndex != 2)
            {
                DropWeapon(WeaponAction.Throw, activeWeaponIndex);

                if (!GetWeapon(0) || !GetWeapon(1))
                {
                    SwitchWeapon(equippedWeapon[2]);
                }
                else
                {
                    if (activeWeaponIndex == 0) SwitchWeapon(equippedWeapon[1]);
                    else SwitchWeapon(equippedWeapon[0]);
                }
            }
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

    public static WeaponPickup GetWeapon(int index)
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

        if (isHoldWeapon) DropWeapon(ActiveWeapon.WeaponAction.Switch, (int)equippedWeapon[activeWeaponIndex].weaponSlot);
       
        EquipWeapon(WeaponAction.Switch, activateWeapon, true);
        SetupNewWeapon(activateWeapon.weaponStats);
    }

    public void EquipWeapon(WeaponAction action, WeaponPickup newWeapon, bool runAnimation, bool isExistWeaponSlot = false)
    {
        int weaponSlotIndex = (int)newWeapon.weaponSlot;

        equippedWeapon[weaponSlotIndex] = newWeapon;
        equippedWeaponParent[weaponSlotIndex] = equippedWeapon[weaponSlotIndex].transform.parent;


        bool sameWeaponSlot = (activeWeaponIndex == weaponSlotIndex);

        if (action == WeaponAction.Switch || runAnimation || isExistWeaponSlot)
        {

            if (action == WeaponAction.Switch || runAnimation)
            {
                activeWeaponIndex = weaponSlotIndex;
                newWeapon.transform.parent.gameObject.SetActive(true);
            }

            if (sameWeaponSlot || runAnimation)
            {
                equippedWeaponParent[weaponSlotIndex].parent = weaponPivot;
                SetWeaponAnimation();
            }
        }

        if (activeWeaponIndex == weaponSlotIndex)
        {
            newWeapon.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            newWeapon.transform.parent.gameObject.SetActive(false);
        }

        if (action == WeaponAction.Pickup)
        {
            //Set current weapon and its parent
            newWeapon.GetComponent<CameraShake>().SetUpWeaponRecoilForNewWeapon(playerCamera, rigController);

            //UI, Layer, Physic
            if (newWeapon.weaponUI) newWeapon.weaponUI.gameObject.SetActive(false);
            SetupUtilities.SetLayers(equippedWeaponParent[weaponSlotIndex], "Local Player", "Local Player", "Effect");
            equippedWeaponParent[weaponSlotIndex].gameObject.GetComponent<Rigidbody>().isKinematic = true;

            //Set parent for weapon, change noParent value and run animation
            newWeapon.noParent = false;
        }

        equippedWeaponParent[weaponSlotIndex].parent = weaponActivateSlots[weaponSlotIndex];
        equippedWeaponParent[weaponSlotIndex].localPosition = new Vector3(0, -0.5f, 0.5f);
        isHoldWeapon = true;
    }

    public void DropWeapon(WeaponAction action, int weaponSlotIndex)
    {
        bool isExsitWeaponSlot = true;

        //Don't run this function when don't hold any weapon or throw a null weapon
        if (!equippedWeapon[weaponSlotIndex])
        {
            //Debug.Log("Don't have weapon index: " + weaponSlotIndex);
            isExsitWeaponSlot = false;
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
            equippedWeapon[weaponSlotIndex].GetComponent<CameraShake>().rigController = null;
            equippedWeapon[weaponSlotIndex].noParent = true;

            droppedWeapon = equippedWeaponParent[weaponSlotIndex].transform;

            SetupUtilities.SetLayers(equippedWeaponParent[weaponSlotIndex], "Ignore Player", "Only Player", null);

            equippedWeaponParent[weaponSlotIndex].GetComponent<Rigidbody>().isKinematic = false;
            equippedWeaponParent[weaponSlotIndex].GetComponent<Rigidbody>().AddForce(gunCamera.forward * 2, ForceMode.Impulse);

            isHoldWeapon = false;

            if (!equippedWeapon[weaponSlotIndex].weaponUI)
            {
                equippedWeapon[weaponSlotIndex].CreateWeaponUI();
            }
        }

        if (action == WeaponAction.Throw || (action == WeaponAction.Pickup && isExsitWeaponSlot))
        {
            droppedWeapon.gameObject.SetActive(true);
            droppedWeapon.parent = null;
            equippedWeaponParent[weaponSlotIndex] = null;
            equippedWeapon[weaponSlotIndex] = null;
        }
    }

    public void AttachWeapon(WeaponPickup attachedWeapon, Transform attachedWeaponParent, int attachedWeaponSlotIndex)
    {
        attachedWeapon.transform.parent.gameObject.SetActive(false);
        SetupUtilities.SetLayers(attachedWeapon.transform.parent, "Local Player", "Local Player", "Effect");
        attachedWeapon.noParent = false;

        attachedWeapon.GetComponent<CameraShake>().rigController = rigController;
        equippedWeapon[attachedWeaponSlotIndex] = attachedWeapon;
        equippedWeaponParent[attachedWeaponSlotIndex] = attachedWeapon.transform.parent;

        StartCoroutine(SetWeaponParent(attachedWeapon, attachedWeaponParent));

    }

    public void SetupNewWeapon(WeaponStats weaponStats)
    {
        shootController.fireRate = weaponStats.fireRate;
        shootController.autoReloadDelay = weaponStats.reloadSpeed;
        shootController.raycastWeapon = equippedWeapon[activeWeaponIndex].GetComponent<RaycastWeapon>();
        shootController.magazine = equippedWeapon[activeWeaponIndex].GetComponent<RaycastWeapon>().magazine;
    }

    void SetWeaponAnimation()
    {
        Vector3 droppedWeaponPosition = Vector3.zero;
        if (droppedWeapon)
        {
            droppedWeaponPosition = droppedWeapon.position;
            rigController.Rebind();
            droppedWeapon.position = droppedWeaponPosition;
        }
        else rigController.Rebind();

        rigController.Play("Base Layer.Equip " + equippedWeapon[activeWeaponIndex].weaponStats.name, 0, 0f);
    }

    IEnumerator SetWeaponParent(WeaponPickup weapon, Transform weaponParent)
    {
        yield return new WaitForSeconds(defaultWeapon2.GetComponent<RaycastWeapon>().weaponAnimation.length);
        weapon.transform.parent.parent = weaponParent;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!triggerWeaponList.Contains(other.GetComponent<WeaponPickup>()))
        {
            triggerWeaponList.Add(other.GetComponent<WeaponPickup>());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (triggerWeaponList.Contains(other.GetComponent<WeaponPickup>()))
        {
            //minDistanceToWeapon = 5;
            triggerWeaponList.Remove(other.GetComponent<WeaponPickup>());
        }
    }

    public WeaponPickup GetNearestWeapon()
    {
        WeaponPickup weapon = null;
        float min = 5;
        for (int i = 0; i < triggerWeaponList.Count; i++)
        {
            if (triggerWeaponList[i].viewPortPoint.z <= min && triggerWeaponList[i].inWeaponViewport)
            {
                min = triggerWeaponList[i].viewPortPoint.z;
                weapon = triggerWeaponList[i];
            }
        }
        return weapon;
    }

    //[ContextMenu("Save Weapon Pose")]
    //void SaveWeaponPose()
    //{
    //    GameObjectRecorder recorder = new GameObjectRecorder(transform.GetChild(0).gameObject);
    //    recorder.BindComponentsOfType<Transform>(weaponPivot.gameObject, false);
    //    recorder.BindComponentsOfType<Transform>(equippedWeaponParent[activeWeaponIndex].gameObject, false);
    //    recorder.BindComponentsOfType<Transform>(gunCamera.gameObject, false);
    //    recorder.BindComponentsOfType<Transform>(leftHandHolder.gameObject, false);
    //    recorder.BindComponentsOfType<Transform>(rightHandHolder.gameObject, false);
    //    recorder.TakeSnapshot(0.0f);
    //    recorder.SaveToClip(equippedWeapon[activeWeaponIndex].GetComponent<RaycastWeapon>().weaponAnimation);
    //}
}