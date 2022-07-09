using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpChecker : MonoBehaviour
{
    public string footColliderName;
    public float distanceOfJumChecker;

    [SerializeField]
    GameObject rootGameObject;
    [SerializeField]
    float timeOfPreLanding;
    [SerializeField]
    string collisonCollider;

    MovementController movementController;
    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        movementController = rootGameObject.GetComponent<MovementController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.DrawRay(transform.position, -Vector3.up * distanceOfJumChecker, Color.green);
        //CheckOnGrounded();
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!movementController.onGround && !(other.name == footColliderName))
    //    {
    //        movementController.onGround = true;
    //        movementController.onAir = false;
    //    }
    //}
    //private void OnTriggerExit(Collider other)
    //{
    //    if (!(other.name == footColliderName))
    //    {
    //        movementController.onGround = false;
    //        movementController.onAir = true;    
    //    }
    //}

    //#region CheckOnGround
    //void CheckOnGround()
    //{
    //    if (Physics.Raycast(transform.position, -Vector3.up * distanceOfJumChecker, out hit, distanceOfJumChecker))
    //    {
    //        StartCoroutine(CheckPreLanding());
    //    }
    //}

    //IEnumerator CheckPreLanding()
    //{
    //    yield return new WaitForSeconds(timeOfPreLanding);
    //    if (Physics.Raycast(transform.position, -Vector3.up * distanceOfJumChecker, out hit, distanceOfJumChecker))
    //    {
    //        movementController.onGround = true;
    //        Debug.Log(hit.collider.name);
    //        movementController.onAir = false;
    //    }
    //}
    //#endregion
}