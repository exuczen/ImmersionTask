using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerupScript : MonoBehaviour
{
    [SerializeField]
    protected string _logName = default;
    [SerializeField]
    private float _duration = default;
    [SerializeField]
    protected float _multiplier = default;

    private Coroutine _routine = default;
    private float _startTime = default;
    private float _elapsedTime = default;
    protected string _logPrefix = default;

    private float TimeLeft { get => _duration - _elapsedTime; }
    public float Duration { get => _duration; }

    public override string ToString() { return _logPrefix + TimeLeft.ToString("F2"); }

    private void Start()
    {
        _logPrefix = "+" + (int)(100 * (_multiplier - (int)_multiplier)) + "% " + _logName + " : ";
    }

    private IEnumerator PowerupRoutine(PlayerScript player, PowerupHandlerScript handler)
    {
        yield return new WaitWhile(() => (_elapsedTime = Time.time - _startTime) < _duration);
        OnPowerupEnd(player);
        handler.OnPowerupEnd(this);
        Destroy(gameObject);
        _routine = null;
    }

    public void OnPlayerEnter(PlayerScript player, PowerupHandlerScript handler)
    {
        OnPowerupStart(player);
        if (_routine == null)
        {
            SetChildrenActive(false);
            _startTime = Time.time;
            _routine = StartCoroutine(PowerupRoutine(player, handler));
        }
        else
        {
            _startTime += _duration;
        }
    }

    public void SetChildrenActive(bool active)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void StopCoroutine()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }
    }

    private void OnDestroy()
    {
        StopCoroutine();
    }

    protected abstract void OnPowerupStart(PlayerScript player);

    protected abstract void OnPowerupEnd(PlayerScript player);
}
