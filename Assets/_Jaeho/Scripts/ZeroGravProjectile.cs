using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroGravProjectile : MonoBehaviour
{
    Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void ChangeVelocity(Vector3 newVelocity)
    {
        rb.velocity = newVelocity;
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
