using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QT_TrafficLightController : MonoBehaviour
{
	public List<QT_TrafficLight> GreenLights;

	public List<QT_TrafficLight> RedLights;

	public List<QT_TrafficLight> BlinkingLights;

	public float StayOnGreenTime = 10f;

	public float StayOnYellowTime = 4f;

	public float RedToGreenTime = 4f;

	public float BlinkTime = 2f;

	public byte nothin;

	public bool ShowLinks;

	public float GlobalTimeOffset;

	public bool UseDynamicLights;

	private List<int> greenindices = new List<int>();

	private List<int> redindices = new List<int>();

	private List<int> yellowindices = new List<int>();

	private bool swapSets;

	private void Start()
	{
		UpdateIndices();
		SetupInitialBulbs();
		if (greenindices.Count > 0 && redindices.Count > 0)
		{
			StartCoroutine(DoLightLoop());
		}
		if (yellowindices.Count > 0)
		{
			StartCoroutine(DoBlinkLoop());
		}
	}

	private IEnumerator DoLightLoop()
	{
		yield return new WaitForSeconds(GlobalTimeOffset);
		while (true)
		{
			yield return new WaitForSeconds(StayOnGreenTime);
			if (!swapSets)
			{
				foreach (int greenindex in greenindices)
				{
					GreenLights[greenindex].SetLightValue(0, 2, 0);
					if (UseDynamicLights)
					{
						GreenLights[greenindex].Lights[0].SetActive(false);
						GreenLights[greenindex].Lights[1].SetActive(true);
					}
				}
				yield return new WaitForSeconds(StayOnYellowTime);
				foreach (int greenindex2 in greenindices)
				{
					GreenLights[greenindex2].SetLightValue(2, 0, 0);
					if (UseDynamicLights)
					{
						GreenLights[greenindex2].Lights[1].SetActive(false);
						GreenLights[greenindex2].Lights[2].SetActive(true);
					}
				}
				yield return new WaitForSeconds(RedToGreenTime);
				foreach (int redindex in redindices)
				{
					RedLights[redindex].SetLightValue(0, 0, 2);
					if (UseDynamicLights)
					{
						RedLights[redindex].Lights[2].SetActive(false);
						RedLights[redindex].Lights[0].SetActive(true);
					}
				}
			}
			else
			{
				foreach (int redindex2 in redindices)
				{
					RedLights[redindex2].SetLightValue(0, 2, 0);
					if (UseDynamicLights)
					{
						RedLights[redindex2].Lights[0].SetActive(false);
						RedLights[redindex2].Lights[1].SetActive(true);
					}
				}
				yield return new WaitForSeconds(StayOnYellowTime);
				foreach (int redindex3 in redindices)
				{
					RedLights[redindex3].SetLightValue(2, 0, 0);
					if (UseDynamicLights)
					{
						RedLights[redindex3].Lights[1].SetActive(false);
						RedLights[redindex3].Lights[2].SetActive(true);
					}
				}
				yield return new WaitForSeconds(RedToGreenTime);
				foreach (int greenindex3 in greenindices)
				{
					GreenLights[greenindex3].SetLightValue(0, 0, 2);
					if (UseDynamicLights)
					{
						GreenLights[greenindex3].Lights[0].SetActive(true);
						GreenLights[greenindex3].Lights[2].SetActive(false);
					}
				}
			}
			swapSets = !swapSets;
		}
	}

	private IEnumerator DoBlinkLoop()
	{
		yield return new WaitForSeconds(GlobalTimeOffset);
		while (true)
		{
			yield return new WaitForSeconds(BlinkTime / 2f);
			foreach (int yellowindex in yellowindices)
			{
				BlinkingLights[yellowindex].SetLightValue(0, 0, 0);
				if (UseDynamicLights)
				{
					BlinkingLights[yellowindex].Lights[1].SetActive(false);
				}
			}
			yield return new WaitForSeconds(BlinkTime / 2f);
			foreach (int yellowindex2 in yellowindices)
			{
				BlinkingLights[yellowindex2].SetLightValue(0, 2, 0);
				if (UseDynamicLights)
				{
					BlinkingLights[yellowindex2].Lights[1].SetActive(true);
				}
			}
		}
	}

	private void UpdateIndices()
	{
		greenindices.Clear();
		redindices.Clear();
		yellowindices.Clear();
		for (int i = 0; i < GreenLights.Count; i++)
		{
			if (GreenLights[i] != null)
			{
				greenindices.Add(i);
			}
		}
		for (int j = 0; j < BlinkingLights.Count; j++)
		{
			if (BlinkingLights[j] != null)
			{
				yellowindices.Add(j);
			}
		}
		for (int k = 0; k < RedLights.Count; k++)
		{
			if (RedLights[k] != null)
			{
				redindices.Add(k);
			}
		}
	}

	private void SetupInitialBulbs()
	{
		if (greenindices.Count > 0)
		{
			foreach (int greenindex in greenindices)
			{
				GreenLights[greenindex].InitializeTrafficLight();
				GreenLights[greenindex].SetLightValue(0, 0, 2);
				GameObject[] lights = GreenLights[greenindex].Lights;
				foreach (GameObject gameObject in lights)
				{
					gameObject.SetActive(false);
				}
				if (UseDynamicLights)
				{
					GreenLights[greenindex].Lights[0].SetActive(true);
				}
			}
		}
		if (redindices.Count > 0)
		{
			foreach (int redindex in redindices)
			{
				RedLights[redindex].InitializeTrafficLight();
				RedLights[redindex].SetLightValue(2, 0, 0);
				GameObject[] lights2 = RedLights[redindex].Lights;
				foreach (GameObject gameObject2 in lights2)
				{
					gameObject2.SetActive(false);
				}
				if (UseDynamicLights)
				{
					RedLights[redindex].Lights[2].SetActive(true);
				}
			}
		}
		if (yellowindices.Count <= 0)
		{
			return;
		}
		foreach (int yellowindex in yellowindices)
		{
			BlinkingLights[yellowindex].InitializeTrafficLight();
			BlinkingLights[yellowindex].SetLightValue(0, 2, 0);
			GameObject[] lights3 = BlinkingLights[yellowindex].Lights;
			foreach (GameObject gameObject3 in lights3)
			{
				gameObject3.SetActive(false);
			}
		}
	}

	private void ResetAll()
	{
		ResetGreen();
		ResetBlinking();
		ResetRed();
	}

	private void ResetGreen()
	{
		for (int i = 0; i < GreenLights.Count; i++)
		{
			GreenLights[i] = null;
		}
	}

	private void ResetRed()
	{
		for (int i = 0; i < RedLights.Count; i++)
		{
			RedLights[i] = null;
		}
	}

	private void ResetBlinking()
	{
		for (int i = 0; i < BlinkingLights.Count; i++)
		{
			BlinkingLights[i] = null;
		}
	}

	private void DisplayLinks()
	{
		ShowLinks = !ShowLinks;
		UpdateIndices();
		foreach (int greenindex in greenindices)
		{
			GreenLights[greenindex].showLinks = ShowLinks;
			GreenLights[greenindex].linkColor = Color.green;
			GreenLights[greenindex].controllerPosition = base.transform.position;
		}
		foreach (int yellowindex in yellowindices)
		{
			BlinkingLights[yellowindex].showLinks = ShowLinks;
			BlinkingLights[yellowindex].linkColor = Color.yellow;
			BlinkingLights[yellowindex].controllerPosition = base.transform.position;
		}
		foreach (int redindex in redindices)
		{
			RedLights[redindex].showLinks = ShowLinks;
			RedLights[redindex].linkColor = Color.red;
			RedLights[redindex].controllerPosition = base.transform.position;
		}
		base.transform.position = base.transform.position;
	}

	private void HelpClick()
	{
		Application.OpenURL("http://quantumtheoryentertainment.com/UCP/Documentation/traffic-light-system/");
	}
}
