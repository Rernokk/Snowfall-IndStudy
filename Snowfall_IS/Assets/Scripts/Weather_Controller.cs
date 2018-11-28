using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather_Controller : MonoBehaviour {
	[SerializeField]
	FogVolume fog;

	[SerializeField]
	float startVis = 200;

	[SerializeField]
	Material treeMat, groundMat;

	[SerializeField]
	float snowFactorLimit = 0.05f;
	float progress = 0;

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
			progress += Time.deltaTime / 32f;
			treeMat.SetFloat("_SnowAccum", Mathf.Clamp01(progress));
			groundMat.SetFloat("_SnowCover", Mathf.Clamp01(progress*.5f));
			treeMat.SetFloat("_Factor", Mathf.Clamp(snowFactorLimit * progress, 0, snowFactorLimit));
		}
	}
}
