using UnityEngine;

[System.Serializable]
public class Payline
{
    public Transform point1;
    public Transform point2;
    public Transform point3;

    

    [HideInInspector] public LineRenderer lineRenderer;
}

public class PaylineManager : MonoBehaviour
{
    [Header("Payline Settings")]
    public Material lineMaterial;
    public float lineWidth = 0.05f;

    [Header("Defined Paylines")]
    public Payline[] paylines;

    void Start()
    {
        foreach (var payline in paylines)
        {
            CreateLine(payline);
        }
    }

    void CreateLine(Payline payline)
    {
        GameObject lineObj = new GameObject("Payline");
        lineObj.transform.SetParent(transform);

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.positionCount = 3;

        // Ensure it renders above reels/symbols
        lr.sortingOrder = 20;
        lr.sortingLayerName = "Default"; // or change to your custom sorting layer

        // Set initial positions
        lr.SetPosition(0, payline.point1.position);
        lr.SetPosition(1, payline.point2.position);
        lr.SetPosition(2, payline.point3.position);

        payline.lineRenderer = lr;
    }

    public void UpdatePaylinePositions()
    {
        foreach (var payline in paylines)
        {
            if (payline.lineRenderer != null)
            {
                payline.lineRenderer.SetPosition(0, payline.point1.position);
                payline.lineRenderer.SetPosition(1, payline.point2.position);
                payline.lineRenderer.SetPosition(2, payline.point3.position);
            }
        }
    }
}
