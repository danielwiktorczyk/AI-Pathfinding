using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PathNode : MonoBehaviour
{
    public List<PathNode> neighboringNodes;
    [SerializeField] private Material lineMat;

    [SerializeField] protected Material normalCenterMat;
    [SerializeField] protected Material highlightedCenterMat;

    public abstract void Highlight();

    public abstract void RevertHighlighting();

    public void DrawNeighboringConnections()
    {
        foreach (var node in this.neighboringNodes)
            DrawNeighboringConnection(node);
    }

    private void DrawNeighboringConnection(PathNode node)
    {
        if (node == null)
            return;

        var line = new GameObject();
        line.transform.parent = transform;

        line.AddComponent<LineRenderer>();
        var lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.material = this.lineMat;

        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;

        lineRenderer.startWidth = .3f;
        lineRenderer.endWidth = .3f;

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, node.transform.position);

        lineRenderer.alignment = LineAlignment.View;
    }
}
