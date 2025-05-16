using UnityEngine;
using System;
using System.Collections.Generic;

public class ModeTabController : MonoBehaviour
{
    public enum Mode { Map, Robot }

    [Header("UI")]
    public ModeLibrary library;
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    [Tooltip("Reference to ModeTabController")]
    public MapModeController mapController;

    [Header("Animation")]
    public float hoverScale = 1.05f;
    public float tweenSeconds = 0.12f;

    public Mode CurrentMode { get; private set; } = Mode.Map;

    readonly Dictionary<Mode, ModeButton> buttons = new Dictionary<Mode, ModeButton>();
    Mode? hoverMode = null;

    void Start()
    {
        CreateButtons();
        if (mapController != null)
            mapController.gameObject.SetActive(CurrentMode == Mode.Map);
    }

    public void SetHover(Mode m) { hoverMode = m; UpdateUI(); }
    public void ClearHover(Mode m) { if (hoverMode == m) hoverMode = null; UpdateUI(); }

    public void SwitchTo(Mode m)
    {
        if (m == CurrentMode) return;
        CurrentMode = m;
        Debug.Log($"[Tab] switched to {m}");

        UpdateUI();

        if (mapController != null)
            mapController.gameObject.SetActive(CurrentMode == Mode.Map);
    }

    void CreateButtons()
    {
        foreach (Transform c in buttonContainer) Destroy(c.gameObject);
        buttons.Clear();

        foreach (Mode m in Enum.GetValues(typeof(Mode)))
        {
            var go = Instantiate(buttonPrefab, buttonContainer);
            go.name = $"{library.Get((int)m).name}Button";

            var mb = go.GetComponent<ModeButton>();
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
                (active || hover) ? info.iconActive : info.iconInactive,
                (active || hover) ? hoverScale : 1f,
                tweenSeconds
            );
        }
    }
}
