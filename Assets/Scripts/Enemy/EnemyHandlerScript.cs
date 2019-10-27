//#define DEBUG_RANDOM_CELL_POSITION

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MustHave.Utilities;

public class EnemyHandlerScript : MonoBehaviour
{
    [SerializeField, Range(1, 10)]
    private int enemiesCount = default;
    [SerializeField]
    private EnemyScript[] _enemyPrefabs = default;
    [SerializeField]
    private Transform _target = default;
    [SerializeField]
    private Transform _particlesContainer = default;
    [SerializeField]
    private SpriteGridScript _groundFloorGrid = default;
    [SerializeField]
    private SpriteGridScript _firstFloorGrid = default;

    private List<Vector3Int> _groundFloorGridCellsPool = default;
    private List<Vector3Int> _firstFloorGridCellsPool = default;
    private EnemyScript _debugEnemy = default;
    private EnemyScript _debugEnemyPrefab = default;
    private Dictionary<int, EnemyScript> _enemyPrefabsDict = new Dictionary<int, EnemyScript>();

    public Transform Target { get => _target; }
    public Transform ParticlesContainer { get => _particlesContainer; }

    private void Start()
    {
        _groundFloorGridCellsPool = new List<Vector3Int>(_groundFloorGrid.SpriteCells);
        _firstFloorGridCellsPool = new List<Vector3Int>(_firstFloorGrid.SpriteCells);
        int minHealth = int.MaxValue;
        int maxHealth = 0;
        foreach (var enemyPrefab in _enemyPrefabs)
        {
            _enemyPrefabsDict.Add(enemyPrefab.FullHealthPoints, enemyPrefab);
            minHealth = Mathf.Min(minHealth, enemyPrefab.FullHealthPoints);
            maxHealth = Mathf.Max(maxHealth, enemyPrefab.FullHealthPoints);
        }
        int halfEnemiesCount = enemiesCount / 2;
        for (int i = 0; i < halfEnemiesCount; i++)
        {
            CreateEnemyAtRandomCell(minHealth);
            CreateEnemyAtRandomCell(maxHealth);
        }
        if (enemiesCount % 2 == 1)
        {
            CreateEnemyAtRandomCell(minHealth);
        }
    }

#if DEBUG_RANDOM_CELL_POSITION
    private void Update()
    {
        _debugEnemy.transform.position = GetEnemyRandomCellPosition(_debugEnemyPrefab);
    }
#endif

    public EnemyScript CreateEnemyAtRandomCell(int fullHealthPoints)
    {
        if (_enemyPrefabsDict.TryGetValue(fullHealthPoints, out EnemyScript enemyPrefab))
        {
            return CreateEnemyAtRandomCell(enemyPrefab);
        }
        return null;
    }

    private EnemyScript CreateEnemyAtRandomCell(EnemyScript prefab)
    {
        _debugEnemyPrefab = prefab;
        return _debugEnemy = Instantiate(prefab, GetEnemyRandomCellPosition(prefab), Quaternion.identity, transform);
    }

    private void GetPlayerFloorGrid(out List<Vector3Int> cellsPool, out SpriteGridScript grid)
    {
        if (Mathf.Abs(_target.position.y - _groundFloorGrid.transform.position.y) < Mathf.Abs(_target.position.y - _firstFloorGrid.transform.position.y))
        {
            cellsPool = _groundFloorGridCellsPool;
            grid = _groundFloorGrid;
        }
        else
        {
            cellsPool = _firstFloorGridCellsPool;
            grid = _firstFloorGrid;
        }
    }

    private Vector3 GetEnemyRandomCellPosition(EnemyScript prefab)
    {
        GetPlayerFloorGrid(out List<Vector3Int> cellsPool, out SpriteGridScript grid);
        Vector3Int playerCell = grid.Grid.WorldToCell(_target.position);
        playerCell.z = 0;
        List<Vector3Int> occupiedCells = new List<Vector3Int>();
        occupiedCells.Add(playerCell);
        foreach (Transform child in transform)
        {
            Vector3Int childCell = grid.Grid.WorldToCell(child.position);
            childCell.z = 0;
            occupiedCells.Add(childCell);
        }
        occupiedCells.ForEach(cell => { cellsPool.Remove(cell); });
        if (cellsPool.Count == 0)
        {
            cellsPool.AddRange(grid.SpriteCells);
            occupiedCells.ForEach(cell => { cellsPool.Remove(cell); });
        }
        Vector3Int randomCell = cellsPool.PickRandomElement();
        Vector3 cellPos = grid.Grid.GetCellCenterWorld(randomCell);
        return new Vector3(cellPos.x, prefab.transform.localPosition.y + grid.transform.position.y, cellPos.z); ;
    }
}
