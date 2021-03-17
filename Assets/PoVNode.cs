using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PoVNode : PathNode
{
    [SerializeField] private GameObject centerNode;

    // Start is called before the first frame update
    void Start()
    {
        // ensure bidirectionality in case I missed it in the editor ;)
        foreach (var node in this.neighboringNodes)
            if (node != null && !node.neighboringNodes.Contains(this))
                node.neighboringNodes.Add(this);

        // remove duplicates
        this.neighboringNodes = this.neighboringNodes.Distinct().ToList();

        // remove empty
        this.neighboringNodes.RemoveAll(node => node == null);

        DrawNeighboringConnections();
    }

    private void OnDrawGizmos()
    {
        Handles.Label(transform.position, transform.gameObject.name);
    }

    public override void Highlight()
    {
        this.centerNode.GetComponent<MeshRenderer>().material = highlightedCenterMat;
    }

    public override void RevertHighlighting()
    {
        this.centerNode.GetComponent<MeshRenderer>().material = normalCenterMat;
    }
}
