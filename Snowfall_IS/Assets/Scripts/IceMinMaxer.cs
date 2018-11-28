using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceMinMaxer : MonoBehaviour {
	void Start() {
		MeshRenderer mRenderer = GetComponent<MeshRenderer>();
		Texture texRef = mRenderer.material.GetTexture("_NoiseTex");
		float min = 1;
		float max = 0;
		for (int i = 0; i < texRef.width; i++){
			for (int j = 0; j < texRef.height; j++){
				Color col = (texRef as Texture2D).GetPixel(i, j);
				if (col.r < min) {
					min = col.r;
				}
				if (col.r > max){
					max = col.r;
				}
			}
		}

		mRenderer.material.SetFloat("_Lowest", min);
		mRenderer.material.SetFloat("_Highest", max);
	}
}
