using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVehicleController : MonoBehaviour
{
    public GameObject targetObject;
    private float distanceToTarget;

    public GameObject debuggingSphere01;

    private float appliedMotorTorque;
    private float appliedBrakeTorque;

    private float appliedTurnAngle;

    [Header("Wheel Colliders and Rigid Body")]
    public WheelCollider wc_fl;
    public WheelCollider wc_fr;
    public WheelCollider wc_rl;
    public WheelCollider wc_rr;
    public Rigidbody aiCarBody;

    // Update is called once per frame
    void Update()
    {
       

        distanceToTarget = Vector3.Distance(targetObject.transform.position, transform.position);
        if (distanceToTarget < 5)
        {
            appliedBrakeTorque = 1000f;
            appliedMotorTorque = 0f;
        }
        else 
        {
            appliedBrakeTorque = 0;
            Vector3 targetDirection = (targetObject.transform.position - transform.position).normalized;
            float dotProduct = Vector3.Dot(transform.forward, targetDirection);

            if(dotProduct > 0)
            {
                appliedMotorTorque = 700;
            }
            
            else 
            {
                if(distanceToTarget > 7) 
                {
                    appliedMotorTorque = 700;
                }
                else
                {
                    appliedMotorTorque = -700;
                }
            }

            float targetAngle = Vector3.SignedAngle(transform.forward, targetDirection, Vector3.up);

            float targetAngleRadians = targetAngle * Mathf.Deg2Rad;

            appliedTurnAngle = Mathf.Sin(targetAngleRadians) * 25;
        }
    }

    private void FixedUpdate()
    {
        wc_fl.motorTorque = appliedMotorTorque;
        wc_fl.brakeTorque = appliedBrakeTorque;
        wc_fl.steerAngle = appliedTurnAngle;

        wc_fr.motorTorque = appliedMotorTorque;
        wc_fr.brakeTorque = appliedBrakeTorque;
        wc_fr.steerAngle = appliedTurnAngle;
    }
}
