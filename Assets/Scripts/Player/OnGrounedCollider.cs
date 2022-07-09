using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGrounedCollider : MonoBehaviour
{
    public float distanceOfJumChecker;

    [SerializeField]
    GameObject rootGameObject;
    [SerializeField]
    float timeOfPreLanding;
    [SerializeField]
    string triggerCollider;

    MovementController movementController;
    MainCharacterAnimator mainCharacterAnimator;
    SkinnedMeshRenderer skinnedMesh;
    MeshCollider meshCollider;

    // Start is called before the first frame update
    void Start()
    {
        movementController = rootGameObject.GetComponent<MovementController>();
        mainCharacterAnimator = rootGameObject.GetComponent<MainCharacterAnimator>();
    }

    //private void OnTriggerStay(Collider collider)
    //{
    //    triggerCollider = collider.name;
    //    if (!movementController.onGround)
    //    {
    //        movementController.UpdateCapsuleCollider(false);
    //    }
    //}

    //private void OnTriggerExit(Collider collider)
    //{
    //    triggerCollider = "exit";
    //    movementController.UpdateCapsuleCollider(true);
    //}

    //private void OnTriggerEnter(Collider collider)
    //{
    //    movementController.allowJump = true;
    //    if (mainCharacterAnimator.animator.GetBool("isJump") && mainCharacterAnimator.animator.GetFloat("jumpValue") == 0)
    //    {
    //        //mainCharacterAnimator.SetJumpAnimationParameter(false, 1, 0);
    //    }
    //}

    //private void UpdateMeshCollider()
    //{
    //    Mesh mesh = new Mesh();
    //    skinnedMesh.BakeMesh(mesh);
    //    meshCollider.sharedMesh = null;
    //    meshCollider.sharedMesh = mesh;
    //}
}