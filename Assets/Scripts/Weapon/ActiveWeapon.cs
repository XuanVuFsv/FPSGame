using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class ActiveWeapon : MonoBehaviour
{
    private static ActiveWeapon instance;

    ShootController shootController;
    WeaponPickup weaponPickup;

    public UnityEngine.Animations.Rigging.Rig handIk;
    public Transform weaponPivot;
    public Transform currentWeapon;
    public Transform parent;
    public Transform rightHandHolder, leftHandHolder;
    public Transform gunCamera;
    public float minDistanceToWeapon = 5, countWeponInArea = 0;
    public bool isHoldWeapon = false;

    [SerializeField]
    Animator animator;
    [SerializeField]
    AnimatorOverrideController animatorOverride;

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
        animator = transform.GetChild(0).gameObject.GetComponent<Animator>();
        animatorOverride = animator.runtimeAnimatorController as AnimatorOverrideController;

        shootController = GetComponent<ShootController>();

        foreach (Transform child in weaponPivot.transform)
        {
            if (child.gameObject.tag == "Weapon")
            {
                EquipWeapon(child.GetChild(0));
                SetupNewWeapon(weaponPickup.weaponStats);
                break;
            }
            else
            {
                handIk.weight = 0.0f;
                animator.SetLayerWeight(1, 0.0f);
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
        //Set current weapon and its parent
        currentWeapon = newWeapon;
        parent = currentWeapon.parent;

        SetupUtilities.SetLayers(parent, "Local Player", "Local Player", "Effect");

        //Ref weaponpickup of wepon and disable UI
        weaponPickup = currentWeapon.gameObject.GetComponent<WeaponPickup>();
        if (weaponPickup.weaponUI) weaponPickup.weaponUI.gameObject.SetActive(false);

        parent.gameObject.GetComponent<Rigidbody>().isKinematic = true;

        //Set parent for weapon and change noParent value
        parent.parent = weaponPivot;
        weaponPickup.noParent = false;

        //Animation setup
        isHoldWeapon = true;
        handIk.weight = 1.0f;
        animator.SetLayerWeight(1, 1.0f);

        Invoke(nameof(SetAnimationDelayded), 0.001f);
    }

    public void DropWeapon()
    {
        if (!isHoldWeapon) return;

        //Set parent for weapon and change noParent value
        parent.parent = null;
        weaponPickup.noParent = true;

        SetupUtilities.SetLayers(parent, "Ignore Player", "Only Player", null);

        parent.localEulerAngles = Vector3.zero;
        parent.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        //parent.gameObject.GetComponent<Rigidbody>().AddForce(currentWeapon.forward, ForceMode.Impulse);

        //Animation setup
        isHoldWeapon = false;
        animatorOverride["Weapon Animation Empty"] = null;
        handIk.weight = 0.0f;
        animator.SetLayerWeight(1, 0.0f);

        //Set current weapon, weapon pickup and its parent
        currentWeapon = null;
        parent = null;
        weaponPickup = null;
    }

    public void SetupNewWeapon(WeaponStats weaponStats)
    {
        shootController.fireRate = weaponStats.fireRate;
        shootController.raycastWeapon = currentWeapon.GetComponent<RaycastWeapon>();
    }

    void SetAnimationDelayded()
    {
        animatorOverride["Weapon Animation Empty"] = currentWeapon.GetComponent<RaycastWeapon>().weaponAnimation;
    }

    [ContextMenu("Save Weapon Pose")]
    void SaveWeaponPose()
    {
        GameObjectRecorder recorder = new GameObjectRecorder(transform.GetChild(0).gameObject);
        recorder.BindComponentsOfType<Transform>(weaponPivot.gameObject, false);
        recorder.BindComponentsOfType<Transform>(parent.gameObject, false);
        recorder.BindComponentsOfType<Transform>(gunCamera.gameObject, false);
        recorder.BindComponentsOfType<Transform>(leftHandHolder.gameObject, false);
        recorder.BindComponentsOfType<Transform>(rightHandHolder.gameObject, false);
        recorder.TakeSnapshot(0.0f);
        recorder.SaveToClip(currentWeapon.GetComponent<RaycastWeapon>().weaponAnimation);
    }
}