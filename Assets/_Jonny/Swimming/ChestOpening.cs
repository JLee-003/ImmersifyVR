using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    [Header("Chest Parts")]
    public Transform chestTop;   // Assign child "chestTop" in Inspector
    public GameObject fish;      // Assign child "fish" in Inspector

    [Header("Animation Settings")]
    public float openAngle = -90f;  // How far to rotate the lid
    public float openSpeed = 2f;    // Speed of rotation

    private bool isOpening = false;
    private bool hasOpened = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        if (chestTop == null)
            chestTop = transform.Find("chestTop");

        if (fish == null)
            fish = transform.Find("fish").gameObject;

        // Store closed & open rotations
        closedRotation = chestTop.localRotation;
        openRotation = closedRotation * Quaternion.Euler(openAngle, 0, 0);

        // Hide fish at start
        if (fish != null)
            fish.SetActive(false);
    }

    void Update()
    {
        if (isOpening && !hasOpened)
        {
            chestTop.localRotation = Quaternion.Lerp(chestTop.localRotation, openRotation, Time.deltaTime * openSpeed);

            // Check if lid is "close enough" to open rotation
            if (Quaternion.Angle(chestTop.localRotation, openRotation) < 1f)
            {
                chestTop.localRotation = openRotation;
                hasOpened = true;
                isOpening = false;

                // Reveal the fish
                fish.SetActive(true);
            }
        }
    }

    // Trigger detection
    private void OnTriggerEnter(Collider other)
    {
        if (!hasOpened && other.transform.root.CompareTag("Player"))
        {
            isOpening = true;
        }
    }

}