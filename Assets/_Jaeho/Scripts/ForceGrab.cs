using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ForceGrab : MonoBehaviour
{
    XRGrabInteractable grabInteractable;
    public bool isRight = true;
    private void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        XRDirectInteractor rightInteractor = PlayerReferences.instance.rightController.GetComponentInChildren<XRDirectInteractor>();
        XRDirectInteractor leftInteractor = PlayerReferences.instance.leftController.GetComponentInChildren<XRDirectInteractor>();
        //transform.position = PlayerReferences.instance.rightController.transform.position;
        if (isRight)
        {
            grabInteractable.interactionManager.SelectEnter(rightInteractor as IXRSelectInteractor, grabInteractable);
        }
        else
        {
            grabInteractable.interactionManager.SelectEnter(leftInteractor as IXRSelectInteractor, grabInteractable);
        }
        
    }
    private void Update()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        XRDirectInteractor rightInteractor = PlayerReferences.instance.rightController.GetComponentInChildren<XRDirectInteractor>();
        XRDirectInteractor leftInteractor = PlayerReferences.instance.leftController.GetComponentInChildren<XRDirectInteractor>();
        //transform.position = PlayerReferences.instance.rightController.transform.position;
        if (isRight)
        {
            grabInteractable.interactionManager.SelectEnter(rightInteractor as IXRSelectInteractor, grabInteractable);
        }
        else
        {
            grabInteractable.interactionManager.SelectEnter(leftInteractor as IXRSelectInteractor, grabInteractable);
        }
    }
}
