using UnityEngine;

public class TutorialRock : MonoBehaviour
{
    private LineSwimmer swimmer;
    private NewSwimTutorial tutorial;

    [Header("Rock Settings")]
    [SerializeField] private float velocityRequired = 2f;
    [SerializeField] private bool destroyOnBreak = true;

    [Header("Stop / Bounce")]
    [SerializeField] private float bounceBackMultiplier = -0.15f;

    [Header("Visuals")]
    [SerializeField] private Color crackedRockColor = new Color(0.9f, 0.87f, 0.82f);

    private Renderer rend;
    private Color originalColor;
    private bool broken = false;

    private void Start()
    {
        FindSwimmer();

        tutorial = FindObjectOfType<NewSwimTutorial>();

        rend = GetComponent<Renderer>();

        if (rend != null)
        {
            originalColor = rend.material.color;
        }
    }

    private void FindSwimmer()
    {
        if (PlayerReferences.instance == null)
        {
            Debug.LogError("PlayerReferences.instance is null.");
            return;
        }

        swimmer = PlayerReferences.instance.GetComponent<LineSwimmer>();

        if (swimmer == null && PlayerReferences.instance.playerObject != null)
        {
            swimmer = PlayerReferences.instance.playerObject.GetComponent<LineSwimmer>();

            if (swimmer == null)
            {
                swimmer = PlayerReferences.instance.playerObject.GetComponentInChildren<LineSwimmer>();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (broken)
            return;

        if (!other.CompareTag("Player"))
        {
            Debug.Log("TutorialRock touched by: " + other.name + " | Tag: " + other.tag);
            return;
        }

        BreakRock();
    }

    private void BreakRock()
    {
        broken = true;

        if (rend != null)
        {
            rend.material.color = crackedRockColor;
        }

        if (swimmer != null)
        {
            swimmer.velocity *= bounceBackMultiplier;
        }

        if (tutorial != null)
        {
            tutorial.OnTutorialRockBroken();
        }
        else
        {
            Debug.LogError("TutorialRock could not find NewSwimTutorial.");
        }

        if (destroyOnBreak)
        {
            Destroy(gameObject, 0.15f);
        }
    }
}