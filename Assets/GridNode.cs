using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GridNode : PathNode
{
    [SerializeField] private GameObject centerNode;

    private static List<GameObject> obstacles;

    private void Awake()
    {
        obstacles = GameObject.FindGameObjectsWithTag("Obstacle").ToList();
    }

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
        foreach (var obstacle in obstacles)
        {
            var collider = obstacle.GetComponent<Collider>();
            var closestPoint = collider.ClosestPoint(transform.position);

            if ((closestPoint - transform.position).magnitude <= 0.5f)
            {
                //Debug.Log($"Obstructed {transform.name}");

                this.centerNode.GetComponent<MeshRenderer>().material = this.obstructedCenterMat;

                foreach (var node in this.neighboringNodes)
                    node.neighboringNodes.Remove(this);

                return true;
            }
        }

        return false;
    }
}
