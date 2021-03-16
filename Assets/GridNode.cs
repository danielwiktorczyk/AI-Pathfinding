using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : MonoBehaviour
{
    public List<GridNode> neighboringGridNodes;

    [SerializeField] private Material lineMat;

    public void SetNeighboringGridNodes()
    {
        // Get all 8 directions
        for (var i = -1; i <= 1; ++i)
            for (var j = -1; j <= 1; ++j)
                SetNeighboringGridNode(i, j);
    }

    private void SetNeighboringGridNode(int i, int j)
    {
        var neighboringGridNodePosition = transform.position
            + Vector3.left * i
            + Vector3.back * j
            + Vector3.down * transform.position.y; // Set y to 0
        
        var colliders = Physics.OverlapSphere(
            neighboringGridNodePosition,
            0.01f,
            LayerMask.NameToLayer("GridNode"));

        if (colliders.Length == 0)
            return;

        var neighboringGridNode = colliders[0].GetComponentInParent<GridNode>();

        if (neighboringGridNode == null || neighboringGridNode == this)
            return;

        if (!neighboringGridNode.neighboringGridNodes.Contains(this))
            neighboringGridNode.neighboringGridNodes.Add(this);
        
        if(!this.neighboringGridNodes.Contains(neighboringGridNode))
            this.neighboringGridNodes.Add(neighboringGridNode);
    }

    public void DrawNeighboringConnections()
    {
        foreach (var node in this.neighboringGridNodes)
            DrawNeighboringConnection(node);
    }

    private void DrawNeighboringConnection(GridNode node)
    {
        var line = new GameObject();
        line.transform.parent = transform;

        line.AddComponent<LineRenderer>();
        var lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.material = this.lineMat;
        
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;

        lineRenderer.startWidth = .15f;
        lineRenderer.endWidth = .15f;
        
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, node.transform.position);
    }
}
