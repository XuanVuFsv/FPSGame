using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public List<PhysicMaterial> physicMaterials;
    public Vector3 nextPosition;
    public Quaternion nextRotation;
    public LayerMask groundCheckLayers = -1;
    public float moveSpeed, walkSpeed, onAimOrFireSpeed, jumpForce;
    public float pauseJumpTime;
    public float groundCheckDistance = 0.05f;
    public bool onGrounded, onAir;
    public bool isReadyToJump = true, allowJump = true;
    public bool IsGrounded { get; private set; }
    public bool HasJumpedThisFrame { get; private set; }

    [SerializeField]
    Transform followTargetTransform;
    [SerializeField]
    new Rigidbody rigidbody;
    [SerializeField]
    Vector3 movementVector;
    [SerializeField]
    Vector3 turnSpeed;
    [SerializeField]
    Vector2 limitAngleY; // properties: x-min y-max
    [SerializeField]
    float rotationLerp = 0.5f;
    [SerializeField]
    string collisonCollider;

    InputController inputController;
    CapsuleCollider capsuleCollider;
    MainCharacterAnimator mainCharacterAnimator;
    CameraController cameraController;
    Vector3 m_GroundNormal;

    const float k_JumpGroundingPreventionTime = 0.2f;
    const float k_GroundCheckDistanceInAir = 0.07f;

    #region Test
    public GameObject cubeRigidbody;
    public int forceTest;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        inputController = GetComponent<InputController>();
        mainCharacterAnimator = GetComponent<MainCharacterAnimator>();
        cameraController = GetComponent<CameraController>();
        capsuleCollider.material = physicMaterials[0];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        //GroundCheck();
        //UpdateFollowTarget();
        //AdjustAnimationByVelocity();
        //ControlCamera();
        //ModifyOnFire();
        if (inputController.isJump && isReadyToJump && allowJump)
        {
            if (onGrounded)
            {
                StartCoroutine(Jump(1));
            }
            else if (!onGrounded && rigidbody.velocity.y <= -5)
            {
                StartCoroutine(Jump(0));
            }
        }

        // jumping
        if (IsGrounded && inputController.isJump)
        {
            // force the crouch state to false
            //if (SetCrouchingState(false, false))
            //{
            // start by canceling out the vertical component of our velocity
            CharacterVelocity = new Vector3(CharacterVelocity.x, 0f, CharacterVelocity.z);

            // then, add the jumpSpeed value upwards
            CharacterVelocity += Vector3.up * JumpForce;

            // play sound
            AudioSource.PlayOneShot(JumpSfx);

            // remember last time we jumped because we need to prevent snapping to ground for a short time
            m_LastTimeJumped = Time.time;
            HasJumpedThisFrame = true;

            // Force grounding to false
            IsGrounded = false;
            m_GroundNormal = Vector3.up;
            //}
        }

        if (!inputController.isWalk)
        {
            capsuleCollider.material = physicMaterials[0];
        }
        else
        {
            capsuleCollider.material = physicMaterials[1];
        }

        #region ExecuteTest
        //cubeRigidbody.transform.Rotate(Vector3.forward * forceTest);
        #endregion
    }

    //void ModifyOnFire()
    //{
    //    //var eulerAngles = body.localEulerAngles;
    //    if (inputController.in && !isRotateOnFire)
    //    {
    //        //eulerAngles.y = 35;
    //        body.Rotate(0, -17, 0);
    //        isRotateOnFire = true;

    //    }
    //    else if (!inputController.isManipulationFire && isRotateOnFire)
    //    {
    //        //eulerAngles.y = 18;
    //        body.Rotate(0, 17, 0);
    //        isRotateOnFire = false;
    //    }
    //    //body.localEulerAngles = eulerAngles;
    //}

    //void UpdateFollowTarget()
    //{
    //    #region Follow Transform Rotation

    //    //Rotate the Follow Target transform based on the input
    //    followTargetTransform.transform.rotation *= Quaternion.AngleAxis(inputController.look.x * turnSpeed.y, Vector3.up);

    //    #endregion

    //    #region Vertical Rotation
    //    followTargetTransform.transform.rotation *= Quaternion.AngleAxis(inputController.look.y * turnSpeed.x, Vector3.right);

    //    var angles = followTargetTransform.transform.localEulerAngles;
    //    angles.z = 0;

    //    var angle = followTargetTransform.transform.localEulerAngles.x;

    //    //Clamp the Up/Down rotation
    //    if (angle > 180 && angle < limitAngleY.y)
    //    {
    //        angles.x = limitAngleY.y;
    //    }
    //    else if (angle < 180 && angle > limitAngleY.x)
    //    {
    //        angles.x = limitAngleY.x;
    //    }

    //    followTargetTransform.transform.localEulerAngles = angles;
    //    #endregion

    //    nextRotation = Quaternion.Lerp(followTargetTransform.transform.rotation, nextRotation, Time.deltaTime * rotationLerp);

    //    if (inputController.move.x == 0 && inputController.move.y == 0)
    //    {

    //        //if (aimValue == 1)
    //        //{
    //        //    //Set the player rotation based on the look transform
    //        //    transform.rotation = Quaternion.Euler(0, followTransform.transform.rotation.eulerAngles.y, 0);
    //        //    //reset the y rotation of the look transform
    //        //    followTransform.transform.localEulerAngles = new Vector3(angles.x, 0, 0);
    //        //}

    //        return;
    //    }

    //    //Set the player rotation based on the look transform
    //    transform.rotation = Quaternion.Euler(0, followTargetTransform.transform.rotation.eulerAngles.y, 0);
    //    //reset the y rotation of the look transform
    //    followTargetTransform.transform.localEulerAngles = new Vector3(angles.x, 0, 0);
    //}

    //void ControlCamera()
    //{
    //    //transform.Rotate(Vector3.up, inputController.mouseX * turnSpeed.y * Time.deltaTime);
    //    //float changeHeadDistance = (headCharacter.transform.localPosition.y + inputController.mouseY * turnSpeed.x * Time.deltaTime);
    //    //headCharacter.transform.localPosition = new Vector3(headCharacter.transform.localPosition.x, Mathf.Clamp(changeHeadDistance, limitPositionY.x, limitPositionY.y), headCharacter.transform.localPosition.z);
    //}

    //private void AdjustAnimationByVelocity()
    //{
    //    if (rigidbody.velocity.y <= -2f && onAir)
    //    {
    //        //mainCharacterAnimator.SetSpeedMultiplierAnimator(0);
    //        //Debug.Log(rigidbody.velocity.y);
    //        Time.timeScale = 0;
    //    }
    //    else
    //    {
    //        //mainCharacterAnimator.SetSpeedMultiplierAnimator(1);
    //    }
    //}

    void Move()
    {
        movementVector = (transform.forward * inputController.vertical + transform.right * inputController.horizontal).normalized;

        if ((inputController.vertical > 0) && !inputController.isAim)
        {
            //rigidbody.MovePosition(rigidbody.position + movementVector * speedMove * Time.deltaTime);
            moveSpeed = walkSpeed;
        }
        else
        {
            //rigidbody.MovePosition(rigidbody.position + movementVector * speedMove * 0.5f * Time.deltaTime);
            moveSpeed = onAimOrFireSpeed;
        }

        rigidbody.MovePosition(rigidbody.position + movementVector * moveSpeed * Time.deltaTime);
    }

    //void GroundCheck()
    //{
    //    // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
    //    float chosenGroundCheckDistance =
    //        IsGrounded ? (m_Controller.skinWidth + GroundCheckDistance) : k_GroundCheckDistanceInAir;

    //    // reset values before the ground check
    //    IsGrounded = false;
    //    m_GroundNormal = Vector3.up;

    //    // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
    //    if (Time.time >= m_LastTimeJumped + k_JumpGroundingPreventionTime)
    //    {
    //        // if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
    //        if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(m_Controller.height),
    //            m_Controller.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, GroundCheckLayers,
    //            QueryTriggerInteraction.Ignore))
    //        {
    //            // storing the upward direction for the surface found
    //            m_GroundNormal = hit.normal;
    //            Debug.LogFormat("m_GroundNormal: {0} up: {1} Dot: {2} Angle: {3}", hit.point, transform.up, Vector3.Dot(hit.normal, transform.up), Vector3.Angle(hit.normal, transform.up));

    //            // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
    //            // and if the slope angle is lower than the character controller's limit

    //            if (Vector3.Dot(hit.normal, transform.up) > 0f &&
    //                IsNormalUnderSlopeLimit(m_GroundNormal))
    //            {
    //                IsGrounded = true;

    //                // handle snapping to the ground
    //                if (hit.distance > m_Controller.skinWidth)
    //                {
    //                    m_Controller.Move(Vector3.down * hit.distance);
    //                }
    //            }
    //        }
    //    }
    //}

    //// Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
    //bool IsNormalUnderSlopeLimit(Vector3 normal)
    //{
    //    return Vector3.Angle(transform.up, normal) <= m_Controller.slopeLimit;
    //}

    //// Gets the center point of the bottom hemisphere of the character controller capsule    
    //Vector3 GetCapsuleBottomHemisphere()
    //{
    //    bot = transform.position + (transform.up * m_Controller.radius);
    //    return transform.position + (transform.up * m_Controller.radius);
    //}

    //// Gets the center point of the top hemisphere of the character controller capsule    
    //Vector3 GetCapsuleTopHemisphere(float atHeight)
    //{
    //    top = transform.position + (transform.up * (atHeight - m_Controller.radius));
    //    return transform.position + (transform.up * (atHeight - m_Controller.radius));
    //}

    IEnumerator Jump(float jumpValue)
    {
        isReadyToJump = false;
        allowJump = false;
        onGrounded = false;
        onAir = true;
        mainCharacterAnimator.SetJumpAnimationParameter(true, jumpValue, 0.533f);
        StartCoroutine(PauseJump());
        yield return new WaitForSeconds(0.533f * 0.57f); //play a part of animation before jump 1.2f / 36 * 8
        rigidbody.AddForce(transform.up * jumpForce);
    }

    IEnumerator PauseJump()
    {
        yield return new WaitForSeconds(pauseJumpTime);
        isReadyToJump = true;
    }

    public void UpdateCapsuleCollider(bool isJump)
    {
        if (isJump)
        {
            onGrounded = false;
            onAir = true;
            return;
        }
        onGrounded = true;
        onAir = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        collisonCollider = collision.collider.name;
    }

    private void OnCollisionExit(Collision collision)
    {
        collisonCollider = "exit";
    }
}
