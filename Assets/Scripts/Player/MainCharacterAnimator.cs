using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainCharacterAnimator : MonoBehaviour
{
    public Animator animator;
    public List<Animator> weaponAnimators;

    [SerializeField]
    private MovementController movementController;

    [SerializeField]
    private bool endFireSession = true;
    [SerializeField]
    private bool endFireAnimation = true;
    [SerializeField]
    private string currentFireAnimation;
    [SerializeField]
    private float timeEndFireAnimation = 1f;

    private InputController inputController;
    private ShootController shootController;

    #region Test
    public bool isUseVSync;
    public int targetFrameRate;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if (isUseVSync)
        {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = targetFrameRate;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }

        movementController = GetComponent<MovementController>();
        inputController = GetComponent<InputController>();
        shootController = GetComponent<ShootController>();
    }
    
    // Update is called once per frame
    void Update()
    {
        UpdateAnimatorParameters();
        Test();
    }

    void UpdateAnimatorParameters()
    {
        #region Basic Animation Properties

        //Change aim animator parameter when right mouse down
        if (inputController.isAim)
        {
            animator.SetBool("isAim", !animator.GetBool("isAim"));
        }
        animator.SetBool("isManipulationFire", inputController.isManipulationFire);
        animator.SetFloat("vertical", inputController.vertical);
        animator.SetFloat("horizontal", inputController.horizontal);
        animator.SetBool("allowJump", movementController.allowJump);
        #endregion

        #region Idle and Walk
        if (movementController.isGrounded)
        {
            animator.SetBool("isIdle", inputController.isIdle);
            animator.SetBool("isWalk", inputController.isWalk);
        }
        else
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("isWalk", false);
        }
        #endregion

        #region Shoot
        if (ActiveWeapon.Instance.isHoldWeapon) // only when test pickup weapon system
        {
            if (endFireSession)
            {
                //Single Shoot
                if (Input.GetMouseButtonDown(0) && endFireAnimation)
                {
                    currentFireAnimation = "single";

                    animator.SetBool("isFire", shootController.isFire);
                    animator.SetFloat("fireValue", 0);
                    animator.SetFloat("fireRate", 1);

                    endFireSession = false;
                    endFireAnimation = false;

                    StartCoroutine(ApllyFireRateAnimation());
                    StartCoroutine(EndSingleFireAnimation());
                }
                else if (Input.GetMouseButton(0) && inputController.fireValue == 2)
                {
                    currentFireAnimation = "auto";

                    animator.SetBool("isFire", shootController.isFire);
                    animator.SetFloat("fireValue", 1);
                    animator.SetFloat("fireRate", shootController.fireRate / 50);
                }
            }

            if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButton(0) && (inputController.fireValue == 0))
            {
                currentFireAnimation = "none";
                animator.SetBool("isFire", false);
                endFireAnimation = true;
            }
        }
        #endregion

        #region Update Animator Value
        #endregion
    }

    public IEnumerator ApllyFireRateAnimation()
    {
        yield return new WaitForSeconds(1/ shootController.fireRate);
        endFireSession = true;
    }
    
    public IEnumerator EndSingleFireAnimation()
    {
        yield return new WaitForSeconds(timeEndFireAnimation);
        if (inputController.fireValue > 0)
        {
            endFireAnimation = false;
            animator.SetBool("isFire", true && shootController.isFire);
        }
        else
        {
            endFireAnimation = true;
            animator.SetBool("isFire", false);
        }
    }

    public void SetJumpAnimationParameter(bool isGrounded, bool isJumporFloat, float jumpProcessValue, float timeHoldAnimation)
    {
        animator.SetBool("isJumporFloat", isJumporFloat);
        animator.SetFloat("jumpProcessValue", jumpProcessValue);
    }

    public IEnumerator HoldFloatJumpAnimation(float timeHoldAnimation)
    {
        yield return new WaitForSeconds(timeHoldAnimation);
        if (!movementController.isGrounded)
        {
            if (animator.GetFloat("jumpProcessValue") == 1)
            {
                animator.SetFloat("jumpProcessValue", 0);
            }
        }
        else
        {
            animator.SetBool("isJumporFloat", false);
        }
    }

    public void SetSpeedMultiplierAnimator(float speed)
    {
        animator.SetFloat("multiplier", speed);
    }
    
    private void Test()
    {
        //variableTestInformations[0] = endFirstBullet.ToString();
        //variableTestInformations[1] = endFireAnimation.ToString();
        //variableTestInformations[2] = currentFireAnimation.ToString();
    }

    #region
    //void UpdateAnimationState()
    //{
    //    inTransition = animator.IsInTransition(0);
    //    inIdle = state.IsName("Idle");
    //    inWalk = state.IsName("Walk");
    //    inJump = state.IsName("Jump");
    //}

    //IEnumerator TransitionAfterJump()
    //{
    //    yield return new WaitForSeconds(timeOfPreLanding);
    //    animator.SetBool("isJump", inputController.isJump);
    //}
    #endregion
}
