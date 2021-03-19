using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GridNode : PathNode
{
    [SerializeField] private GameObject centerNode;

    public void SetNeighboringGridNodes()
    {
        if (this.IsObstructed())
            return; // Don't bother setting neighboring nodes
        
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

        if (neighboringGridNode == null 
            || neighboringGridNode == this
            || neighboringGridNode.IsObstructed())
            return;

        if (!neighboringGridNode.neighboringNodes.Contains(this))
            neighboringGridNode.neighboringNodes.Add(this);
        
        if(!this.neighboringNodes.Contains(neighboringGridNode))
            this.neighboringNodes.Add(neighboringGridNode);
    }

    public override void Highlight()
    {
        this.centerNode.GetComponent<MeshRenderer>().material = this.highlightedCenterMat;
    }

    public override void RevertHighlighting()
    {
        this.centerNode.GetComponent<MeshRenderer>().material = this.normalCenterMat;
    }

    public bool IsObstructed()
    {
        var colliders = Physics.OverlapSphere(transform.position, 0.45f);

        colliders = colliders.Where(collider => collider.CompareTag("Obstacle")).ToArray();

        if (colliders.Length == 0)
            return false;

        this.centerNode.GetComponent<MeshRenderer>().material = this.obstructedCenterMat;

        foreach (var node in this.neighboringNodes)
            node.neighboringNodes.Remove(this);

        return true;
    }

    internal override void Explore()
    {
        Highlight();
    }
}
