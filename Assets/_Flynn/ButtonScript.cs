using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    public LevelChanger levelChanger;
    public Button button;

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(TaskOnClick);
    }

    // Update is called once per frame
    void TaskOnClick()

    {
        levelChanger.FadeToLevel("Main");
        

    }
    void Update()
    {
        
    }
}
