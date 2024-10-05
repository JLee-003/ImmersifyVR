using System.Collections;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private float _intensity = 0.0f;
    [SerializeField] private Color _color = Color.black;
    [SerializeField] private Material _fadeMaterial = null;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_fadeMaterial == null)
        {
            Debug.LogError("Fade material is not assigned.");
            Graphics.Blit(source, destination);
            return;
        }

        _fadeMaterial.SetFloat("_Intensity", Mathf.Clamp01(_intensity));
        _fadeMaterial.SetColor("_FadeColor", _color);
        Graphics.Blit(source, destination, _fadeMaterial);
    }

    public Coroutine StartFadeIn()
    {
        StopAllCoroutines();
        return StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        while (_intensity < 1.0f)
        {
            _intensity += _speed * Time.deltaTime;
            _intensity = Mathf.Clamp01(_intensity);
            yield return null;
        }
    }

    public Coroutine StartFadeOut()
    {
        StopAllCoroutines();
        return StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        while (_intensity > 0.0f)
        {
            _intensity -= _speed * Time.deltaTime;
            _intensity = Mathf.Clamp01(_intensity);
            yield return null;
        }
    }
}
