using UnityEngine;

public class IcicleMapGenerator : MonoBehaviour
{
	[SerializeField]
	private Texture2D iceMap;

	[SerializeField]
	private AnimationCurve iceRamp;

	// Use this for initialization
	private void Start()
	{
		int width = 256, height = 256;
		iceMap = new Texture2D(width, height);

		//Default to base black
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				iceMap.SetPixel(i, j, Color.black);
			}
		}

		for (int i = 0; i < 32; i++)
		{
			AddIcicle();
		}
		iceMap.Apply();
		iceMap.filterMode = FilterMode.Bilinear;
		iceMap.wrapMode = TextureWrapMode.Clamp;

		GetComponent<MeshRenderer>().material.SetTexture("_NoiseTex", iceMap);
		GetComponent<MeshRenderer>().material.SetTexture("_MainTex", iceMap);
	}

	private void AddIcicle()
	{
		int radius = 30;
		float iceLength = Random.Range(.6f, 1f);
		Vector2Int tarPos = new Vector2Int(iceMap.width / 2, iceMap.height / 2);
		tarPos = new Vector2Int(Random.Range(radius, iceMap.width - radius), Random.Range(radius, iceMap.height - radius));
		for (int i = -radius; i <= radius; i++)
		{
			for (int j = -radius; j <= radius; j++)
			{
				if (Mathf.Sqrt(i * i + j * j) < radius && i + tarPos.x >= 0 && i + tarPos.x < iceMap.width && j + tarPos.y >= 0 && j + tarPos.y < iceMap.height)
				{
					iceMap.SetPixel(tarPos.x + i, tarPos.y + j, (.25f * iceMap.GetPixel(tarPos.x + i, tarPos.y + j) + Color.white * iceLength * Mathf.FloorToInt(8 * ((radius - Mathf.Floor(Mathf.Sqrt(i * i + j * j))) / radius)) / 8));
				}
			}
		}
	}
}
