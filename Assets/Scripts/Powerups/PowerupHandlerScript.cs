//#define DEBUG_RANDOM_CELL_POSITION

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MustHave;
using MustHave.Utilities;
using UnityEngine.UI;

public class PowerupHandlerScript : MonoBehaviour
{
    [SerializeField, Range(1, 3)]
    private int _damagePowerupsCount = default;
    [SerializeField, Range(1, 3)]
    private int _speedPowerupsCount = default;
    [SerializeField, Range(1, 3)]
    private int _flyPowerupsCount = default;
    [SerializeField]
    private TextMessageEvent _logMessage = default;
    [SerializeField]
    private PowerupScript[] _powerupPrefabs = default;
    [SerializeField]
    private SpriteGridScript _floorGrid = default;

    private Dictionary<Type, PowerupScript> _powerupPrefabsDict = new Dictionary<Type, PowerupScript>();
    private Dictionary<Type, PowerupScript> _powerupsDict = new Dictionary<Type, PowerupScript>();
    private List<Vector3Int> _floorGridCellsPool = default;

    private PowerupScript _debugPowerup = default;

    private void Awake()
    {
        foreach (var prefab in _powerupPrefabs)
        {
            _powerupPrefabsDict.Add(prefab.GetType(), prefab);
        }
    }

    private void Start()
    {
        _floorGridCellsPool = new List<Vector3Int>(_floorGrid.SpriteCells);
        for (int i = 0; i < _speedPowerupsCount; i++)
        {
            CreatePowerupAtRandomCell<SpeedPowerupScript>();
        }
        for (int i = 0; i < _damagePowerupsCount; i++)
        {
            CreatePowerupAtRandomCell<DamagePowerupScript>();
        }
        for (int i = 0; i < _flyPowerupsCount; i++)
        {
            CreatePowerupAtRandomCell<FlyPowerupScript>();
        }
    }

    private void Update()
    {
        SetLogMessageData();
    }

    private void SetLogMessageData()
    {
        string log = "";
        foreach (var kvp in _powerupsDict)
        {
            log += kvp.Value.ToString() + "\n";
        }
        _logMessage.Data = log;

#if DEBUG_RANDOM_CELL_POSITION
        if (_debugPowerup)
            _debugPowerup.transform.position = GetPowerupRandomCellPosition(_debugPowerup);
#endif
    }

    public void OnPlayerEnter(PlayerScript player, PowerupScript powerup)
    {
        Type powerupType = powerup.GetType();
        if (!_powerupsDict.ContainsKey(powerupType))
            _powerupsDict.Add(powerupType, powerup);
        else
        {
            PowerupScript activePowerup = _powerupsDict[powerupType];
            if (activePowerup && activePowerup != powerup)
            {
                CreatePowerupAtRandomCellAfterTime(powerup.GetType(), powerup.Duration);
                Destroy(powerup.gameObject);
                powerup.SetChildrenActive(false);
                powerup = activePowerup;
            }
            _powerupsDict[powerupType] = powerup;
        }
        powerup.OnPlayerEnter(player, this);
    }

    public void OnPowerupEnd(PowerupScript powerup)
    {
        Type powerupType = powerup.GetType();
        _powerupsDict.Remove(powerupType);
        float delay = 0f;// UnityEngine.Random.Range(3f, 7f);
        CreatePowerupAtRandomCellAfterTime(powerupType, delay);
    }

    private void CreatePowerupAtRandomCellAfterTime(Type powerupType, float delay)
    {
        this.StartCoroutineActionAfterTime(() => {
            CreatePowerupAtRandomCell(powerupType);
        }, delay);
    }

    private T CreatePowerupAtRandomCell<T>() where T : PowerupScript
    {
        return CreatePowerupAtRandomCell(typeof(T)) as T;
    }

    private PowerupScript CreatePowerupAtRandomCell(Type type)
    {
        PowerupScript prefab = _powerupPrefabsDict[type];
        return _debugPowerup = Instantiate(prefab, GetPowerupRandomCellPosition(prefab), Quaternion.identity, transform);
    }

    private Vector3 GetPowerupRandomCellPosition(PowerupScript prefab)
    {
        if (_floorGridCellsPool.Count == 0)
            _floorGridCellsPool.AddRange(_floorGrid.SpriteCells);
        Vector3Int cell = _floorGridCellsPool.PickRandomElement();
        Vector3 cellPos = _floorGrid.Grid.GetCellCenterWorld(cell);
        float y = prefab.transform.localPosition.y;
        return new Vector3(cellPos.x, y, cellPos.z);
    }
}
