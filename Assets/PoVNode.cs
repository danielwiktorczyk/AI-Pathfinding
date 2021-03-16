using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PoVNode : MonoBehaviour
{
    [SerializeField] private List<PoVNode> neighboringNodes;

    [SerializeField] private Material lineMat;

    // Start is called before the first frame update
    void Start()
    {
        // ensure bidirectionality in case I missed it in the editor ;)
        foreach (var node in this.neighboringNodes)
            if (node != null && !node.neighboringNodes.Contains(this))
                node.neighboringNodes.Add(this);

        // remove duplicates
        this.neighboringNodes = this.neighboringNodes.Distinct().ToList();

        DrawNeighboringConnections();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DrawNeighboringConnections()
    {
        foreach (var node in this.neighboringNodes)
            DrawNeighboringConnection(node);
    }

    private void DrawNeighboringConnection(PoVNode node)
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

    private void OnDrawGizmos()
    {
        Handles.Label(transform.position, transform.gameObject.name);
    }
}
