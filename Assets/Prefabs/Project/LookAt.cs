using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform obj;
    // Start is called before the first frame update
    void Update()
    {
        gameObject.transform.LookAt(obj);
    }
}
