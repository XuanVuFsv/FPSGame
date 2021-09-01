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
    private float timeToChangeIdlePose = 60; //ms
    [SerializeField]
    private float timeOfPreLanding;
    [SerializeField]
    private bool inLongIdle = false;
    [SerializeField]
    private bool isEndChangeAnimationIdleCoroutine = true;

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
    private AnimatorStateInfo state;
    private IEnumerator changeIdleAnimationCorroutine;
    private bool inTransition, inIdle, inWalk, inJump;

    delegate void ControlIdleAnimationCoroutine(IEnumerator corountine);
    private ControlIdleAnimationCoroutine startCoroutine = null;

    #region Test
    //public int countParameters, countParametersAnimator;
    //public string[] variableTestInformations;
    //public string[] parameterNames, parameterInformations;
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

        {
            //animator = GetComponent<Animator>();
            //countParametersAnimator = animator.parameterCount;
            //animatorControllerParameters = new AnimatorControllerParameter[countParameters];
            //parameterNames = new string[countParametersAnimator];
            //parameterInformations = new string[countParametersAnimator];
            //countParameters = countParametersAnimator + variableTestInformations.Length;
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
        state = animator.GetCurrentAnimatorStateInfo(0);

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

                    //weaponAnimators[0].SetBool("isFire", true && shootController.isFire);
                    animator.SetBool("isFire", true && shootController.isFire);
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

                    //weaponAnimators[0].SetBool("isFire", true && shootController.isFire);
                    animator.SetBool("isFire", true && shootController.isFire);
                    animator.SetFloat("fireValue", 1);
                    animator.SetFloat("fireRate", shootController.fireRate / 50);
                }
            }

            if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButton(0) && (inputController.fireValue == 0))
            {
                currentFireAnimation = "none";
                animator.SetBool("isFire", false);
                //weaponAnimators[0].SetBool("isFire", false);
                endFireAnimation = true;
            }

            if (!Input.anyKey && !Input.anyKeyDown && !inLongIdle && isEndChangeAnimationIdleCoroutine)
            {
                startCoroutine += StartChangeIdleAnimationCorroutine;
                startCoroutine(ChangeIdleAnimation());
            }
            else if (Input.anyKeyDown || Input.anyKey)
            {
                startCoroutine = null;
                inLongIdle = false;
                animator.SetBool("isIdle2", false);
            }
        }

        #endregion

        #region Update Animator Value
        //for (int i = 0; i < countParametersAnimator; i++)
        //{
        //    parameterNames[i] = animator.GetParameter(i).name;

        //    if (animator.GetParameter(i).type == AnimatorControllerParameterType.Bool)
        //    {
        //        parameterInformations[i] = parameterNames[i] + ": " + animator.GetBool(parameterNames[i]).ToString();
        //    }
        //    else if (animator.GetParameter(i).type == AnimatorControllerParameterType.Float)
        //    {
        //        parameterInformations[i] = parameterNames[i] + ": " + animator.GetFloat(parameterNames[i]).ToString();
        //    }
        //    else if (animator.GetParameter(i).type == AnimatorControllerParameterType.Int)
        //    {
        //        parameterInformations[i] = parameterNames[i] + ": " + animator.GetInteger(parameterNames[i]).ToString();
        //    }
        //}
        #endregion
    }

    public IEnumerator ChangeIdleAnimation()
    {
        isEndChangeAnimationIdleCoroutine = false;
        inLongIdle = true;
        yield return new WaitForSeconds(timeToChangeIdlePose);
        if (!Input.anyKey && !Input.anyKeyDown && startCoroutine != null)
        {
            animator.SetBool("isIdle2", true);
        }
        isEndChangeAnimationIdleCoroutine = true;
    }

    void StartChangeIdleAnimationCorroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
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
            //weaponAnimators[0].SetBool("isFire", true && shootController.isFire);
        }
        else
        {
            endFireAnimation = true;
            animator.SetBool("isFire", false);
            //weaponAnimators[0].SetBool("isFire", false);
        }
        //if (!Input.GetMouseButton(0))
        //{
        //    animator.SetBool("isFire", false);
        //}
    }

    public void SetJumpAnimationParameter(bool isGrounded, bool isJumporFloat, float jumpProcessValue, float timeHoldAnimation)
    {
        animator.SetBool("isJumporFloat", isJumporFloat);
        animator.SetFloat("jumpProcessValue", jumpProcessValue);
        if (!isGrounded && isJumporFloat && timeHoldAnimation != 0) StartCoroutine(HoldFloatJumpAnimation(timeHoldAnimation));
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
