using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;

[RequireComponent(typeof(Button))]
public class MapViewButton : MonoBehaviour, IMixedRealityFocusHandler
{
    public MapModeController controller;
    public MapModeController.MapViewMode representedMode;

    Image bg;
    Text label;
    Coroutine scaleRoutine;

    void Awake()
    {
        bg = GetComponent<Image>();
        label = GetComponentInChildren<Text>();

        GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log($"[UI] Click on {representedMode}");
            if (controller != null)
                controller.SwitchTo(representedMode);
            else
                Debug.LogWarning("MapViewButton: controller is null!");
        });
    }

    public void ApplyVisual(Sprite bgSprite, string labelText, float targetScale, float dur)
    {
        if (bg)
        {
            bg.sprite = bgSprite;
            bg.enabled = bgSprite != null;
        }

        if (label)
            label.text = labelText;

        if (scaleRoutine != null) StopCoroutine(scaleRoutine);
        scaleRoutine = StartCoroutine(LerpScale(targetScale, dur));
    }

    IEnumerator LerpScale(float to, float dur)
    {
        Vector3 from = transform.localScale, target = Vector3.one * to;
        for (float t = 0; t < dur; t += Time.deltaTime)
        {
            float n = Mathf.SmoothStep(0, 1, t / dur);
            transform.localScale = Vector3.Lerp(from, target, n);
            yield return null;
        }
        transform.localScale = target;
    }

    public void OnFocusEnter(FocusEventData e) => controller?.SetHover(representedMode);
    public void OnFocusExit(FocusEventData e) => controller?.ClearHover(representedMode);
}
