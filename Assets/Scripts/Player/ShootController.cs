using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootController : MonoBehaviour
{
    public RaycastWeapon raycastWeapon;
    public bool isFire;

    //Used for fire rate
    private float lastFired;

    [Header("Weapon Settings")]
    //[Tooltip("How fast the weapon fires, higher value means faster rate of fire.")]
    public float fireRate;
    //Delay between shooting last bullet and reloading
    public float autoReloadDelay;
    //Total amount of ammo
    public int ammo;
    public bool autoReload;

    private bool isReloading;
    //How much ammo is currently left
    [SerializeField]
    private int currentAmmo;
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

    private InputController inputController;

    // Start is called before the first frame update

    void Start()
    {
        inputController = InputController.Instance;

        //Weapon sway
        currentAmmo = ammo;

        //raycastWeapon.StopFiring();
    }

    // Update is called once per frame
    void Update()
    {
        if (ActiveWeapon.Instance.isHoldWeapon) //only when test pickup weapon system
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
        isReloading = true;
        yield return new WaitForSeconds(autoReloadDelay);
        //Restore ammo when reloading
        currentAmmo = ammo;
        outOfAmmo = false;
        isReloading = false;
    }
}
