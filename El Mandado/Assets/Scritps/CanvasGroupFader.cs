using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class CanvasGroupFader : MonoBehaviour
{
    public CanvasGroup cg;

    void Reset()
    {
        cg = GetComponent<CanvasGroup>();
        if (!cg) cg = gameObject.AddComponent<CanvasGroup>();
    }

    public void Instant(float a)
    {
        if (!cg) cg = GetComponent<CanvasGroup>();
        cg.alpha = Mathf.Clamp01(a);
        cg.blocksRaycasts = a > 0.99f;
        cg.interactable = a > 0.99f;
    }

    public async Task FadeTo(float target, float duration)
    {
        if (!cg) cg = GetComponent<CanvasGroup>();
        float start = cg.alpha;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / duration);
            cg.alpha = Mathf.Lerp(start, target, k);
            await Task.Yield();
        }
        cg.alpha = target;
        cg.blocksRaycasts = target > 0.99f;
        cg.interactable = target > 0.99f;
    }
}
