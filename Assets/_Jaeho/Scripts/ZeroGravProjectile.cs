using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroGravProjectile : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float targetSpeed = 2f;
    [SerializeField] TennisEnemy enemy;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void ChangeVelocity(Vector3 newVelocity, bool isPlayer = false)
    {
        newVelocity.z = Mathf.Clamp(newVelocity.z, 1.5f, 5f);
        newVelocity.y = Mathf.Clamp(newVelocity.z, -2f, 2f);

        rb.velocity = newVelocity;



        if (isPlayer)
        {
            float multiplier = targetSpeed / newVelocity.magnitude;
            multiplier = Mathf.Clamp(multiplier, 0.1f, 2f);
            enemy.moveSpeedMultiplier = multiplier;
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.collider.gameObject;
        Debug.Log("COLLIDED!");
        if (other.CompareTag("EnemyPointZone"))
        {
            ScoreManager.Instance.playerPoint();
            StartCoroutine(FlashGreen(other.gameObject));
        }
        if (other.CompareTag("PlayerPointZone"))
        {
            ScoreManager.Instance.enemyPoint();
            StartCoroutine(FlashGreen(other.gameObject));
        }


    }

        IEnumerator FlashGreen(GameObject obj) {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.material.color;
            renderer.material.color = Color.green;

            yield return new WaitForSeconds(1f);

            renderer.material.color = originalColor;
        }
    }

}
