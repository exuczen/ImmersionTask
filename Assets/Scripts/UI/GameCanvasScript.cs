using MustHave;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvasScript : MonoBehaviour
{
    [SerializeField] private MessageBus _messageBus = default;
    [SerializeField] private TextMessageEvent _playerStateMessage = default;
    [SerializeField] private TextMessageEvent _powerupLogMessage = default;
    [SerializeField] private BoolMessageEvent _enemyTargetMessage = default;
    [SerializeField] private Text _playerStateText = default;
    [SerializeField] private Text _powerupLogText = default;
    [SerializeField] private Image _targetImage = default;
    [SerializeField] private Text _fpsText = default;

    private float _fps = default;

    private void Awake()
    {
        _messageBus.Register(OnNotify);
    }

    private void OnNotify(MessageEvent messageEvent)
    {
    }

    private void Update()
    {
        _targetImage.transform.position = Input.mousePosition;

        float deltaTime = Time.deltaTime;
        if (deltaTime > 0f)
        {
            _fps = Mathf.Lerp(_fps, 1f / deltaTime, 5f * deltaTime);
            _fpsText.text = "FPS: " + _fps.ToString("F2");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    private void LateUpdate()
    {
        _targetImage.color = _enemyTargetMessage.Data ? Color.red : Color.black;
        _playerStateText.text = _playerStateMessage.Data;
        _powerupLogText.text = _powerupLogMessage.Data;
    }
}
