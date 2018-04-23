using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightPulse : MonoBehaviour {

    public float period;
    public float minimum;
    public float maximum;

    Light _light;

	void Start () {
        _light = GetComponent<Light>();
	}
	
	void Update () {
        _light.intensity = Mathf.Sin(Time.time * Mathf.PI / period) * (maximum - minimum) + minimum;
	}
}
