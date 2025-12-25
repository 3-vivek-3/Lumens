using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeManager : MonoBehaviour
{
    [Header("Settings")]
    public int mazeSize;
    public int cellSize;

    [Header("References")]
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject exitWallPrefab;

    private Graph maze;
    [HideInInspector]
    public bool mazeGenerated = false;

    [HideInInspector]
    public GameObject exitWall;

    public void GenerateMaze()
    {
        maze = new Graph(mazeSize, cellSize);

        GenerateWilsonMaze(maze);

        if (floorPrefab)
        {
            GameObject floorClone = Instantiate(floorPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, GameplayManager.instance.dynamicContainer);
            float scale = mazeSize * cellSize / 10f;
            floorClone.transform.localScale = new Vector3(scale, 1f, scale);
            //Debug.Log("Floor spawned.");
        }

        List<Edge> edges = maze.GetEdges();

        foreach (Edge edge in edges)
        {
            if (edge.GetWallStatus())
            {
                Pose pose = edge.GetWallPose();
                Instantiate(wallPrefab, pose.position, pose.rotation, GameplayManager.instance.dynamicContainer);
            }
        }

        float halfMazeExtent = (cellSize / 2f) * (mazeSize);
        float halfCellAdjustedExtent = (cellSize / 2f) * (mazeSize - 1);
        for (int i = 0; i < mazeSize - 1; i++)
        {
            // Upper main wall
            Instantiate(wallPrefab, new Vector3(-halfCellAdjustedExtent + (i * cellSize), (cellSize / 2f), halfMazeExtent), Quaternion.identity, GameplayManager.instance.dynamicContainer);
            // Lower main wall
            Instantiate(wallPrefab, new Vector3(-halfCellAdjustedExtent + (i * cellSize), (cellSize / 2f), -halfMazeExtent), Quaternion.identity, GameplayManager.instance.dynamicContainer);
            // Right main wall
            Instantiate(wallPrefab, new Vector3(halfMazeExtent, (cellSize / 2f), halfCellAdjustedExtent - (i * cellSize)), Quaternion.Euler(0f, 90f, 0f), GameplayManager.instance.dynamicContainer);
            // Left main wall
            Instantiate(wallPrefab, new Vector3(-halfMazeExtent, (cellSize / 2f), halfCellAdjustedExtent - (i * cellSize)), Quaternion.Euler(0f, 90f, 0f), GameplayManager.instance.dynamicContainer);
        }

        int j = mazeSize - 1;
        // Final lower main wall
        Instantiate(wallPrefab, new Vector3(-halfCellAdjustedExtent + (j * cellSize), (cellSize / 2f), -halfMazeExtent), Quaternion.identity, GameplayManager.instance.dynamicContainer);
        // Final right main wall
        Instantiate(wallPrefab, new Vector3(halfMazeExtent, (cellSize / 2f), halfCellAdjustedExtent - (j * cellSize)), Quaternion.Euler(0f, 90f, 0f), GameplayManager.instance.dynamicContainer);
        // Final left main wall
        Instantiate(wallPrefab, new Vector3(-halfMazeExtent, (cellSize / 2f), halfCellAdjustedExtent - (j * cellSize)), Quaternion.Euler(0f, 90f, 0f), GameplayManager.instance.dynamicContainer);
        // Final upper main wall (exit)
        exitWall = Instantiate(exitWallPrefab, new Vector3(-halfCellAdjustedExtent + (j * cellSize), (cellSize / 2f), halfMazeExtent), Quaternion.identity, GameplayManager.instance.dynamicContainer);

        mazeGenerated = true;
    }

    public void GenerateWilsonMaze(Graph maze)
    {
        List<Node> unvisited = maze.GetNodes();

        HashSet<Node> visited = new HashSet<Node>();

        Node first = unvisited[UnityEngine.Random.Range(0, unvisited.Count)];
        visited.Add(first);
        unvisited.Remove(first);

        while(unvisited.Count > 0)
        {
            Node currentNode = unvisited[UnityEngine.Random.Range(0, unvisited.Count)];

            Dictionary<Node, (Edge, Node)> path = new Dictionary<Node, (Edge, Node)>();
            
            Node current = currentNode;
            while(!visited.Contains(current))
            {
                Edge nextEdge = maze.GetRandomEdge(current);
                Node nextNode = nextEdge.GetOtherNode(current);

                path[current] = (nextEdge, nextNode);
                current = nextNode;
            }

            Node step = currentNode;
            while(path.ContainsKey(step))
            {
                (Edge, Node) next = path[step];

                next.Item1.SetWallStatus(false);

                visited.Add(step);
                unvisited.Remove(step);
                step = next.Item2;
            }
        }
    }
}

