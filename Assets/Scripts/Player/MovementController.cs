using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    //public List<PhysicMaterial> physicMaterials;
    public LayerMask groundCheckLayers = -1;
    public float runSpeed, walkSpeed, onAimOrFireSpeed, jumpForce;
    public float jumpDelay;
    public float groundCheckDistance = 0.05f;
    public float skinWidth = 0.02f;
    public float slopeLimit;
    public float currentSlope;
    public bool readyToJump = true, allowJump = true;
    public bool isGrounded;

    private Transform followTargetTransform;
    [SerializeField]
    private new Rigidbody rigidbody;
    [SerializeField]
    private Vector3 movementVector;

    private InputController inputController;
    private CapsuleCollider capsuleCollider;
    private MainCharacterAnimator mainCharacterAnimator;
    private Vector3 m_GroundNormal;

    public float k_GroundCheckDistanceInAir = 0.07f;

    #region Test
    //public GameObject cubeRigidbody;
    //public int forceTest;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        inputController = GetComponent<InputController>();
        mainCharacterAnimator = GetComponent<MainCharacterAnimator>();
    }

    private void Update()
    {
        GroundCheck();

        // jumping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Jump");
            if (isGrounded)
            {
                isGrounded = false;
                m_GroundNormal = Vector3.up;
                Jump(1);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();

        #region ExecuteTest
        //cubeRigidbody.transform.Rotate(Vector3.forward * forceTest);
        #endregion
    }

    void Move()
    {
        movementVector = (transform.forward * inputController.rawVertical + transform.right * inputController.rawHorizontal).normalized;

        if ((inputController.rawVertical > 0) && !inputController.isAim)
        {
            runSpeed = walkSpeed;
        }
        else
        {
            runSpeed = onAimOrFireSpeed;
        }

        rigidbody.MovePosition(rigidbody.position + movementVector * runSpeed * Time.deltaTime);
    }

    void GroundCheck()
    {
        // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
        float chosenGroundCheckDistance =
            isGrounded ? (skinWidth + groundCheckDistance) : k_GroundCheckDistanceInAir;

        m_GroundNormal = Vector3.up;

        // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
        if (true)
        {
            {
                //Debug.LogFormat("GetCapsuleBottomHemisphere: {0} GetCapsuleTopHemisphere: {1} height: {2}", GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(capsuleCollider.height), capsuleCollider.height);
                //Debug.LogFormat("down: {0} chosenGroundCheckDistance: {1}", Vector3.down, chosenGroundCheckDistance);
            }

            // if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
            if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(capsuleCollider.height),
                capsuleCollider.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, groundCheckLayers,
                QueryTriggerInteraction.Ignore) || (currentSlope <= slopeLimit && Physics.Raycast(transform.position, Vector3.down, out hit, 0.05f, groundCheckLayers)))
            {
                // storing the upward direction for the surface found
                m_GroundNormal = hit.normal;

                {
                    //Debug.Log(hit.collider.name);
                    //Debug.LogFormat("m_GroundNormal: {0} up: {1} Dot: {2} Angle: {3}", hit.point, transform.up, Vector3.Dot(hit.normal, transform.up), Vector3.Angle(transform.up, hit.normal));
                }

                // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                // and if the slope angle is lower than the character controller's limit
                if (Vector3.Dot(hit.normal, transform.up) > 0f &&
                    IsNormalUnderSlopeLimit(m_GroundNormal))
                {
                    isGrounded = true;
                    if (mainCharacterAnimator.animator.GetBool("isJumporFloat"))
                    {
                        mainCharacterAnimator.SetJumpAnimationParameter(isGrounded, false, 0, 0);
                    }
                }
                else if(!IsNormalUnderSlopeLimit(m_GroundNormal))
                {
                    isGrounded = true;
                    //Debug.Log("Not same direction or slope");
                }
            }
            else
            {
                isGrounded = false;
                if (!mainCharacterAnimator.animator.GetBool("isJumporFloat"))
                {
                    //Debug.Log("Falling");
                    mainCharacterAnimator.SetJumpAnimationParameter(isGrounded, true, 0, 0);
                }
                //Debug.Log("Not collide ground");
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(GetCapsuleBottomHemisphere(), capsuleCollider.radius);
    //    Gizmos.DrawWireSphere(GetCapsuleTopHemisphere(capsuleCollider.height), capsuleCollider.radius);
    //}

    // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
    bool IsNormalUnderSlopeLimit(Vector3 normal)
    {
        currentSlope = Vector3.Angle(transform.up, normal);
        return Vector3.Angle(transform.up, normal) <= slopeLimit;
    }

    // Gets the center point of the bottom hemisphere of the character controller capsule    
    Vector3 GetCapsuleBottomHemisphere()
    {
        return transform.position + (transform.up * capsuleCollider.radius);
    }

    // Gets the center point of the top hemisphere of the character controller capsule    
    Vector3 GetCapsuleTopHemisphere(float atHeight)
    {
        return transform.position + (transform.up * (atHeight - capsuleCollider.radius));
    }

    void Jump(float jumpProcessValue)
    {
        //readyToJump = false;
        mainCharacterAnimator.SetJumpAnimationParameter(isGrounded, true, jumpProcessValue, 0.5f);
        //StartCoroutine(DelayJump());
        readyToJump = true;
        //yield return new WaitForSeconds((18 / 31) * (16 / 3)); //play a part of animation before jump t = 8/15 * 0.533 8/15 frame length = 0.533s
        rigidbody.AddForce(transform.up * jumpForce);
    }
}
