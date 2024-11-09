using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBiasedMovement : MonoBehaviour
{
    public Vector3 Position;
    public Vector3 Velocity;
    public Rigidbody fish;
    public float MaxSpeed = 3.0f;
    public float AttractionStrength = 0.1f;
    public GameObject target;

    void Start()
    {
        fish = GetComponent<Rigidbody>();
    }

    public void Update()
    { 
        Vector3 target_pos = target.transform.position;
        Position = fish.position;
        Vector3 attraction_force = GetAttractiveForce(target_pos);
        Vector3 rand_movement = GetRandomDirection(-0.1f, 0.1f);

        Velocity += attraction_force + rand_movement;
        limitVelocity();
        fish.AddForce(Velocity, ForceMode.Impulse);
    }


    Vector3 GetRandomDirection(float min, float max)
    {
        float randX = Random.Range(min, max);
        float randY = Random.Range(min, max);
        float randZ = Random.Range(min, max);

        return new Vector3(randX, randY, randZ).normalized;
    }

     Vector3 GetAttractiveForce(Vector3 v)
    {
        Vector3 dirToTarget = v - Position;
        dirToTarget = Vector3.Normalize(dirToTarget);
        return dirToTarget * AttractionStrength;
    }

    void limitVelocity()
    {
        if (Velocity.magnitude > MaxSpeed)
        {
            Velocity = Vector3.Normalize(Velocity) * MaxSpeed;
        }
    }

    //private void checkBoundary()
    //{
    //    float xBoundary;
    //    float yBoundary;
    //    float zBoundary;

    //    if (Position.X > xBoundary - 10 || Position.X < xBoundary - 10) Velocity.X = -Velocity.X;
    //    if (Position.Y > yBoundary - 10 || Position.Y < yBoundary - 10) Velocity.Y = -Velocity.Y;
    //    if (Position.Z > zBoundary - 10 || Position.Z < zBoundary - 10) Velocity.Z = -Velocity.Zw;
    //}
}
