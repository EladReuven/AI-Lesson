using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace AstarPathfinding
{
    public class Grid : MonoBehaviour
    {
        Node[,] _grid;

        [SerializeField] private Vector2 _gridWorldSize;
        [SerializeField] private float _nodeRadius;
        [SerializeField] private TerrainType[] _walkableTerrains;
        [SerializeField] private LayerMask _unwalkableMask;

        private LayerMask _walkableLayerMask;
        Dictionary<int, int> _walkableMaskDictionary = new Dictionary<int, int>();
        float NodeDiameter => _nodeRadius * 2;
        int GridSizeX => Mathf.RoundToInt(_gridWorldSize.x / NodeDiameter);
        int GridSizeY => Mathf.RoundToInt(_gridWorldSize.y / NodeDiameter);

        public int MaxSize => GridSizeX * GridSizeY;

        [Header("Gizmos/Debugging")]
        public Node endNode;

        private void Awake()
        {
            foreach (var t in _walkableTerrains)
            {
                _walkableLayerMask.value |= t.terrainLayerMask.value;
                _walkableMaskDictionary.Add((int)Mathf.Log(t.terrainLayerMask.value, 2), t.terrainPenalty);
            }
            CreateGrid();
        }

        [ContextMenu("Create Grid")]
        void CreateGrid()
        {
            _grid = new Node[GridSizeX, GridSizeY];

            Vector3 worldBottomLeft = transform.position - Vector3.right * _gridWorldSize.x / 2 - Vector3.forward * _gridWorldSize.y / 2;

            for (int x = 0; x < GridSizeX; x++)
            {
                for (int y = 0; y < GridSizeY; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * NodeDiameter + _nodeRadius) + Vector3.forward * (y * NodeDiameter + _nodeRadius);
                    bool walkable = !Physics.CheckBox(worldPoint, Vector3.one * _nodeRadius, Quaternion.identity, _unwalkableMask);
                    int movementPenalty = 0;

                    Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100, _walkableLayerMask))
                    {
                        _walkableMaskDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                    }

                    _grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
                }
            }

            BlurPenaltyMap(2);
        }

        public Node NodeFromWorldPoint(Vector3 worldPosition)
        {
            Vector3 worldBottomLeft = transform.position - Vector3.right * _gridWorldSize.x / 2 - Vector3.forward * _gridWorldSize.y / 2;

            float percentX = (worldPosition.x - worldBottomLeft.x) / _gridWorldSize.x;
            float percentY = (worldPosition.z - worldBottomLeft.z) / _gridWorldSize.y;

            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.FloorToInt(GridSizeX * percentX);
            int y = Mathf.FloorToInt(GridSizeY * percentY);

            return _grid[x, y];
        }

        void BlurPenaltyMap(int blurSize)
        {
            int kernelSize = blurSize * 2 + 1;
            int kernelExtents = (kernelSize - 1) / 2;

            int[,] penaltiesHorizontalPass = new int[GridSizeX, GridSizeY];
            int[,] penaltiesVerticalPass = new int[GridSizeX, GridSizeY];

            for (int y = 0; y < GridSizeY; y++)
            {
                for (int x = -kernelExtents; x <= kernelExtents; x++)
                {
                    int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                    penaltiesHorizontalPass[0, y] += _grid[sampleX, y].Penalty;
                }

                for (int x = 1; x < GridSizeX; x++)
                {
                    int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, GridSizeX);
                    int addIndex = Mathf.Clamp(x + kernelExtents, 0, GridSizeX - 1);

                    penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] - _grid[removeIndex, y].Penalty + _grid[addIndex, y].Penalty;
                }
            }

            for (int x = 0; x < GridSizeX; x++)
            {
                for (int y = -kernelExtents; y <= kernelExtents; y++)
                {
                    int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                    penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
                }

                for (int y = 1; y < GridSizeY; y++)
                {
                    int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, GridSizeY);
                    int addIndex = Mathf.Clamp(y + kernelExtents, 0, GridSizeY - 1);

                    penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];
                    int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                    _grid[x, y].Penalty = blurredPenalty;
                }
            }
        }


        public List<Node> GetNeighbors(Node nodeToCheck)
        {
            List<Node> neighborNodes = new List<Node>();
            for (int x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;

                    //if not out of range
                    //if x > 0 < gridx, y > 0 < gridy
                    int xToCheck = nodeToCheck.GridX + x;
                    int yToCheck = nodeToCheck.GridY + y;
                    if (xToCheck >= 0 && xToCheck < GridSizeX && yToCheck >= 0 && yToCheck < GridSizeY)
                        neighborNodes.Add(_grid[xToCheck, yToCheck]);

                    //if (_grid[x,y] != null)
                    //{
                    //    neighborNodes.Add(_grid[x, y]);
                    //}
                }
            return neighborNodes;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position, new Vector3(_gridWorldSize.x, 1, _gridWorldSize.y));

            if (_grid == null)
                return;
            foreach (Node node in _grid)
            {
                Gizmos.color = node.Walkable ? Color.white : Color.red;

                Gizmos.DrawWireCube(node.WorldPosition, Vector3.one * (NodeDiameter - 0.05f));
            }
        }
    }

    [Serializable]
    public class TerrainType
    {
        public LayerMask terrainLayerMask;
        public int terrainPenalty;
    }
}
