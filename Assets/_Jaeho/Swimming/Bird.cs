using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public float lifeTime = 10f;
    public float speed = 10f;
    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
        {
            Destroy(gameObject);
        }

        transform.position += transform.forward.normalized * speed * Time.deltaTime;
    }
}
