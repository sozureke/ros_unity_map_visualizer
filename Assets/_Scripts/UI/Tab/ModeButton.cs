using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;

[RequireComponent(typeof(Button))]
public class ModeButton : MonoBehaviour,
    IMixedRealityFocusHandler,
    IMixedRealityPointerHandler
{
    public ModeTabController controller;
    public ModeTabController.Mode representedMode;

    Image bg, icon;
    Coroutine scaleRoutine;

    void Awake()
    {
        bg = transform.Find("Background")?.GetComponent<Image>();
        icon = transform.Find("Icon")?.GetComponent<Image>();

        GetComponent<Button>()
            .onClick.AddListener(() =>
            {
                Debug.Log($"UI CLICK on {representedMode}");
                controller.SwitchTo(representedMode);
            });
    }

    public void ApplyVisual(Sprite bgSprite, Sprite iconSprite,
                            float targetScale, float dur)
    {
        if (bg)
        {
            bg.sprite = bgSprite;
            bg.enabled = bgSprite;
        }
        if (icon) icon.sprite = iconSprite;

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

    public void OnFocusEnter(FocusEventData e) => controller.SetHover(representedMode);
    public void OnFocusExit(FocusEventData e) => controller.ClearHover(representedMode);

    public void OnPointerDown(MixedRealityPointerEventData e)
    {
        if (e.Pointer is PokePointer)
        {
            Debug.Log($"POKE CLICK on {representedMode}");
            controller.SwitchTo(representedMode);
            e.Use();
        }
    }

    public void OnPointerClicked(MixedRealityPointerEventData e) { }
    public void OnPointerUp(MixedRealityPointerEventData e) { }
    public void OnPointerDragged(MixedRealityPointerEventData e) { }
}
