using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BlitScript : MonoBehaviour {
	[SerializeField]
	Shader repShader;

	void OnEnable(){
		if (repShader != null){
			GetComponent<Camera>().SetReplacementShader(repShader, "");
		}
	}

	void OnDisable(){
		GetComponent<Camera>().ResetReplacementShader();
	}
}
