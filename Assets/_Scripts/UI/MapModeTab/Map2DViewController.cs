using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class Map2DViewController : MonoBehaviour
{
    public MapModeController mapModeController;
    public float fadeDuration = 0.3f;

    CanvasGroup cg;
    Coroutine fadeRoutine;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        bool startVisible = mapModeController.CurrentMode == MapModeController.MapViewMode.View2D;
        cg.alpha = startVisible ? 1f : 0f;
        cg.interactable = startVisible;
        cg.blocksRaycasts = startVisible;
    }

    void OnEnable()
    {
        mapModeController.OnModeSwitched += OnModeChanged;
    }

    void OnDisable()
    {
        mapModeController.OnModeSwitched -= OnModeChanged;
    }

    void OnModeChanged(MapModeController.MapViewMode mode)
    {
        bool shouldShow = mode == MapModeController.MapViewMode.View2D;
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(Fade(shouldShow));
    }

    IEnumerator Fade(bool show)
    {
        float start = cg.alpha;
        float end = show ? 1f : 0f;
        for (float t = 0f; t < fadeDuration; t += Time.deltaTime)
        {
            cg.alpha = Mathf.Lerp(start, end, t / fadeDuration);
            yield return null;
        }
        cg.alpha = end;
        cg.interactable = show;
        cg.blocksRaycasts = show;
    }
}
