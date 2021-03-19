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
    [SerializeField] private float iterationsPerFrame = 1000;
    // let's make sure we don't enter an endless loop....

    private bool hasPathFindingStarted;
    private bool hasPathComputationEnded;
    private float timeToWait = 1f;
    private PathNode currentNode;
    private NodeEntry currentEntry;
    private List<NodeEntry> openList, closedList;
    private int currentIteration;

    private void Update()
    {
        if (hasPathComputationEnded)
            return;
        
        if (!hasPathFindingStarted)
        {
            if (timeToWait <= 0)
            {
                FindPath();
                return;
            }
            
            timeToWait -= Time.deltaTime;
            return;
        }

        ContinuePathFinding();
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

    private void FindPath()
    {
        hasPathFindingStarted = true;

        this.startingNode = GetClosestNodeTo(startingFlag);
        this.goalNode = GetClosestNodeTo(goalFlag);
        
        InitializePathfinding();

        ContinuePathFinding();
    }

    private void InitializePathfinding()
    {
        currentNode = this.startingNode;
        currentEntry = new NodeEntry
        {
            PathNode = currentNode,
            Connection = null,
            CostSoFar = 0,
            EstimatedTotalCost = usingEuclideanHeristic ? Vector3.Distance(goalNode.transform.position, currentNode.transform.position) : 0
        };
        openList = new List<NodeEntry>
        {
            currentEntry
        };
        closedList = new List<NodeEntry>();
        currentIteration = 0;
    }

    private void ContinuePathFinding()
    {
        var currentFrameIteration = 0;

        while (openList.Count > 0 
            && currentIteration < maxIterations
            && currentFrameIteration < iterationsPerFrame)
        {
            closedList.Add(currentEntry);
            closedList = closedList.Distinct().ToList();

            currentEntry = openList.First();
            openList.Remove(currentEntry);
            currentNode = currentEntry.PathNode;

            if (usingEuclideanHeristic && currentNode == goalNode)
            {
                closedList.Add(currentEntry);
                break;
            }

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

                var existingEntryInOpenList = openList.Find(entry => entry.PathNode == neighboringNode);
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

            //Debug.Log(openList.Count());

            ++currentIteration;
            ++currentFrameIteration;
        }

        if (usingEuclideanHeristic && currentNode == goalNode
            || openList.Count() == 0 
            || currentIteration >= maxIterations)
            HandlePathResult();
    }

    private void HandlePathResult()
    {
        hasPathComputationEnded = true;

        var goalEntries = closedList.Where(entry => entry.PathNode == goalNode);
        if (goalEntries.Count() == 0)
        {
            Debug.Log("Goal state not reached");

            return;
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
