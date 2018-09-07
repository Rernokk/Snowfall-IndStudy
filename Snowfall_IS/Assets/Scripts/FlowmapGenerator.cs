using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FlowmapGenerator : MonoBehaviour
{
	[SerializeField]
	public Texture2D maskMap, flowMap;

	[SerializeField]
	LayerMask mask;

	[SerializeField]
	float cutFactor = .05f;

	[SerializeField]
	bool applyBlur = false;

	[SerializeField]
	int amnt;

	Material mat;
	Vector2[,] vecArray;
	Vector2[,] backupArray;

	// Use this for initialization
	void Start()
	{
		maskMap = new Texture2D(512, 512);
		flowMap = new Texture2D(512, 512);
		for (int i = 0; i < maskMap.width; i++)
		{
			for (int j = 0; j < maskMap.height; j++)
			{
				RaycastHit inf;
				Physics.Raycast(new Ray(new Vector3(i * 100.0f / maskMap.width, 50, j * 100.0f / maskMap.height), Vector3.down), out inf, 100f);
				if (inf.transform == null || inf.transform.tag == "Terrain")
				{
					maskMap.SetPixel(i, j, Color.black);
				}
				else
				{
					maskMap.SetPixel(i, j, Color.white);
				}
			}
		}

		//CreateFlowMap();

		#region Adaptation
		vecArray = new Vector2[flowMap.width, flowMap.height];
		backupArray = new Vector2[flowMap.width, flowMap.height];

		for (int i = 0; i < flowMap.width; i++)
		{
			for (int j = 0; j < flowMap.height; j++)
			{
				vecArray[i, j] = Vector2.zero;
			}
		}

		CreateFlowMapVTwo();

		for (int i = 0; i < flowMap.width; i++)
		{
			for (int j = 0; j < flowMap.height; j++)
			{
				backupArray[i, j] = vecArray[i, j];
			}
		}

		if (applyBlur)
		{
			for (int k = 0; k < flowMap.width; k++)
			{
				for (int j = 0; j < flowMap.height; j++)
				{
					Vector2 origCol = backupArray[k, j];
					if (origCol != Vector2.zero)
					{
						float falloff = .9f;
						for (int i = 0; i < amnt; i++)
						{
							int indX = k - i * (int)(backupArray[k, j].x != 0 ? Mathf.Sign(backupArray[k, j].x) : 0);
							int indY = j - i * (int)(backupArray[k, j].y != 0 ? Mathf.Sign(backupArray[k, j].y) : 0);
							if (indY >= 0 && indX >= 0 && indX < flowMap.width && indY < flowMap.height)
							{
								//vecArray[indX, indY] += backupArray[k, j] * (amnt - i) / amnt;
								vecArray[indX, indY] += backupArray[k, j] * falloff;
								falloff *= .9f;
							}
						}
					}
				}
			}
		}

		for (int i = 0; i < flowMap.width; i++)
		{
			for (int j = 0; j < flowMap.height; j++)
			{
				Vector2 clamped = vecArray[i, j];
				clamped += Vector2.one;
				clamped *= .5f;
				Color col = new Color(Mathf.Clamp01(clamped.x), Mathf.Clamp01(clamped.y), 0);
				flowMap.SetPixel(i, j, col);
			}
		}
		flowMap.Apply();

		flowMap.filterMode = FilterMode.Trilinear;
		flowMap.wrapMode = TextureWrapMode.Repeat;
		#endregion

		mat = GetComponent<MeshRenderer>().material;
		mat.SetTexture("_FlowTex", flowMap);
	}

	void CreateFlowMap()
	{
		for (int i = 0; i < flowMap.width; i++)
		{
			for (int j = 0; j < flowMap.height; j++)
			{
				if (maskMap.GetPixel(i, j) == Color.white)
				{
					RaycastHit inf;
					Physics.Raycast(new Ray(new Vector3(i * 100.0f / flowMap.width, 40, j * 100.0f / flowMap.height), Vector3.down), out inf, 100f, mask);
					Vector3 normDir = inf.normal;
					normDir.y = 0;
					normDir.Normalize();
					flowMap.SetPixel(i, j, new Color(Mathf.Clamp01(.5f + -normDir.x * cutFactor), Mathf.Clamp01(.5f + -normDir.z * cutFactor), 0));
				}
				else
				{
					flowMap.SetPixel(i, j, new Color(.5f, .5f, 0));
				}
			}
		}
		flowMap.Apply();
	}

	void CreateFlowMapVTwo()
	{
		for (int i = 0; i < flowMap.width; i++)
		{
			for (int j = 0; j < flowMap.height; j++)
			{
				if (maskMap.GetPixel(i, j) == Color.white || true)
				{
					RaycastHit inf;
					Physics.Raycast(new Ray(new Vector3(i * 100.0f / flowMap.width, 40, j * 100.0f / flowMap.height), Vector3.down), out inf, 100f, mask);
					Vector3 normDir = inf.normal;
					normDir.y = 0;
					//normDir.Normalize();
					vecArray[i, j] = new Vector2(-normDir.x * cutFactor, -normDir.z * cutFactor);
				}
			}
		}
	}
}
