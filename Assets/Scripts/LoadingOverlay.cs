using System.Collections;
using UnityEngine;

public class LoadingOverlay : MonoBehaviour
{

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private float fadeInTime;

    [SerializeField]
    private float fadeOutTime;

    public IEnumerator FadeInBlack()
    {
        Debug.Log("Overlay fading in.");
        yield return FadeTo(1f, fadeInTime);
    }

    public IEnumerator FadeOutBlack()
    {
        yield return FadeTo(0f, fadeOutTime);
        Debug.Log("Overlay faded out.");
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {   
        float startAlpha = canvasGroup.alpha;
        float startTime = Time.time;

        while(Time.time < startTime + duration)
        {
            float t = Mathf.Clamp01((Time.time - startTime) / duration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}
