using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private static InputController instance;

    public Vector2 move;
    public Vector2 look;

    public bool isIdle;
    public bool isWalk, isWalkRight, isWalkLeft, isWalkBackward, isWalkForward;
    public bool isAim;
    public bool isFire;
    public bool isReload;
    public bool isManipulationFire;
    public bool isJump;
    public bool isBrake;

    public float horizontal, vertical;
    public float rawHorizontal, rawVertical;
    public float mouseX, mouseY;
    public float fireValue = 0;

    void MakeInstance()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else instance = this;
    }

    public static InputController Instance
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

    }

    // Update is called once per frame
    void Update()
    {
        move.x = horizontal = Input.GetAxis("Horizontal");
        move.y = vertical = Input.GetAxis("Vertical");
        rawHorizontal = Input.GetAxisRaw("Horizontal");
        rawVertical = Input.GetAxisRaw("Vertical");
        look.x = mouseX = Input.GetAxis("Mouse X");
        look.y = mouseY = Input.GetAxis("Mouse Y");

        isJump = isBrake = Input.GetKeyDown(KeyCode.Space);
        isReload = Input.GetKeyDown(KeyCode.R);
        isAim = Input.GetMouseButtonDown(1);

        if (fireValue == 0) isFire = false;

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            isFire = true;
            fireValue += 0.25f;
        }
        else
        {
            fireValue -= 0.25f;
        }

        fireValue = Mathf.Clamp(fireValue, 0, 2);

        UpdateStatusAnimation();
    }

    void UpdateStatusAnimation()
    {
        if (horizontal != 0 || vertical != 0)
        {
            isIdle = false;
            isWalk = true;
        }
        else
        {
            isIdle = true;
            isWalk = false;
        }

        if (isFire || isAim)
        {
            isManipulationFire = true;
        }
        else isManipulationFire = false;
    }
}
