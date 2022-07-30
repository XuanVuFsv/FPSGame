using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera playerCamera;
    public Cinemachine.CinemachinePOV playerAiming;  
    public Cinemachine.CinemachineImpulseSource cameraShake;
    public Animator rigController;
    public Vector2[] recoilPattern;

    public float duration;
    public float yAxisValue;

    float horizontalRecoil, verticalRecoil;
    float time;
    public int index = 0;

    private void Awake()
    {
        cameraShake = GetComponent<Cinemachine.CinemachineImpulseSource>();
        playerAiming = playerCamera.GetCinemachineComponent<Cinemachine.CinemachinePOV>();
    }

    // Update is called once per frame
    void Update()
    {
        yAxisValue = playerAiming.m_VerticalAxis.Value;
        if (rigController)
        {
            rigController.SetBool("isAttack", InputController.Instance.isFire);
        }

        if (time > 0)
        {
            playerAiming.m_HorizontalAxis.Value -= horizontalRecoil * Time.deltaTime / duration;
            playerAiming.m_VerticalAxis.Value -= verticalRecoil * Time.deltaTime / duration;
            time -= Time.deltaTime;
        }
    }

    int NextIndex()
    {
        return ((index + 1) % recoilPattern.Length);
    }
        
    public void ResetRecoil(string weaponName)
    {
        index = 0;
    }

    public void GenerateRecoil(string weaponName)
    {
        time = duration;

        cameraShake.GenerateImpulse(Camera.main.transform.forward);

        horizontalRecoil = recoilPattern[index].x;
        verticalRecoil = recoilPattern[index].y;

        index = NextIndex();

        rigController.Play("Recoil " + weaponName, 1, 0.0f);
    }

    public void SetUpWeaponRecoilForNewWeapon(Cinemachine.CinemachineVirtualCamera newCamera, Animator newRigController)
    {
        playerCamera = newCamera;
        rigController = newRigController;
    }
}
