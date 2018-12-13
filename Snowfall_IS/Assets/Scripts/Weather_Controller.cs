using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather_Controller : MonoBehaviour {
	[SerializeField]
	FogVolume fog;

	[SerializeField]
	float startVis = 200;

	[SerializeField]
	Material treeMat, groundMat, distortionMat;

	[SerializeField]
	float snowFactorLimit = 0.05f;
	float progress = 0;

	// Use this for initialization
	void Start () {
		startVis = fog.Visibility;
		Shader.SetGlobalFloat("_StoneGloss", 0);
		Shader.SetGlobalFloat("_BushSnowAmnt", 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (startVis >= 40)
		{
			startVis -= Time.deltaTime * 5f;
			fog.Visibility = Mathf.Clamp(startVis, 40, 500);
			progress += Time.deltaTime / 32f;
			Shader.SetGlobalFloat("_StoneGloss", progress * .5f);
			Shader.SetGlobalFloat("_BushSnowAmnt", progress * .5f);
			distortionMat.SetFloat("_DistortionRate", Mathf.Clamp(1.0f - progress * .5f, .2f, 1f));
			Shader.SetGlobalFloat("_SnowAccum", Mathf.Clamp01(progress));
			groundMat.SetFloat("_SnowCover", Mathf.Clamp01(progress*.5f));
			Shader.SetGlobalFloat("_Factor", Mathf.Clamp(snowFactorLimit * progress, 0, snowFactorLimit));
		}
	}
}
