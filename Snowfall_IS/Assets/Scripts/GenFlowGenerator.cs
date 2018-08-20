using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenFlowGenerator : MonoBehaviour {
	Texture2D genMap;

	[SerializeField]
	Gradient grad;

	[SerializeField]
	List<Transform> nodes = new List<Transform>();

	// Use this for initialization
	void Start () {
		genMap = new Texture2D(100, 100);
		for (int i = 0; i < genMap.width; i++){
			for (int j = 0; j < genMap.height; j++){
				genMap.SetPixel(i, j, new Color(.5f, .5f, 0));
			}
		}
		genMap.wrapMode = TextureWrapMode.Clamp;
		genMap.Apply();

		Color neutralColor = genMap.GetPixel(0, 0);

		float fullLength = 0;
		for (int i = 0; i < nodes.Count - 2; i++){
			fullLength += Vector3.Distance(nodes[i].transform.position, nodes[i + 1].transform.position);
		}

		float dist = 0;
		for (int i = 0; i < nodes.Count - 1; i++){
			float tempDist = dist;
			for (int j = 0; j < genMap.height * 2; j++){
				float amnt = (float) j / ((float) genMap.height * 2.0f);
				Vector3 botLeft = transform.position - GetComponent<Collider>().bounds.extents;
				Vector3 topRight = transform.position + GetComponent<Collider>().bounds.extents;
				botLeft.y = 0;
				topRight.y = 0;
				Vector3 refVector = Vector3.Lerp(nodes[i].transform.position, nodes[i + 1].transform.position, amnt);
				tempDist = dist + Vector3.Distance(nodes[i].transform.position, Vector3.Lerp(nodes[i].transform.position, nodes[i + 1].transform.position, amnt));
				int resX = (int)(genMap.width * ((refVector.x - botLeft.x) / (topRight.x - botLeft.x)));
				int resY = (int)(genMap.height * ((refVector.z - botLeft.z) / (topRight.z - botLeft.z)));
				for (int a = -2; a <= 2; a++)
				{
					for (int b = -2; b <= 2; b++)
					{
						if (genMap.GetPixel(a + resX, resY + b).Equals(neutralColor))
						{
							genMap.SetPixel(resX + a, resY + b, new Color(.5f * (Vector3.Dot(Vector3.forward, (nodes[i + 1].position - nodes[i].position).normalized) + 1.0f), .5f * (Vector3.Dot(Vector3.right, (nodes[i + 1].position - nodes[i].position).normalized) + 1.0f), 0));
						}
					}
				}
			}
			dist += Vector3.Distance(nodes[i].position, nodes[i + 1].position);
		}
		genMap.Apply();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
