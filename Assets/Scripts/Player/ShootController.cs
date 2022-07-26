using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootController : MonoBehaviour
{
    [Header("Shooting")]
    public RaycastWeapon raycastWeapon;
    public bool isFire;

    private InputController inputController;

    [Header("Reload")]
    public Animator rigController;
    public WeaponAnimationEvents animationEvents;
    public GameObject leftHand, magazine, magazineHand;

    [Header("Weapon Settings")]
    //[Tooltip("How fast the weapon fires, higher value means faster rate of fire.")]
    public float fireRate;
    //Delay between shooting last bullet and reloading
    public float autoReloadDelay;
    //Total amount of ammo
    public int ammo;
    public bool autoReload;

    //Used for fire rate
    private float lastFired;
    private bool isReloading;
    //How much ammo is currently left
    public int currentAmmo, shootingTime = 0;
    //Check if out of ammo
    private bool outOfAmmo;

    [Header("Muzzleflash Settings")]
    public ParticleSystem sparkParticles;
    public ParticleSystem muzzleParticles;
    public int maxRandomValue = 5;
    public int minSparkEmission = 1;
    public int maxSparkEmission = 7;
    public bool randomMuzzleflash = false;
    [Range(2, 25)]
    public bool enableMuzzleflash = true;
    public bool enableSparks = true;

    private int minRandomValue = 1;
    private int randomMuzzleflashValue;

    [Header("Muzzleflash Light Settings")]
    public Light muzzleflashLight;
    public float lightDuration = 0.02f;

    // Start is called before the first frame update

    void Start()
    {
        animationEvents.weaponAnimationEvent.AddListener(OnAnimationEvent);
        inputController = InputController.Instance;

        //Weapon sway
        currentAmmo = ammo;

        //raycastWeapon.StopFiring();
    }

    // Update is called once per frame
    void Update()
    {
        //if (ActiveWeapon.Instance.isHoldWeapon && (int)GamePlayManager.Instance.state == 2) //only when test pickup weapon system
        {
            //If out of ammo
            if (currentAmmo == 0)
            {
                //Toggle bool
                outOfAmmo = true;
                isFire = false;
                //Auto reload if true
                if (autoReload == true && !isReloading)
                {
                    StartCoroutine(Reload());
                }
            }
            else
            {
                //Toggle bool
                outOfAmmo = false;
            }

            if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && !outOfAmmo && !isReloading)
            {
                //Shoot automatic
                if (Time.time - lastFired > 1 / fireRate)
                {
                    lastFired = Time.time;

                    //Remove 1 bullet from ammo
                    currentAmmo -= 1;
                    shootingTime++;

                    //Control muzzle flash
                    if (!inputController.isAim) //if not aiming
                    {
                    }
                    else //if aiming
                    {
                    }

                    isFire = true;
                    raycastWeapon.StartFiring();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                isFire = false;
                raycastWeapon.StopFiring();
            }

            //Reload 
            if (inputController.isReload && !isReloading)
            {
                //Reload
                StartCoroutine(Reload());
            }
        }
    }

    //Reload
    IEnumerator Reload()
    {
        rigController.SetTrigger("ReloadAK");
        isReloading = true;
        yield return new WaitForSeconds(1);
        //Restore ammo when reloading
        rigController.SetTrigger("ReloadAK");
        currentAmmo = ammo;
        outOfAmmo = false;
        isReloading = false;
    }

    void OnAnimationEvent(string eventName)
    {
        switch (eventName)  
        {
            case "detach_magazine":
                DeTachMagazine();
                break;
            case "drop_magazine":
                DropMagazine();
                break;
            case "refill_magazine":
                RefillMagazine();
                break;
            case "attach_magazine":
                AttachMagazine();
                break;
        }    
    }

    void DeTachMagazine()
    {
        magazineHand = Instantiate(magazine, leftHand.transform, true);
        magazine.SetActive(false);
    }

    void DropMagazine()
    {
            
    }

    void RefillMagazine()
    {

    }

    void AttachMagazine()
    {
        magazine.SetActive(true);
        Destroy(magazineHand);
    }
}
