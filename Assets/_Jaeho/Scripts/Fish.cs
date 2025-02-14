using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    MeshFilter meshFilter;
    //Renderer objRenderer;
    public Mesh mesh1, mesh2;
    public int value1, value2;
    [SerializeField] AudioClip removeAudio;

    //public Material mat1, mat2;

    public int type;

    int value;

    private void Start()
    {
        meshFilter = GetComponentInChildren<MeshFilter>();
        //objRenderer = GetComponent<Renderer>();
        SetModel();
        
    }

    void SetModel()
    {
        switch (type)
        {
            case 1: meshFilter.mesh = mesh1; value = value1; break;
            case 2: meshFilter.mesh = mesh2; value = value2; break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Controller"))
        {
            Money.Instance.AddMoney(value);
            AudioSource.PlayClipAtPoint(removeAudio, transform.position, 1f);
            Destroy(gameObject);

        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Controller"))
    //    {
    //        Money.Instance.AddMoney(value);
    //        AudioSource.PlayClipAtPoint(removeAudio, transform.position, 1f);
    //        Destroy(gameObject);

    //    }
    //}
}
