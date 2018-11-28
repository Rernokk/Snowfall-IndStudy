using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshotter : MonoBehaviour {
	int screenCounter = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.O)){
			ScreenCapture.CaptureScreenshot("Screenshot" + screenCounter + ".jpg");
			screenCounter++;
			print("Screenshot saved: " + screenCounter);
		}
	}
}
