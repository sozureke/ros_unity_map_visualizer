using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapModeController : MonoBehaviour
{
    public enum MapViewMode { View2D, View3D, Live }

    public event Action<MapViewMode> OnModeSwitched;

    [Header("UI")]
    public MapViewLibrary library;
    public GameObject buttonPrefab;
    public Transform buttonContainer;

    [Header("Animation")]
    public float hoverScale = 1.05f;
    public float tweenSeconds = 0.12f;
    public float fadeDuration = 0.3f;

    public MapViewMode CurrentMode { get; private set; } = MapViewMode.View2D;
    readonly Dictionary<MapViewMode, MapViewButton> buttons = new Dictionary<MapViewMode, MapViewButton>();
    MapViewMode? hoverMode = null;
    CanvasGroup tabCanvasGroup;

    void Awake()
    {
        tabCanvasGroup = GetComponent<CanvasGroup>();
        if (tabCanvasGroup == null)
            tabCanvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Start()
    {
        CreateButtons();
        tabCanvasGroup.alpha = 1;
        tabCanvasGroup.interactable = true;
        tabCanvasGroup.blocksRaycasts = true;
    }

    public void SetHover(MapViewMode m)
    {
        hoverMode = m;
        UpdateUI();
    }

    public void ClearHover(MapViewMode m)
    {
        if (hoverMode == m) hoverMode = null;
        UpdateUI();
    }

    public void SwitchTo(MapViewMode m)
    {
        if (m == CurrentMode) return;
        CurrentMode = m;
        UpdateUI();
        OnModeSwitched?.Invoke(CurrentMode);
    }

    void CreateButtons()
    {
        foreach (Transform c in buttonContainer) Destroy(c.gameObject);
        buttons.Clear();

        foreach (MapViewMode m in Enum.GetValues(typeof(MapViewMode)))
        {
            var go = Instantiate(buttonPrefab, buttonContainer);
            go.name = $"{library.Get((int)m).name}Button";
            var mb = go.GetComponent<MapViewButton>();
            mb.controller = this;
            mb.representedMode = m;
            buttons[m] = mb;
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        foreach (var kv in buttons)
        {
            var mode = kv.Key;
            var btn = kv.Value;
            var info = library.Get((int)mode);
            bool active = mode == CurrentMode;
            bool hover = hoverMode == mode;

            btn.ApplyVisual(
                active ? info.bgActive : null,
                info.label,
                (active || hover) ? hoverScale : 1f,
                tweenSeconds
            );
        }

        if (ThreeDMap.Instance != null)
            ThreeDMap.Instance.SetVisible(CurrentMode == MapViewMode.View3D);
    }
}
