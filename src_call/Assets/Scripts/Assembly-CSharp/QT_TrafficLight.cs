using UnityEngine;

public class QT_TrafficLight : MonoBehaviour
{
	[HideInInspector]
	public Material trafficlightMaterial;

	[HideInInspector]
	public bool showLinks;

	[HideInInspector]
	public Color linkColor;

	[HideInInspector]
	public Vector3 controllerPosition;

	[HideInInspector]
	public GameObject[] Lights = new GameObject[3];

	private void Awake()
	{
	}

	private void Start()
	{
	}

	public void InitializeTrafficLight()
	{
		foreach (Transform item in base.transform)
		{
			if (item.name.Equals("Traffic-LightA") || item.name.Equals("Traffic-LightB"))
			{
				trafficlightMaterial = new Material(item.GetComponent<MeshRenderer>().sharedMaterial);
				trafficlightMaterial.SetInt("_RedLight", 0);
				trafficlightMaterial.SetInt("_YellowLight", 0);
				trafficlightMaterial.SetInt("_GreenLight", 0);
				item.GetComponent<MeshRenderer>().sharedMaterial = trafficlightMaterial;
			}
			else if (item.name.Contains("LOD") && (bool)trafficlightMaterial)
			{
				item.GetComponent<MeshRenderer>().sharedMaterial = trafficlightMaterial;
			}
			else if (item.name.Equals("Light-BulbGreen"))
			{
				Lights[0] = item.gameObject;
			}
			else if (item.name.Equals("Light-BulbYellow"))
			{
				Lights[1] = item.gameObject;
			}
			else if (item.name.Equals("Light-BulbRed"))
			{
				Lights[2] = item.gameObject;
			}
		}
	}

	public void SetLightValue(int RedOn, int YellowOn, int GreenOn)
	{
		trafficlightMaterial.SetInt("_RedLight", RedOn);
		trafficlightMaterial.SetInt("_YellowLight", YellowOn);
		trafficlightMaterial.SetInt("_GreenLight", GreenOn);
	}
}
