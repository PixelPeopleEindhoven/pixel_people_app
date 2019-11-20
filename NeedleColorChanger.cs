using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

// Answer is 9

public class NeedleColorChanger : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public Slider slider;
    public Material material;
    public GameObject panelToRemove;
    public Waypoint waypoint;

    private bool sliderMoved = false;
    private bool sliderMovedFirstTime = false;
    private bool stop = false;
    // Start is called before the first frame update
    void Start()
    {
        slider.onValueChanged.AddListener(delegate {
                panelToRemove.SetActive(false);
                material.color = Color.HSVToRGB(slider.value, .5f, 1f);
                sliderMoved = true;
                sliderMovedFirstTime = true;
            });
    }


    float period  = 0.0f;
    float period2 = 0.0f;
    void Update()
    {

        if (period2 > 60.0f) {
            period2 = 0;
            waypoint.Visited = true;
            waypoint.Enabled = false;
            stop             = true;
            Debug.Log("1");
        }

        if (sliderMovedFirstTime) period2 += Time.deltaTime;

        if (!sliderMoved) return;

        if (period > 1.0f) {
            period = 0;

            sliderMoved = false;
            var scene = Math.Floor((21.0 / slider.maxValue) * slider.value);
            var URL   = "http://vps736303.ovh.net:3000/" + scene.ToString();
            Debug.Log("scene< " + scene);
            UnityWebRequest www = UnityWebRequest.Get(URL);
            www.SendWebRequest();
        }
        period += Time.deltaTime;

    }
}
