using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;


public class ShaderValueChanger : MonoBehaviour
{
    public Material material; // The material whose shader value you want to change
    public string shaderProperty = "_MyShaderProperty"; // The name of the shader property to change
    public float targetValue = 1.0f; // The target value for the shader property
    public float duration = 3.0f; // The duration over which to change the value
    public GameObject airport;
    public Button myButton;

    private void Start()
    {
        //airport.SetActive(false);

        if (myButton != null)
        {
            // Add a listener to the button to call the OnButtonClick method when clicked
            myButton.onClick.AddListener(OnButtonClick);
        }
    }


    private IEnumerator ChangeShaderValueOverTime()
    {
        
        float startValue = material.GetFloat(shaderProperty);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newValue = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            material.SetFloat(shaderProperty, newValue);
            yield return null;
            SceneManager.LoadScene("Lobby");

        }

        // Ensure the final value is set
        material.SetFloat(shaderProperty, targetValue);
    }
    private void OnButtonClick()
    {
        airport.SetActive(true);
        Debug.Log("Button clicked!");
        ChangeShaderValueOverTime();
    }
}
