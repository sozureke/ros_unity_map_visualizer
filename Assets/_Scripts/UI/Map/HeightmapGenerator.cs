using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class HeightmapGenerator : MonoBehaviour
{
    public static HeightmapGenerator Instance { get; private set; }

    public LayerMask meshLayerMask;
    public Material mapMaterial;
    public int resolution = 50;
    public float size = 5f;
    public bool autoUpdate = true;
    public float updateInterval = 1f;

    public bool enableSmoothing = true;
    public int smoothingIterations = 1;

    private float timer;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start() => Generate();

    void Update()
    {
        if (!autoUpdate) return;
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            Generate();
            timer = 0f;
        }
    }

    public void Generate()
    {
        int steps = resolution;
        float half = size * 0.5f;
        float step = size / steps;
        var heights = new float[steps + 1, steps + 1];

        for (int z = 0; z <= steps; z++)
        {
            for (int x = 0; x <= steps; x++)
            {
                float wx = -half + x * step;
                float wz = -half + z * step;
                var origin = new Vector3(wx, 10f, wz);
                if (Physics.Raycast(origin, Vector3.down, out var hit, 20f, meshLayerMask))
                    heights[x, z] = hit.point.y;
                else
                    heights[x, z] = 0f;
            }
        }

        if (enableSmoothing)
        {
            for (int iter = 0; iter < smoothingIterations; iter++)
            {
                var temp = (float[,])heights.Clone();
                for (int z = 1; z < steps; z++)
                    for (int x = 1; x < steps; x++)
                    {
                        float sum = 0f;
                        for (int oz = -1; oz <= 1; oz++)
                            for (int ox = -1; ox <= 1; ox++)
                                sum += temp[x + ox, z + oz];
                        heights[x, z] = sum / 9f;
                    }
            }
        }

        var verts = new Vector3[(steps + 1) * (steps + 1)];
        var tris = new int[steps * steps * 6];
        var colors = new Color[verts.Length];
        int ti = 0;
        float minH = float.MaxValue, maxH = float.MinValue;

        for (int z = 0; z <= steps; z++)
            for (int x = 0; x <= steps; x++)
            {
                float h = heights[x, z];
                minH = Mathf.Min(minH, h);
                maxH = Mathf.Max(maxH, h);
            }

        for (int z = 0; z <= steps; z++)
            for (int x = 0; x <= steps; x++)
            {
                int idx = z * (steps + 1) + x;
                float wx = -half + x * step;
                float wz = -half + z * step;
                float h = heights[x, z];
                verts[idx] = new Vector3(wx, h, wz);
                float t = maxH > minH ? (h - minH) / (maxH - minH) : 0f;
                colors[idx] = Color.Lerp(Color.blue, Color.red, t);

                if (x < steps && z < steps)
                {
                    tris[ti++] = idx;
                    tris[ti++] = idx + steps + 1;
                    tris[ti++] = idx + 1;
                    tris[ti++] = idx + 1;
                    tris[ti++] = idx + steps + 1;
                    tris[ti++] = idx + steps + 2;
                }
            }

        var mesh = new Mesh { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.colors = colors;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        var mf = GetComponent<MeshFilter>();
        mf.mesh = mesh;
        var mr = GetComponent<MeshRenderer>();
        mr.material = mapMaterial;
        var mc = GetComponent<MeshCollider>();
        mc.sharedMesh = mesh;
    }
}
