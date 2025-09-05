using System.Collections.Generic;

using UnityEngine;


public struct MapData
{
    public  struct Node
    {
        public int XPos;
        public List<Vector2Int> LinkedNodePoses;
    }


    public bool[,] NodePlane;
    public Dictionary<int, List<Node>> Nodes;
}