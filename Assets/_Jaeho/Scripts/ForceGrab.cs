using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ForceGrab : MonoBehaviour
{
    XRGrabInteractable grabInteractable;
    private void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        //transform.position = PlayerReferences.instance.rightController.transform.position;
        grabInteractable.interactionManager.SelectEnter(PlayerReferences.instance.rightController.GetComponentInChildren<XRDirectInteractor>() as IXRSelectInteractor, grabInteractable);
    }
    private void Update()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        //transform.position = PlayerReferences.instance.rightController.transform.position;
        grabInteractable.interactionManager.SelectEnter(PlayerReferences.instance.rightController.GetComponentInChildren<XRDirectInteractor>() as IXRSelectInteractor, grabInteractable);
    }
}
