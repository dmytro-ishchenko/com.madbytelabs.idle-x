using System;
using System.Collections;
using System.Collections.Generic;
using Exoa.Cameras;
using Exoa.Events;
using UnityEngine;
using UnityEngine.UI;
using static CameraActionButton;

[AddComponentMenu("Exoa/Demo/CustomControlUIZoomSliderDemo")]
public class CustomControlUIZoomSliderDemo : MonoBehaviour
{
    public Slider zoomSlider;
    private CameraPerspective cam1;

    void Start()
    {
        cam1 = GetComponent<CameraPerspective>();
        zoomSlider.minValue = cam1.minMaxDistance.x;
        zoomSlider.maxValue = cam1.minMaxDistance.y;
        zoomSlider.onValueChanged.AddListener(Zoom);
    }

    private void Zoom(float v)
    {
        cam1.MoveCameraToInstant(Mathf.Clamp(v, cam1.minMaxDistance.x, cam1.minMaxDistance.y));
    }

    private void LateUpdate()
    {
        zoomSlider.value = cam1.FinalDistance;
    }



}
