using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class FaderCaller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Task abc = Fader.Instance.FadeOut();
    }
}
