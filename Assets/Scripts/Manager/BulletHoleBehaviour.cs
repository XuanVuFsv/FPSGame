using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHoleBehaviour : MonoBehaviour
{
    public float lifeTime;
    public int index;

    private void OnEnable()
    {
        Invoke(nameof(OnDisable), lifeTime);
    }

    private void OnDisable()
    {
        if (index <= PoolingManager.Instance.indexReadyToUse) PoolingManager.Instance.indexReadyToUse = index;
        gameObject.SetActive(false);
    }

}
