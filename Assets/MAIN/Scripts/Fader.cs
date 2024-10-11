using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;
using System.Threading.Tasks;

public class Fader : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;

    public static Fader Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }

    public async Task FadeIn(float time = 0.5f, Ease ease = Ease.Linear, bool reset = true, bool ignoreTimeScale = false)
    {
        if(reset)
        {
            meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, 0);
        }
        await Tween.MaterialAlpha(meshRenderer.material, endValue: 1f, duration: time, ease: ease, useUnscaledTime: ignoreTimeScale);
    }

    public async Task FadeOut(float time = 0.5f, Ease ease = Ease.Linear, bool reset = true, bool ignoreTimeScale = false)
    {
        if (reset)
        {
            meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, 1f);
        }

        Debug.Log("FADING");
        await Tween.MaterialAlpha(meshRenderer.material, endValue: 0f, duration: time, ease: ease, useUnscaledTime: ignoreTimeScale);

    }

    public async Task FadeInOut(float time = 1f, Ease ease = Ease.Linear, bool reset = true, bool ignoreTimeScale = false)
    {
        if (reset)
        {
            await FadeIn(time / 2f, ease, ignoreTimeScale: ignoreTimeScale);
        }
        await FadeOut(time / 2f, ease, ignoreTimeScale: ignoreTimeScale);
    }
}
