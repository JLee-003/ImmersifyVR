using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildLoader : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        if (!Application.isEditor) {
            LoadPersistent();
        }
    }

    // Update is called once per frame
    private void LoadPersistent()
    {
        SceneManager.LoadSceneAsync("SecondScene", LoadSceneMode.Additive);
    }
}
