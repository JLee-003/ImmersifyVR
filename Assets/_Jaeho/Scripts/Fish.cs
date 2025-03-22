using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    MeshFilter meshFilter;
    //Renderer objRenderer;
    public Mesh[] meshes;
    [SerializeField] AudioClip removeAudio;

    //public Material mat1, mat2;

    public int type;

    int value = 100;

    private void Start()
    {
        meshFilter = GetComponentInChildren<MeshFilter>();
        //objRenderer = GetComponent<Renderer>();
        SetModel();
        
    }

    void SetModel()
    {
        meshFilter.mesh = meshes[type-1];
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Controller"))
        {
            Money.Instance.AddMoney(value);
            AudioSource.PlayClipAtPoint(removeAudio, transform.position, 1f);
            HapticFeedbackManager.Instance.InitiateHapticFeedback(true, true, 1f, 1f);
            Destroy(gameObject);
            if (FishGame.Instance != null)
            {
                FishGame.Instance.CaughtFishUIDisplay();
            }
        }
    }
}
