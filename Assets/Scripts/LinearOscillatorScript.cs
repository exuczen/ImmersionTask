using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class LinearOscillatorScript : MonoBehaviour
{
    [SerializeField]
    private Axis _axis = default;
    [SerializeField]
    private bool _absolute = default;
    [SerializeField]
    private float _frequency = 1f;
    [SerializeField]
    private float _amplitude = 1f;

    private const float PI2 = (Mathf.PI * 2f);

    private Vector3 _initialPosition = default;
    private Action<float> _setTranslationAlongAxis = default;

    private void Start()
    {
        _initialPosition = transform.localPosition;
        switch (_axis)
        {
            case Axis.X:
                _setTranslationAlongAxis = SetTranslationX;
                break;
            case Axis.Y:
                _setTranslationAlongAxis = SetTranslationY;
                break;
            case Axis.Z:
                _setTranslationAlongAxis = SetTranslationZ;
                break;
            default:
                _setTranslationAlongAxis = translation => { };
                break;
        }
    }

    private void Update()
    {
        float sinOmegaTime = Mathf.Sin((_frequency * Time.time) % PI2);
        float translation = _amplitude * (_absolute ? Mathf.Abs(sinOmegaTime) : sinOmegaTime);
        _setTranslationAlongAxis.Invoke(translation);
    }

    private void SetTranslationX(float translation)
    {
        transform.localPosition = _initialPosition + new Vector3(translation, 0f, 0f);
    }
    private void SetTranslationY(float translation)
    {
        transform.localPosition = _initialPosition + new Vector3(0f, translation, 0f);
    }

    private void SetTranslationZ(float translation)
    {
        transform.localPosition = _initialPosition + new Vector3(0f, 0f, translation);
    }
}
