using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class VehicleController : MonoBehaviour
{
    private float accelerationInput;
    private float currentTurnInput;
    private float targetTurnInput;
    private Vector3 currentVelocity;
    private bool isNitrousActive;

    public Rigidbody carBody;
    public float maxTurnAngle = 20f;
    public float carHorsePower = 400f;

    [Header("Nitrous Variables")]
    public float nitrousActiveDuration = 4f;
    public float nitrousRechargeTime = 10f;
    public float nirousRechargeDelay = 3f;
    public float nitrousTorque = 500f;

    private float currentNitrousCapacity = 1f;
    private float currentNitrousDelay = 0;
    private float currentNitrousTorque = 0;

    [Header("Wheel Colliders")]
    public WheelCollider wc_FrontLeft;
    public WheelCollider wc_FrontRight;
    public WheelCollider wc_BackLeft;
    public WheelCollider wc_BackRight;

    [Header("Effects")]
    public ParticleSystem RLWParticleSystem;
    public ParticleSystem RRWParticleSystem;
    public ParticleSystem RLWParticleSystemExost;
    public ParticleSystem RRWParticleSystemExost; 
   

    public TrailRenderer RLWTireSkid;
    public TrailRenderer RRWTireSkid;

    [Header("UI")]
    public Image cooldown;

    /*public void IANitrous(InputAction.CallbackContext context)
    {
        if(context.started == true)
        {
            isNitrousActive = true;
        }

        if(context.canceled == true)
        {
            isNitrousActive = false;
        }

    }*/

    void Update()
    {
        currentVelocity = carBody.velocity;

        accelerationInput = Input.GetAxis("Vertical");

        targetTurnInput = Input.GetAxis("Horizontal");

        isNitrousActive = Input.GetKey(KeyCode.Space);

        if(isNitrousActive == true)
        {
            if(currentNitrousCapacity > 0)
            {
                currentNitrousCapacity -= (Time.deltaTime/nitrousActiveDuration);
                currentNitrousTorque = nitrousTorque;
                RLWParticleSystem.Play();
                RRWParticleSystem.Play();

                cooldown.fillAmount -= (Time.deltaTime / nitrousActiveDuration);
                RLWTireSkid.emitting = true;
                RRWTireSkid.emitting = true;
              
            }
            // The following else executes if Nitrous was held or pressed when the Nitrous capacity was already empty
            else
            {
                
            }
        }
        else
        {
            if(currentNitrousCapacity < 1)
            {
                currentNitrousCapacity += (Time.deltaTime / nitrousRechargeTime);
                cooldown.fillAmount += (Time.deltaTime / nitrousActiveDuration);
                currentNitrousTorque = 0;
                RLWParticleSystem.Stop();
                RRWParticleSystem.Stop();
                RLWTireSkid.emitting = false;
                RRWTireSkid.emitting = false;
            }
        }

    }

    private void FixedUpdate()
    {
        Vector3 combinedInput = (transform.forward) * accelerationInput;
        float DotProduct = Vector3.Dot(currentVelocity.normalized, combinedInput);

        if( DotProduct < 0 && accelerationInput !=0)
        {
            wc_FrontLeft.brakeTorque = 1000f;
            wc_FrontRight.brakeTorque = 1000f;
            wc_BackLeft.brakeTorque = 1000f;
            wc_BackRight.brakeTorque = 1000f;

            wc_BackLeft.motorTorque = 0;
            wc_BackRight.motorTorque = 0;
            wc_FrontLeft.motorTorque = 0;
            wc_FrontRight.motorTorque = 0;

            if (accelerationInput < 0)
            {
                RLWParticleSystemExost.Stop();
                RRWParticleSystemExost.Stop();
               
            }
        }
        else 
        {
            wc_FrontLeft.brakeTorque = 0;
            wc_FrontRight.brakeTorque = 0;
            wc_BackLeft.brakeTorque = 0;
            wc_BackRight.brakeTorque = 0;

            if (accelerationInput > 0)
            {
                RLWParticleSystemExost.Play();
                RRWParticleSystemExost.Play();
              
            }

    wc_BackLeft.motorTorque = ((accelerationInput * carHorsePower) + currentNitrousTorque) ;
            wc_BackRight.motorTorque = ((accelerationInput * carHorsePower) + currentNitrousTorque);
        }

        currentTurnInput = ApproachTargetValueWithIncrement(currentTurnInput, targetTurnInput, 0.07f);
        wc_FrontLeft.steerAngle = currentTurnInput * maxTurnAngle;
        wc_FrontRight.steerAngle = currentTurnInput * maxTurnAngle;
    }

    private float ApproachTargetValueWithIncrement(float currentValue, float targetValue, float increment)
    {
        if(currentValue == targetValue)
        {
            return currentValue;
        }
        else
        {
            if(currentValue < targetValue)
            {
                currentValue = currentValue + increment;
            }
            else
            {
                currentValue = currentValue - increment;
            }
        }
        return currentValue;
    }
}
