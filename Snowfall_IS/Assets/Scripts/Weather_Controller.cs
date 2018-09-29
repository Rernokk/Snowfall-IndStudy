using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather_Controller : MonoBehaviour {
	[SerializeField]
	FogVolume fog;

	[SerializeField]
	float startVis = 200;
	// Use this for initialization
	void Start () {
		startVis = fog.Visibility;
	}
	
	// Update is called once per frame
	void Update () {
		if (startVis >= 40)
		{
			startVis -= Time.deltaTime * 5f;
			fog.Visibility = Mathf.Clamp(startVis, 40, 500);
		}
	}
}
