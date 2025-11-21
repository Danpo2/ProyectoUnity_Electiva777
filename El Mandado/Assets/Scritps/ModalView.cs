using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ModalView : MonoBehaviour
{
    [Header("Refs")]
    public RectTransform panel;     // El Panel (contenido del modal)
    public CanvasGroup panelCg;     // CanvasGroup en el Panel
    public Image backdrop;          // Image negro full-screen (con Button)

    [Header("Anim")]
    public float fadeTime = 0.18f;
    public float popScale = 1.06f;

    bool isOpen;

    void Reset()
    {
        panel = GetComponentInChildren<RectTransform>(true);
        panelCg = panel ? panel.GetComponent<CanvasGroup>() : null;
        backdrop = GetComponentInChildren<Image>(true);
    }

    void Awake()
    {
        InstantHide();
    }

    public void InstantHide()
    {
        isOpen = false;

        if (panel)
            panel.localScale = Vector3.one;

        if (panelCg)
        {
            panelCg.alpha = 0f;
            panelCg.blocksRaycasts = false;
            panelCg.interactable = false;
        }

        if (backdrop)
        {
            var c = backdrop.color;
            c.a = 0f;
            backdrop.color = c;

            var btn = backdrop.GetComponent<Button>();
            if (btn) btn.enabled = false;
        }

        gameObject.SetActive(false);
    }

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(CoOpen());
    }

    public void Close()
    {
        if (!isOpen && !gameObject.activeSelf) return;
        isOpen = false;

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);   // activo mientras anima el cierre

        StopAllCoroutines();
        StartCoroutine(CoClose());
    }

    IEnumerator CoOpen()
    {
        // Estado inicial
        if (panel) panel.localScale = Vector3.one * 0.92f;
        if (panelCg)
        {
            panelCg.alpha = 0f;
            panelCg.blocksRaycasts = true;
            panelCg.interactable = true;
        }
        if (backdrop)
        {
            var btn = backdrop.GetComponent<Button>();
            if (btn) btn.enabled = true;
        }

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / fadeTime);

            // Fade panel + backdrop
            if (panelCg) panelCg.alpha = k;
            if (backdrop)
            {
                var c = backdrop.color;
                c.a = Mathf.Lerp(0f, 0.5f, k);
                backdrop.color = c;
            }

            // Pop scale (ligero overshoot)
            if (panel)
            {
                float s = Mathf.SmoothStep(0.92f, popScale, k);
                panel.localScale = new Vector3(s, s, 1f);
            }

            yield return null;
        }

        if (panel) panel.localScale = Vector3.one;
    }

    IEnumerator CoClose()
    {
        float startAlpha = panelCg ? panelCg.alpha : 1f;
        float startBackdrop = backdrop ? backdrop.color.a : 0.5f;
        float startScale = panel ? panel.localScale.x : 1f;

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / fadeTime);

            if (panelCg) panelCg.alpha = Mathf.Lerp(startAlpha, 0f, k);
            if (backdrop)
            {
                var c = backdrop.color;
                c.a = Mathf.Lerp(startBackdrop, 0f, k);
                backdrop.color = c;
            }
            if (panel)
            {
                float s = Mathf.Lerp(startScale, 0.92f, k);
                panel.localScale = new Vector3(s, s, 1f);
            }

            yield return null;
        }

        InstantHide();
    }
}