public class Graph
{
    private Dictionary<Node, List<Edge>> adjacencyList;
    private Node[,] nodes;
    private int size;
    private float cellSize;

    // Full maze constructor
    public Graph(int size, float cellSize)
    {
        adjacencyList = new Dictionary<Node, List<Edge>>();
        nodes = new Node[size, size];
        this.size = size;
        this.cellSize = cellSize;

        for(int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                Node node = new(i, j);
                AddNode(node);
                nodes[i, j] = node;
            }
        }

        float verticalStartX = -(cellSize / 2f) * (size - 2);
        float verticalStartZ = (cellSize / 2f) * (size - 1);
        float horizontalStartX = -(cellSize / 2f) * (size - 1);
        float horizontalStartZ = (cellSize / 2f) * (size - 2);

        for(int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Node current = nodes[i, j];

                if (j < size - 1) // vertical walls
                {
                    Node right = nodes[i, j + 1];
                    Pose pose = new Pose(new Vector3(verticalStartX + (j * cellSize), (cellSize / 2f), verticalStartZ - (i * cellSize)), Quaternion.Euler(0f, 90f, 0f));
                    AddEdge(current, right, pose);
                }

                if (i < size - 1) // horizonal walls
                {
                    Node down = nodes[i + 1, j];
                    Pose pose = new Pose(new Vector3(horizontalStartX + (j * cellSize), (cellSize / 2f), horizontalStartZ - (i * cellSize)), Quaternion.identity);
                    AddEdge(current, down, pose);
                }
            }
        }
    }

    public void AddNode(Node node)
    {
        if (!adjacencyList.ContainsKey(node)) adjacencyList[node] = new List<Edge>();
    }

    public void AddEdge(Node node1, Node node2, Pose pose)
    {
        if (node1.Equals(node2)) return;

        AddNode(node1);
        AddNode(node2);

        Edge edge = new(node1, node2, pose);

        adjacencyList[node1].Add(edge);
        adjacencyList[node2].Add(edge);
    }

    public Node GetNode(int rowId, int columnId)
    {
        if(rowId < 0 || rowId >= nodes.GetLength(0) || columnId < 0 || columnId >= nodes.GetLength(1)) throw new ArgumentOutOfRangeException("Row or Column ID is out of bounds.");
        return nodes[rowId, columnId];
    }

    public Edge GetRandomEdge(Node node)
    {
        if(!adjacencyList.ContainsKey(node)) throw new ArgumentException("Node does not exist in the graph.");
        List<Edge> edges = adjacencyList[node];
        if(edges.Count == 0) throw new InvalidOperationException("Node has no edges.");

        return edges[UnityEngine.Random.Range(0, edges.Count)];
    }

    public List<Node> GetNodes()
    {
        return adjacencyList.Keys.ToList<Node>();
    }

    public List<Edge> GetEdges()
    {
        HashSet<Edge> uniqueEdges = new HashSet<Edge>();

        foreach (List<Edge> edgelist in adjacencyList.Values)
        {
            foreach (Edge edge in edgelist)
            {
                uniqueEdges.Add(edge);
            }
        }

        return new List<Edge>(uniqueEdges);
    }
}

public class Node
{
    private readonly int rowId;
    private readonly int columnId;

    public Node(int rowId, int columnId)
    {
        this.rowId = rowId;
        this.columnId = columnId;
    }

    public override bool Equals(object obj)
    {
        return obj is Node other && rowId == other.rowId && columnId == other.columnId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(rowId, columnId);
    }
}

public class Edge
{
    private Node node1;
    private Node node2;
    private bool wallStatus;
    Pose pose;

    public Edge(Node node1, Node node2, Pose pose)
    {
        this.node1 = node1;
        this.node2 = node2;
        this.wallStatus = true;
        this.pose = pose;
    }

    public Node GetOtherNode(Node current)
    {
        if (current.Equals(node1)) return node2;
        else if (current.Equals(node2)) return node1;
        else throw new ArgumentException("Current ID does not match either node.");
    }

    public bool GetWallStatus()
    {
        return wallStatus;
    }

    public void SetWallStatus(bool status)
    {
        this.wallStatus = status;
    }

    public Pose GetWallPose()
    {
        return pose;
    }
}
