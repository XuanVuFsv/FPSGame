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
    //How fast the weapon fires, higher value means faster rate of fire
    //[Tooltip("How fast the weapon fires, higher value means faster rate of fire.")]
    public float fireRate;
    //Eanbles auto reloading when out of ammo
    public bool autoReload;
    //Delay between shooting last bullet and reloading
    public float autoReloadDelay;
    //Check if reloading
    private bool isReloading;

    //How much ammo is currently left
    [SerializeField]
    private int currentAmmo;
    //Totalt amount of ammo
    public int ammo;
    //Check if out of ammo
    private bool outOfAmmo;

    [Header("Weapon Sway")]
    //Enables weapon sway
    public bool weaponSway;

    public float swayAmount = 0.02f;
    public float maxSwayAmount = 0.06f;
    public float swaySmoothValue = 4.0f;

    private Vector3 initialSwayPosition;

    [Header("Muzzleflash Settings")]
    public bool randomMuzzleflash = false;
    private int minRandomValue = 1;

    [Range(2, 25)]
    public int maxRandomValue = 5;

    private int randomMuzzleflashValue;

    public bool enableMuzzleflash = true;
    public ParticleSystem muzzleParticles;
    public bool enableSparks = true;
    public ParticleSystem sparkParticles;
    public int minSparkEmission = 1;
    public int maxSparkEmission = 7;

    [Header("Muzzleflash Light Settings")]
    public Light muzzleflashLight;
    public float lightDuration = 0.02f;

    private InputController inputController;

    //[SerializeField]
    //Animator animator;
    //[SerializeField]
    //MovementController movementController;

    // Start is called before the first frame update

    void Start()
    {
        //animator = transform.Find("Main").gameObject.GetComponent<Animator>();
        //movementController = GetComponent<MovementController>();
        inputController = InputController.Instance;

        //Weapon sway
        initialSwayPosition = transform.localPosition;
        currentAmmo = ammo;

        //raycastWeapon.StopFiring();
    }

    // Update is called once per frame
    void Update()
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
