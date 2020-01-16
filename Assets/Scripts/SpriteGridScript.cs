using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MustHave.Utilities;

[RequireComponent(typeof(Grid))]
public class SpriteGridScript : MonoBehaviour
{
    [SerializeField] private bool _activeOnStart = default;

    private Grid _grid = default;
    private List<Vector3Int> _spriteCells = new List<Vector3Int>();

    public List<Vector3Int> SpriteCells { get => _spriteCells; }
    public Grid Grid { get => _grid; }

    private void Awake()
    {
        _grid = GetComponent<Grid>();

        foreach (Transform child in transform)
        {
            if (child.GetComponent<SpriteRenderer>())
            {
                Vector3Int cell = _grid.LocalToCell(child.localPosition);
                cell.z = 0;
                _spriteCells.Add(cell);
            }
        }
    }

    private void Start()
    {
        gameObject.SetActive(_activeOnStart);
    }
}
