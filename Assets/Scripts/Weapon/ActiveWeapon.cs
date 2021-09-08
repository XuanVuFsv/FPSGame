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
        currentWeapon = newWeapon;
        parent = currentWeapon.parent;

        weaponPickup = currentWeapon.gameObject.GetComponent<WeaponPickup>();
        if (weaponPickup.weaponUI) weaponPickup.weaponUI.gameObject.SetActive(true);

        parent.gameObject.GetComponent<Rigidbody>().isKinematic = true;

        parent.parent = weaponPivot;
        weaponPickup.noParent = false;

        //parent.localPosition = Vector3.zero;

        SetupUtilities.SetLayers(parent, "Local Player", "Local Player", "Effect");

        isHoldWeapon = true;
        handIk.weight = 1.0f;
        animator.SetLayerWeight(1, 1.0f);

        Invoke(nameof(SetAnimationDelayded), 0.001f);

        //SetAnimationDelayded();
    }

    public void DropWeapon()
    {
        if (!isHoldWeapon) return;

        parent.parent = null;
        weaponPickup.noParent = true;

        parent.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        parent.gameObject.GetComponent<Rigidbody>().AddForce(currentWeapon.forward, ForceMode.Impulse);

        isHoldWeapon = false;
        handIk.weight = 0.0f;

        SetupUtilities.SetLayers(parent, "Ignore Player", "Only Player", null);

        currentWeapon = null;
        parent = null;
    }

    public void SetupNewWeapon(WeaponStats weaponStats)
    {
        shootController.fireRate = weaponStats.fireRate;
        shootController.raycastWeapon = currentWeapon.GetComponent<RaycastWeapon>();
    }

    void SetAnimationDelayded()
    {
        Debug.Log(animatorOverride["Weapon Animation Empty"].name + " " + currentWeapon.GetComponent<RaycastWeapon>().weaponAnimation.name);
        animatorOverride["Weapon Animation Empty"] = currentWeapon.GetComponent<RaycastWeapon>().weaponAnimation;
        Debug.Log(animatorOverride["Weapon Animation Empty"].name + " " + currentWeapon.GetComponent<RaycastWeapon>().weaponAnimation.name);
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