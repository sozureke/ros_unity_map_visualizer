using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(CanvasRenderer))]
public class Map2DRenderer : MaskableGraphic
{
    public enum ViewMode { Top, Bottom, Front, Side }
    public ViewMode viewMode = ViewMode.Top;

    [SerializeField] private float padding = 5f;
    [SerializeField] private float mapScale = 1f;
    [SerializeField] private Color boxColor = Color.white;

    private IReadOnlyCollection<GameObject> _cachedMeshes = new List<GameObject>();
    private Bounds _cachedTotalBounds;
    private bool _isDirty = true;

    private readonly Vector3[] _worldCorners = new Vector3[4];
    private readonly Vector2[] _uiCorners = new Vector2[4];

    new void Start()
    {
        base.Start();
        ThreeDMap.Instance.MeshAdded += _ => { _isDirty = true; SetVerticesDirty(); };
        ThreeDMap.Instance.MeshRemoved += _ => { _isDirty = true; SetVerticesDirty(); };
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (mapScale <= 0f || float.IsNaN(mapScale)) mapScale = 1f;
        if (padding < 0f || float.IsNaN(padding)) padding = 0f;

        if (_isDirty)
        {
            _cachedMeshes = ThreeDMap.Instance.GetAllMeshes();
            _cachedTotalBounds = GetTotalBounds(_cachedMeshes);
            _isDirty = false;
        }

        if (_cachedMeshes.Count == 0 || _cachedTotalBounds.size == Vector3.zero)
            return;

        var rect = rectTransform.rect;
        float w = rect.width - padding * 2;
        float h = rect.height - padding * 2;

        float totalW, totalH;
        switch (viewMode)
        {
            case ViewMode.Top:
            case ViewMode.Bottom:
                totalW = _cachedTotalBounds.size.x;
                totalH = _cachedTotalBounds.size.z;
                break;
            case ViewMode.Front:
                totalW = _cachedTotalBounds.size.x;
                totalH = _cachedTotalBounds.size.y;
                break;
            default:
                totalW = _cachedTotalBounds.size.z;
                totalH = _cachedTotalBounds.size.y;
                break;
        }
        totalW *= mapScale;
        totalH *= mapScale;

        float scaleW = w / totalW;
        float scaleH = h / totalH;
        float scale = Mathf.Min(scaleW, scaleH);
        if (float.IsInfinity(scale) || float.IsNaN(scale) || scale <= 0f)
            return;

        Vector2 centerUI = new Vector2(rect.width, rect.height) * 0.5f;
        GetAxes(viewMode, out var axisX, out var axisY);

        int idx = 0;
        var totalCenter = _cachedTotalBounds.center;

        foreach (var go in _cachedMeshes)
        {
            if (go == null) continue;
            var mf = go.GetComponent<MeshFilter>();
            if (mf == null) continue;

            var b = mf.mesh.bounds;
            var worldCenter = go.transform.TransformPoint(b.center);
            var worldSize = Vector3.Scale(b.size, go.transform.lossyScale);

            float halfSizeX = worldSize.x * 0.5f;
            float halfSizeY = ((viewMode == ViewMode.Top || viewMode == ViewMode.Bottom)
                                ? worldSize.z * 0.5f
                                : worldSize.y * 0.5f);

            _worldCorners[0] = worldCenter + axisX * -halfSizeX + axisY * halfSizeY;
            _worldCorners[1] = worldCenter + axisX * halfSizeX + axisY * halfSizeY;
            _worldCorners[2] = worldCenter + axisX * halfSizeX + axisY * -halfSizeY;
            _worldCorners[3] = worldCenter + axisX * -halfSizeX + axisY * -halfSizeY;

            for (int i = 0; i < 4; i++)
            {
                var local = _worldCorners[i] - totalCenter;
                float dx = Vector3.Dot(local, axisX) * mapScale * scale;
                float dy = Vector3.Dot(local, axisY) * mapScale * scale;
                _uiCorners[i] = centerUI + new Vector2(dx, dy);
                vh.AddVert(_uiCorners[i], boxColor, Vector2.zero);
            }

            vh.AddTriangle(idx, idx + 1, idx + 2);
            vh.AddTriangle(idx, idx + 2, idx + 3);
            idx += 4;
        }
    }

    private static void GetAxes(ViewMode mode, out Vector3 axisX, out Vector3 axisY)
    {
        switch (mode)
        {
            case ViewMode.Top:
                axisX = Vector3.right;
                axisY = Vector3.forward;
                break;
            case ViewMode.Bottom:
                axisX = Vector3.right;
                axisY = -Vector3.forward;
                break;
            case ViewMode.Front:
                axisX = Vector3.right;
                axisY = Vector3.up;
                break;
            default:
                axisX = Vector3.forward;
                axisY = Vector3.up;
                break;
        }
    }

    private Bounds GetTotalBounds(IReadOnlyCollection<GameObject> meshes)
    {
        Bounds total = new Bounds();
        bool init = false;
        foreach (var go in meshes)
        {
            if (go == null) continue;
            var mf = go.GetComponent<MeshFilter>();
            if (mf == null) continue;
            var b = mf.mesh.bounds;
            var worldC = go.transform.TransformPoint(b.center);
            var worldS = Vector3.Scale(b.size, go.transform.lossyScale);
            var wb = new Bounds(worldC, worldS);

            if (!init)
            {
                total = wb;
                init = true;
            }
            else
            {
                total.Encapsulate(wb);
            }
        }
        return total;
    }
}
