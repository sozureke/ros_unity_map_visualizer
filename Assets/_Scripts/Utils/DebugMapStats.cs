using UnityEngine;
using TMPro;

public class DebugMapStats : MonoBehaviour
{
    public TextMeshProUGUI statsText;

    void Update()
    {
        if (ThreeDMap.Instance == null) return;
        statsText.text = $"Meshes: {ThreeDMap.Instance.MeshCount()}\n" +
                         $"Markers: {ThreeDMap.Instance.MarkerCount()}";
    }
}
