using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTransform : MonoBehaviour
{
    public Transform horizontalTransform;
    public Transform verticalTransform;

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos= new Vector3(horizontalTransform.position.x, verticalTransform.position.y, horizontalTransform.position.z);
        TransformFunctionality.MakeTwoTransformEqual(newPos, transform);    
    }
}
