using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MustHave;

public class LookAtCameraScript : MonoBehaviour
{
    private void Update()
    {
        Camera camera = CameraUtils.MainOrCurrent;
        transform.rotation = Quaternion.LookRotation(camera.transform.forward, camera.transform.up);
    }
}
