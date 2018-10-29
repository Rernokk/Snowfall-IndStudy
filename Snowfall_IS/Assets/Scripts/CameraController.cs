using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	int camIndex = 0;

	[SerializeField]
	float lerpVal = .005f;
	// Use this for initialization
	void Start ()
	{
		Camera.main.transform.position = transform.GetChild(0).position;
		Camera.main.transform.rotation = transform.GetChild(0).rotation;
		Camera.main.depthTextureMode = DepthTextureMode.DepthNormals;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.RightArrow)){
			camIndex++;
			if (camIndex == transform.childCount){
				camIndex = 0;
			}
		}

		if (Input.GetKeyDown(KeyCode.LeftArrow)){
			camIndex--;
			if (camIndex < 0){
				camIndex = transform.childCount - 1;
			}
		}
	}
	
	void LateUpdate(){
		Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, transform.GetChild(camIndex).transform.position, lerpVal);
		Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, transform.GetChild(camIndex).transform.rotation, lerpVal);
	}
}
