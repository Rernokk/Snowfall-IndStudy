using UnityEngine;

public class BlurTestScript : MonoBehaviour
{
	private Texture2D tex;

	[SerializeField]
	private bool applyBlur = false;

	[SerializeField]
	private int amnt;

	[SerializeField]
	private Terrain terrain;

	[SerializeField]
	private bool includeTerrain, includeGradientBias;

	[SerializeField]
	private float weightScale = 1f, gradientScale = 1f;

	// Use this for initialization
	private void Start()
	{
		tex = new Texture2D(256, 256);
		Vector2[,] vecArray = new Vector2[tex.width, tex.height];
		for (int i = 0; i < tex.width; i++)
		{
			for (int j = 0; j < tex.height; j++)
			{
				vecArray[i, j] = Vector2.zero;
			}
		}

		if (terrain != null)
		{
			for (int i = 0; i < tex.width; i++)
			{
				for (int j = 0; j < tex.height; j++)
				{
					Vector2 val = Vector2.zero;
					if (includeTerrain)
					{
						Vector3 res = Vector3.zero;
						res += terrain.terrainData.GetInterpolatedNormal((float)Mathf.Clamp((i), 0, tex.width) / tex.width,
							(float)Mathf.Clamp((j), 0, tex.height) / tex.height) * weightScale;
						val.x -= res.x;
						val.y -= res.z;
					}

					if (includeGradientBias)
					{
						Vector3 gradientBias = transform.up * gradientScale;
						val.x -= gradientBias.x;
						val.y -= gradientBias.z;
					}
					vecArray[i, j] = new Vector2(val.x, val.y);
				}
			}
		}

		//vecArray[64, 64] = Vector2.one;
		//vecArray[63, 64] = new Vector2(-1, 1);
		//vecArray[64, 63] = new Vector2(1, -1);
		//vecArray[63, 63] = -Vector2.one;
		//for (int i = 0; i < 10; i++)
		//{
		//	vecArray[64, 69 + i] = new Vector2(1, 0);
		//	vecArray[63, 69 + i] = new Vector2(-1, 0);
		//}

		Vector2[,] backupArray = new Vector2[tex.width, tex.height];
		for (int i = 0; i < tex.width; i++)
		{
			for (int j = 0; j < tex.height; j++)
			{
				backupArray[i, j] = vecArray[i, j];
			}
		}

		if (applyBlur)
		{
			for (int k = 0; k < tex.width; k++)
			{
				for (int j = 0; j < tex.height; j++)
				{
					Vector2 origCol = backupArray[k, j];
					if (origCol != Vector2.zero)
					{
						amnt = 24;
						for (int i = 0; i < amnt; i++)
						{
							int indX = k + i * (int)(backupArray[k, j].x != 0 ? Mathf.Sign(backupArray[k, j].x) : 0);
							int indY = j + i * (int)(backupArray[k, j].y != 0 ? Mathf.Sign(backupArray[k, j].y) : 0);
							if (indY >= 0 && indX >= 0 && indX < tex.width && indY < tex.height)
							{
								vecArray[indX, indY] += backupArray[k, j] * (amnt - i) / amnt;
							}
						}
					}
				}
			}

			for (int i = 0; i < tex.width; i++)
			{
				for (int j = 0; j < tex.height; j++)
				{
					Vector2 clamped = vecArray[i, j];
					clamped += Vector2.one;
					clamped *= .5f;
					Color col = new Color(Mathf.Clamp01(clamped.x), Mathf.Clamp01(clamped.y), 0);
					tex.SetPixel(i, j, col);
				}
			}
			tex.Apply();
		}
		tex.filterMode = FilterMode.Trilinear;
		tex.wrapMode = TextureWrapMode.Repeat;

		FastNoise myNoise = new FastNoise((int)Time.time);
		myNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
		Texture2D albedo = new Texture2D(1024, 1024);
		for (int i = 0; i < albedo.width; i++){
			for (int j = 0; j < albedo.height; j++){
				float per = myNoise.GetPerlin(i, j);
				albedo.SetPixel(i, j, new Color(per, per, per, 1));
			}
		}
		albedo.wrapMode = TextureWrapMode.Mirror;
		albedo.Apply();

		GetComponent<MeshRenderer>().material.SetTexture("_FlowTex", tex);
		GetComponent<MeshRenderer>().material.SetTexture("_MainTex", albedo);
	}

}
