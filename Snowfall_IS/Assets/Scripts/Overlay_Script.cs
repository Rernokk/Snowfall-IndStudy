using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Overlay_Script : MonoBehaviour {
	[SerializeField]
	Image flowMap;

	[SerializeField]
	FlowmapGenerator river;
	Sprite flowMapRef;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (river.generatedTexture != null){
			flowMapRef = Sprite.Create(river.generatedTexture, new Rect(0, 0, river.generatedTexture.width, river.generatedTexture.height), new Vector2(.5f, .5f));
		}
		flowMap.sprite = flowMapRef;
	}
}
