using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField]
    InputController inputController;

    [SerializeField]
    float steeringAngle;
    [SerializeField]
    float maxSteeringAngle;
    [SerializeField]
    float motorForce, motorTorque, brakeForce;

    public WheelCollider flWheelCollider, frWheelCollider, rlWheelCollider, rrWheelCollider;
    public Transform flWheelTransform, frWheelTransform, rlWheelTransform, rrWheelTransform;

    private Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        inputController = GetComponent<InputController>();
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Steer();
        Accelerate();
        UpdateWheelPoses();
    }

    void Steer()
    {
        steeringAngle = inputController.horizontal * maxSteeringAngle;
        flWheelCollider.steerAngle = frWheelCollider.steerAngle = steeringAngle;
    }

    void Accelerate()
    {
        motorTorque = inputController.vertical * motorForce;
        flWheelCollider.motorTorque = motorTorque;
        frWheelCollider.motorTorque = motorTorque;
        if (inputController.isBrake || motorTorque == 0)
        {
            Brake(true);
        }
        else
        {
            Brake(false);
        }
    }

    void Brake(bool isBrake)
    {
        if (isBrake)
        {

            if (motorTorque == 0 && rigidbody.velocity.magnitude > 0)
            {
                //rlWheelCollider.brakeTorque = brakeForce / 2;
                //rrWheelCollider.brakeTorque = brakeForce / 2;
                flWheelCollider.brakeTorque = brakeForce / 2;
                frWheelCollider.brakeTorque = brakeForce / 2;
                return;
            }
            flWheelCollider.brakeTorque = brakeForce;
            frWheelCollider.brakeTorque = brakeForce;
            return;
        }
        flWheelCollider.brakeTorque = 0;
        frWheelCollider.brakeTorque = 0;
        //rlWheelCollider.brakeTorque = 0;
        //rrWheelCollider.brakeTorque = 0;
    }

    void UpdateWheelPoses()
    {
        UpdateWheelPose(flWheelTransform, flWheelCollider);
        UpdateWheelPose(frWheelTransform, frWheelCollider);
        UpdateWheelPose(rlWheelTransform, rlWheelCollider);
        UpdateWheelPose(rrWheelTransform, rrWheelCollider);
    }

    void UpdateWheelPose(Transform _transform, WheelCollider _wheelCollider)
    {
        Vector3 position = _transform.position;
        Quaternion quaternion = _transform.rotation;

        _wheelCollider.GetWorldPose(out position, out quaternion);

        _transform.position = position;
        _transform.rotation = quaternion;
    }
}
