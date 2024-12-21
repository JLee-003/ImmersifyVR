using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBiasedMovement : MonoBehaviour
{
    public Vector3 Position;
    public Vector3 Velocity;
    public Rigidbody fish;
    public float MaxSpeed = 4.0f;
    public float AttractionStrength = 1.3f;
    public float AvoidanceStrength = 1.5f;
    public float RandomDirectionStrength = 2.0f;
    public float AvoidRadius = 5.0f;
    public float MaxY = 15.0f;
    public Vector3 avoidance_force;
    public Vector3 attraction_force;
    public GameObject target;
    private float timer;
    public float changeDirectionTimer = 4.0f;
    Vector3 target_pos;

    void Start()
    {
        fish = GetComponent<Rigidbody>();
        fish.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        fish.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public void Update()
    { 

        timer += Time.deltaTime;    
        target_pos = target.transform.position;
        Position = fish.position;
        if (fish.position.y >= MaxY && Velocity.y > 0) {
                Velocity.y = -Mathf.Abs(Velocity.y);
            }
        

        if (timer >= changeDirectionTimer) {
            avoidance_force = GetAvoidanceForce();
            attraction_force = GetAttractiveForce();
            Vector3 rand_movement = GetRandomDirection(-0.25f, 0.25f);


            Velocity += (attraction_force + rand_movement + avoidance_force);
            
            limitVelocity();
            if (fish.position.y >= MaxY && Velocity.y > 0) {
                Velocity.y = -Mathf.Abs(Velocity.y);
            }
            
            timer = 0;
        }
    }

    public void FixedUpdate() {
        fish.AddForce(Velocity, ForceMode.Force);
        if (Velocity != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Velocity.normalized, Vector3.up);
            fish.rotation = Quaternion.Lerp(fish.rotation, targetRotation, Time.fixedDeltaTime * 5.0f);
        }

        if (fish.position.y >= MaxY && Velocity.y > 0)
        {
            Velocity.y = -Mathf.Abs(Velocity.y);
        }
    }


    Vector3 GetRandomDirection(float min, float max)
    {
        float randX = Random.Range(min, max);
        float randY = Random.Range(min, max);
        float randZ = Random.Range(min, max);

        return new Vector3(randX, randY, randZ).normalized * RandomDirectionStrength;
    }

    Vector3 GetAttractiveForce()
    {
        Vector3 dirToTarget = target_pos - Position;
        float distanceToTarget = dirToTarget.magnitude;

        if (distanceToTarget > AvoidRadius) {
            return dirToTarget.normalized * AttractionStrength;
        } 
        
        return Vector3.zero;
    }

    Vector3 GetAvoidanceForce()
    {
        Vector3 dirToTarget = target_pos - Position;
        float distanceToTarget = dirToTarget.magnitude;

        if (distanceToTarget <= AvoidRadius && distanceToTarget > 0.001f)
        {
            Vector3 avoidanceForce = -dirToTarget.normalized * AvoidanceStrength;

            //float scaleFactor = Mathf.Clamp01(1.0f - (distanceToTarget / AvoidRadius)); 
            //avoidanceForce *= scaleFactor;

            return avoidanceForce;
        }

        return Vector3.zero;
    }

   void limitVelocity()
    {
        Vector3 combinedVelocity = fish.velocity + Velocity;
        if (combinedVelocity.magnitude > MaxSpeed)
        {
            Velocity = combinedVelocity.normalized * MaxSpeed - fish.velocity;
        }
    }

    void ClampPositionAndVelocity()
    {
        Vector3 clampedPosition = fish.position;

        clampedPosition.y = Mathf.Clamp(clampedPosition.y, 0.0f, MaxY);

        fish.position = clampedPosition;

        if (fish.position.y >= MaxY && Velocity.y > 0)
        {
            Velocity.y = 0; 
        }
        else if (fish.position.y <= 0.0f && Velocity.y < 0)
        {
            Velocity.y = 0; 
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        fish.velocity *= 0.25f;
        Vector3 collisionNormal = collision.GetContact(0).normal;
        fish.AddForce(collisionNormal * 2.0f, ForceMode.Impulse);
    }
}
