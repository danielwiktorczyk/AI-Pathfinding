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
    [SerializeField] private PathMode pathNode;

    [SerializeField] private PathNode startingNode;
    [SerializeField] private PathNode goalNode;

    private void Start()
    {
        if (this.pathNode == PathMode.NormalNodes)
        {
            var normalNodesInScene = GameObject.FindObjectsOfType<GridNode>();

            this.startingNode = normalNodesInScene[Random.Range(0, normalNodesInScene.Length)];
            this.goalNode = normalNodesInScene[Random.Range(0, normalNodesInScene.Length)];
        }

        StartCoroutine(FindPath());
    }

    private IEnumerator FindPath()
    {
        yield return new WaitForSeconds(0.25f); // Let everything initialize

        PathNode currentNode = this.startingNode;
        NodeEntry currentEntry = new NodeEntry
        {
            PathNode = currentNode,
            Connection = null,
            CostSoFar = 0,
            EstimatedTotalCost = 0 // TODO
        };

        List<NodeEntry> openList = new List<NodeEntry>
        {
            currentEntry
        };
        List<NodeEntry> closedList = new List<NodeEntry>();
        var maxIterations = 1000; // let's make sure we don't enter an endless loop....
        var currentIteration = 0;

        while (openList.Count > 0 && currentIteration < maxIterations)
        {
            closedList.Add(currentEntry);
            closedList = closedList.Distinct().ToList();

            currentEntry = openList.First();
            openList.Remove(currentEntry);
            currentNode = currentEntry.PathNode;

            //Debug.Log($"Current Node: {currentEntry.PathNode.name}");

            foreach (var neighboringNode in currentNode.neighboringNodes)
            {
                var neighboringEntry = new NodeEntry
                {
                    PathNode = neighboringNode,
                    Connection = currentEntry,
                    CostSoFar = currentEntry.CostSoFar + Vector3.Distance(currentNode.transform.position, neighboringNode.transform.position),
                    EstimatedTotalCost = 0 // TODO
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
            openList = openList.OrderBy(node => node.CostSoFar).ToList();

            ++currentIteration;

            if (currentIteration % 1000 == 0)
                Debug.Log($"Still processing path! Closed list now at length {closedList.Count()}");
        }

        var goalEntries = closedList.Where(node => node.PathNode == goalNode);
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
