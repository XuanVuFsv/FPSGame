using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    public RaycastWeapon raycastWeapon;
    public float fireRate;
    public bool isFire;

    [SerializeField]
    Animator animator;
    [SerializeField]
    MovementController movementController;

    private InputController inputController;
    // Start is called before the first frame update
    void Start()
    {
        animator = transform.Find("Main").gameObject.GetComponent<Animator>();
        movementController = GetComponent<MovementController>();
        inputController = GetComponent<InputController>();

        raycastWeapon.StopFiring();
    }

    // Update is called once per frame
    void Update()
    {
        isFire = inputController.isFire;

        if (Input.GetMouseButtonDown(0))
        {
            raycastWeapon.StartFiring();
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            raycastWeapon.StopFiring();
        }
    }


}
