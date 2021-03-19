using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum PathMode
{
    NormalNodes,
    PoVNodes
}

public class NodeEntry
{
    public PathNode PathNode;
    public float CostSoFar;
    public float EstimatedTotalCost;
    public NodeEntry Connection; // a.k.a. the previous node
}

public class Pathfinder : MonoBehaviour
{
    [SerializeField] private PathMode pathMode;

    [SerializeField] private Transform startingFlag;
    [SerializeField] private Transform goalFlag;

    private PathNode startingNode;
    private PathNode goalNode;

    [SerializeField] private bool usingEuclideanHeristic;
    
    [SerializeField] private float maxIterations = 100000; 
    // let's make sure we don't enter an endless loop....


    private void Start()
    {
        this.startingNode = GetClosestNodeTo(startingFlag);
        this.goalNode = GetClosestNodeTo(goalFlag);

        StartCoroutine(FindPath());
    }

    private PathNode GetClosestNodeTo(Transform transform)
    {
        var position = transform.position;

        PathNode closestNode = null;
        var minDistance = Mathf.Infinity;

        List<PathNode> pathNodes = new List<PathNode>(); 

        switch (this.pathMode)
        {
            case PathMode.NormalNodes:
                pathNodes = FindObjectsOfType<GridNode>().Cast<PathNode>().ToList();
                break;
            case PathMode.PoVNodes:
                pathNodes = FindObjectsOfType<PoVNode>().Cast<PathNode>().ToList();
                break;
        }

        foreach (var node in pathNodes)
        {
            var distance = Vector3.Distance(position, node.transform.position);

            if (distance >= minDistance)
                continue;

            minDistance = distance;
            closestNode = node;
        }

        return closestNode;
    }

    private IEnumerator FindPath()
    {
        yield return new WaitForSeconds(1f); // Let everything initialize

        PathNode currentNode = this.startingNode;
        NodeEntry currentEntry = new NodeEntry
        {
            PathNode = currentNode,
            Connection = null,
            CostSoFar = 0,
            EstimatedTotalCost = usingEuclideanHeristic ? Vector3.Distance(goalNode.transform.position, currentNode.transform.position) : 0
        };

        List<NodeEntry> openList = new List<NodeEntry>
        {
            currentEntry
        };
        List<NodeEntry> closedList = new List<NodeEntry>();
        var currentIteration = 0;

        while (openList.Count > 0 && currentIteration < maxIterations)
        {
            closedList.Add(currentEntry);
            closedList = closedList.Distinct().ToList();

            currentEntry = openList.First();
            openList.Remove(currentEntry);
            currentNode = currentEntry.PathNode;

            if (usingEuclideanHeristic && currentNode == goalNode)
                break;


            //Debug.Log($"Current Node: {currentNode.name}");

            currentNode.Explore();

            foreach (var neighboringNode in currentNode.neighboringNodes)
            {
                if (neighboringNode == null)
                    continue;

                var costSoFar = currentEntry.CostSoFar + Vector3.Distance(currentNode.transform.position, neighboringNode.transform.position);
                var neighboringEntry = new NodeEntry
                {
                    PathNode = neighboringNode,
                    Connection = currentEntry,
                    CostSoFar = costSoFar,
                    EstimatedTotalCost = usingEuclideanHeristic ? costSoFar + Vector3.Distance(goalNode.transform.position, neighboringNode.transform.position) : costSoFar
                };

                //Debug.Log($"Neighbor: {neighboringEntry.PathNode.name}");

                var existingEntryInOpenList = closedList.Find(entry => entry.PathNode == neighboringNode);
                if (existingEntryInOpenList != null)
                {
                    //Debug.Log($"Exists in open: {existingEntryInOpenList.PathNode.name}");
                    if (neighboringEntry.CostSoFar < existingEntryInOpenList.CostSoFar)
                        openList.Add(neighboringEntry);

                    continue;
                }

                var existingEntryInClosedList = closedList.Find(entry => entry.PathNode == neighboringNode);
                if (existingEntryInClosedList != null)
                {
                    //Debug.Log($"Exists in closed: {existingEntryInOpenList.PathNode.name}");
                    if (neighboringEntry.CostSoFar < existingEntryInClosedList.CostSoFar)
                        openList.Add(neighboringEntry);

                    continue;
                }

                // If it doesn't exist in either list, add it! 
                openList.Add(neighboringEntry);
            }

            openList = openList.Distinct().ToList();
            openList = openList.OrderBy(node => node.EstimatedTotalCost).ToList();

            ++currentIteration;

            if (currentIteration % 1000 == 0)
                Debug.Log($"Still processing path! Closed list now at length {closedList.Count()}");
        }

        var goalEntries = closedList.Where(entry => entry.PathNode == goalNode);
        if (goalEntries.Count() == 0)
        {
            Debug.Log("Goal state not reached");

            yield break;
        }

        var goalEntry = goalEntries.First();
        if (goalEntry != null)
        {
            Debug.Log("Goal Found!");
            var traceEntry = goalEntry;
            while (traceEntry != null)
            {
                //Debug.Log(traceEntry.PathNode.name);
                traceEntry.PathNode.Highlight();
                traceEntry = traceEntry.Connection;
            }
        }
    }

}
