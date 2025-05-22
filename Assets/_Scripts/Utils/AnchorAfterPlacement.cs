using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

[RequireComponent(typeof(SurfaceMagnetism))]
[RequireComponent(typeof(SolverHandler))]
public class AnchorAfterPlacement : MonoBehaviour
{
    private const float STABLE_DURATION = 0.5f;

    private SurfaceMagnetism solver;
    private SolverHandler handler;

    private bool anchored = false;
    private float stableTime = 0f;
    private Vector3 lastPos = Vector3.zero;

    void Awake()
    {
        solver = GetComponent<SurfaceMagnetism>();
        handler = GetComponent<SolverHandler>();
        lastPos = transform.position;
    }

    void Update()
    {
        if (anchored)
            return;

        Vector3 currentPos = transform.position;
        if (Vector3.Distance(currentPos, lastPos) < 0.001f)
        {
            stableTime += Time.deltaTime;
            if (stableTime >= STABLE_DURATION)
            {
                solver.enabled = false;
                handler.enabled = false;
                anchored = true;
            }
        }
        else
        {
            stableTime = 0f;
        }

        lastPos = currentPos;
    }
}
