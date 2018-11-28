using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
	int counter = 0;
	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.O))
		{
			TakeScreenshot();
		}
	}

	void TakeScreenshot()
	{
		ScreenCapture.CaptureScreenshot("Screenshot" + counter + ".png");
		counter++;
	}
}
