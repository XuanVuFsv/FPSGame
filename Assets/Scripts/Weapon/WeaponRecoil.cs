using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    //[HideInInspector]
    public Cinemachine.CinemachineVirtualCamera playerCamera;
    public Cinemachine.CinemachinePOV playerAiming;
    
    //[HideInInspector]
    public Cinemachine.CinemachineImpulseSource cameraShake;

    public float verticalRecoil;
    public float duration;
    public float yAxisValue;

    float time;

    private void Awake()
    {
        cameraShake = GetComponent<Cinemachine.CinemachineImpulseSource>();
        playerAiming = playerCamera.GetCinemachineComponent<Cinemachine.CinemachinePOV>();
    }

    // Update is called once per frame
    void Update()
    {
        yAxisValue = playerAiming.m_VerticalAxis.Value;

        if (time > 0)
        {
            playerAiming.m_VerticalAxis.Value -= verticalRecoil;
            time -= Time.deltaTime;
        }
    }

    public void GenerateRecoil()
    {
        time = duration;

        cameraShake.GenerateImpulse(Camera.main.transform.forward);
    }
}
