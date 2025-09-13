using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Fish : MonoBehaviour
{
    [System.Serializable]
    public class FishModels
    {
        public Mesh mesh;
        public Vector3 size;
        public Material material;
        [Tooltip("Movement direction offset in Euler angles - defines what direction this fish considers 'forward'")]
        public Vector3 movementDirection = Vector3.zero;
    }
    MeshFilter meshFilter;
    //Renderer objRenderer;
    public FishModels[] meshes;
    [SerializeField] AudioClip removeAudio;

    [Header("Random Materials")]
    public Material[] randomMaterials;

    //public Material mat1, mat2;

    public int type;

    int value = 100;

    public GameObject catchEffect;

    private void Start()
    {
        meshFilter = GetComponentInChildren<MeshFilter>();
        //objRenderer = GetComponent<Renderer>();
        SetModel();

    }

    void SetModel()
    {
        meshFilter.mesh = meshes[type - 1].mesh;
        transform.GetChild(0).localScale = meshes[type - 1].size;
        
        // Apply the material from FishModels or choose randomly
        if (meshes[type - 1].material != null)
        {
            GetComponentInChildren<MeshRenderer>().material = meshes[type - 1].material;
        }
        else if (randomMaterials != null && randomMaterials.Length > 0)
        {
            // Choose a random material from the randomMaterials array
            int randomIndex = Random.Range(0, randomMaterials.Length);
            GetComponentInChildren<MeshRenderer>().material = randomMaterials[randomIndex];
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Controller"))
        {
            Money.Instance.AddMoney(value);
            AudioSource.PlayClipAtPoint(removeAudio, transform.position, 1f);
            HapticFeedbackManager.Instance.InitiateHapticFeedback(true, true, 1f, 1f);

            // Instantiate the particle effect
            if (catchEffect != null)
            {
                GameObject effect = Instantiate(catchEffect, transform.position, Quaternion.identity);
                Debug.Log("CAUGHT!");
            }


            Destroy(gameObject);
            if (FishGame.Instance != null)
            {
                FishGame.Instance.CaughtFishUIDisplay();
            }
        }
    }
}