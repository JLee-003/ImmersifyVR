using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokeButtonTest : MonoBehaviour
{
    public void PokeButton()
    {
        HapticFeedbackManager.Instance.InitiateHapticFeedback(true, true, 1, 1);
    }
}
