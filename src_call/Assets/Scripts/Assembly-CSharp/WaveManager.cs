using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
	private FPSPlayer FPSPlayerComponent;

	[Tooltip("The NPC Spawner objects that the Wave Manager will spawn NPC's from. The Waves list parameters correspond the order of these spawners from top to bottom.")]
	public List<NPCSpawner> NpcSpawners = new List<NPCSpawner>();

	[Tooltip("This list contains information for NPC wave spawning. The array sizes and order correspond with the Npc Spawners list. The Waves list can be expanded to add new waves of varying combinations of NPCs and parameters.")]
	public MultiDimensionalInt[] waves;

	[Tooltip("Time before wave begins.")]
	public float warmupTime = 30f;

	private float startTime = -512f;

	private float countDown;

	[HideInInspector]
	public int NpcsToSpawn;

	[HideInInspector]
	public int killedNpcs;

	[HideInInspector]
	public int waveNumber;

	[Tooltip("Sound FX played when wave starts.")]
	public AudioClip waveStartFx;

	[Tooltip("Sound FX played when wave ends.")]
	public AudioClip waveEndFx;

	private AudioSource asource;

	private bool fxPlayed;

	private bool fxPlayed2;

	private bool lastWave;

	[HideInInspector]
	public WaveText WaveText;

	[HideInInspector]
	public WaveText WaveTextShadow;

	[HideInInspector]
	public WarmupText WarmupText;

	[HideInInspector]
	public WarmupText WarmupTextShadow;

	private Color tempColor;

	private Color tempColor2;

	private Text WarmupUIText1;

	private Text WarmupUIText2;

	private Vector2 warmupTextPos1Orig;

	private Vector2 warmupTextPos2Orig;

	private Vector2 warmupTextPos1;

	private Vector2 warmupTextPos2;

	private void Start()
	{
		FPSPlayerComponent = Camera.main.GetComponent<CameraControl>().playerObj.GetComponent<FPSPlayer>();
		asource = base.gameObject.AddComponent<AudioSource>();
		asource.spatialBlend = 0f;
		WaveText = FPSPlayerComponent.waveUiObj.GetComponent<WaveText>();
		WaveTextShadow = FPSPlayerComponent.waveUiObjShadow.GetComponent<WaveText>();
		WaveText.waveGui = waveNumber;
		WaveTextShadow.waveGui = waveNumber;
		WaveText.waveGui2 = NpcsToSpawn - killedNpcs;
		WaveTextShadow.waveGui2 = NpcsToSpawn - killedNpcs;
		WarmupText = FPSPlayerComponent.warmupUiObj.GetComponent<WarmupText>();
		WarmupTextShadow = FPSPlayerComponent.warmupUiObjShadow.GetComponent<WarmupText>();
		tempColor = WarmupText.textColor;
		tempColor2 = WarmupTextShadow.textColor;
		WarmupText.warmupGui = countDown;
		WarmupTextShadow.warmupGui = countDown;
		WarmupUIText1 = WarmupText.GetComponent<Text>();
		WarmupUIText2 = WarmupTextShadow.GetComponent<Text>();
		warmupTextPos1Orig = WarmupUIText1.rectTransform.anchoredPosition;
		warmupTextPos2Orig = WarmupUIText2.rectTransform.anchoredPosition;
		StartCoroutine(StartWave());
	}

	private void FixedUpdate()
	{
		if (WaveText.waveGui2 != NpcsToSpawn - killedNpcs)
		{
			WaveText.waveGui2 = NpcsToSpawn - killedNpcs;
			WaveTextShadow.waveGui2 = NpcsToSpawn - killedNpcs;
		}
	}

	public IEnumerator StartWave()
	{
		countDown = warmupTime;
		WarmupText.warmupGui = countDown;
		WarmupTextShadow.warmupGui = countDown;
		killedNpcs = 0;
		NpcsToSpawn = 0;
		if (waveNumber <= waves.Length)
		{
			if (waveNumber < waves.Length)
			{
				waveNumber++;
			}
			else
			{
				lastWave = true;
				waveNumber = 1;
			}
		}
		else
		{
			waveNumber = 1;
		}
		WaveText.waveGui = waveNumber;
		WaveTextShadow.waveGui = waveNumber;
		tempColor.a = 1f;
		tempColor2.a = 1f;
		WarmupText.waveBegins = false;
		WarmupTextShadow.waveBegins = false;
		if (waveNumber > 1 || lastWave)
		{
			startTime = Time.time;
			WarmupText.waveComplete = true;
			WarmupTextShadow.waveComplete = true;
			if ((bool)waveEndFx && !fxPlayed2)
			{
				asource.PlayOneShot(waveEndFx, 1f);
				FPSPlayerComponent.StartCoroutine(FPSPlayerComponent.ActivateBulletTime(1f));
				fxPlayed2 = true;
			}
			if (lastWave)
			{
				lastWave = false;
			}
		}
		for (int i = 0; i < NpcSpawners.Count; i++)
		{
			NpcSpawners[i].NPCPrefab = waves[waveNumber - 1].NpcTypes[i];
			NpcSpawners[i].NpcsToSpawn = waves[waveNumber - 1].NpcCounts[i];
			NpcSpawners[i].maxActiveNpcs = waves[waveNumber - 1].NpcLoads[i];
			NpcSpawners[i].spawnDelay = waves[waveNumber - 1].NpcDelay[i];
			NpcsToSpawn += NpcSpawners[i].NpcsToSpawn;
			NpcSpawners[i].pauseSpawning = true;
			NpcSpawners[i].spawnedNpcAmt = 0;
			NpcSpawners[i].huntPlayer = true;
			NpcSpawners[i].unlimitedSpawning = false;
		}
		while (true)
		{
			WarmupUIText1.rectTransform.anchoredPosition = warmupTextPos1Orig;
			WarmupUIText2.rectTransform.anchoredPosition = warmupTextPos2Orig;
			warmupTextPos1 = warmupTextPos1Orig;
			warmupTextPos2 = warmupTextPos2Orig;
			if ((double)startTime + 3.0 < (double)Time.time)
			{
				WarmupText.waveComplete = false;
				WarmupTextShadow.waveComplete = false;
				countDown -= Time.deltaTime;
				WarmupText.warmupGui = countDown;
				WarmupTextShadow.warmupGui = countDown;
			}
			WarmupUIText1.enabled = true;
			WarmupUIText2.enabled = true;
			WarmupUIText1.color = tempColor;
			WarmupUIText2.color = tempColor2;
			if (countDown <= 0f && (bool)waveStartFx && !fxPlayed)
			{
				for (int j = 0; j < NpcSpawners.Count; j++)
				{
					NpcSpawners[j].pauseSpawning = false;
				}
				WarmupText.waveBegins = true;
				WarmupTextShadow.waveBegins = true;
				fxPlayed = true;
				fxPlayed2 = false;
				asource.PlayOneShot(waveStartFx, 1f);
			}
			if (countDown <= -2.75f)
			{
				break;
			}
			yield return null;
		}
		StartCoroutine(FadeWarmupText());
		fxPlayed = false;
	}

	private IEnumerator FadeWarmupText()
	{
		while (true)
		{
			tempColor.a -= Time.deltaTime;
			tempColor2.a -= Time.deltaTime;
			WarmupUIText1.color = tempColor;
			WarmupUIText2.color = tempColor2;
			warmupTextPos1.y -= Time.deltaTime * 9f;
			warmupTextPos2.y -= Time.deltaTime * 9f;
			WarmupUIText1.rectTransform.anchoredPosition = warmupTextPos1;
			WarmupUIText2.rectTransform.anchoredPosition = warmupTextPos2;
			if (tempColor.a <= 0f && tempColor2.a <= 0f)
			{
				break;
			}
			yield return null;
		}
		WarmupUIText1.enabled = false;
		WarmupUIText2.enabled = false;
	}
}
