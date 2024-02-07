using UnityEngine;

public class MainMenu : MonoBehaviour
{
	private FPSPlayer FPSPlayerComponent;

	[Tooltip("True if the menu should be activated by pressing the main menu button (default esc key).")]
	public bool useMainMenu = true;

	[Tooltip("True if the menu size should scale with screen size.")]
	public bool scaleWithScreenSize;

	[Tooltip("The NPC parent objects to enable/disable for game modes in the order of: single player, faction war, wave survival.")]
	public GameObject[] NpcGroups;

	public Font font;

	public Color GuiTint;

	[Tooltip("Color of normal button text.")]
	public Color btnTextColor;

	[Tooltip("Color of text for buttons with active states.")]
	public Color btnActiveTextColor;

	private bool initSyles;

	public float fontSizeScaled = 0.025f;

	public float titleFontSizeScaled;

	public int fontSize = 14;

	public int titleFontSize = 14;

	public float menuPosV = 0.3f;

	public float menuPosH = 0.5f;

	public float buttonHeight;

	public float buttonWidth;

	public float buttonHeightScaled = 0.075f;

	public float buttonWidthScaled = 0.31f;

	private float buttonHeightAmt;

	private float buttonWidthAmt;

	public float buttonSpacing = 10f;

	private float buttonV;

	private float buttonH;

	private bool menuDisplayed;

	private bool resumePress;

	private bool dzState;

	private bool dzButtonState;

	private bool invulButtonState;

	private bool giveAllButtonState;

	private AudioSource aSource;

	public AudioClip buttonClickFx;

	public float buttonFxVol = 1f;

	public AudioClip invulClickFx;

	public float invulFxVol = 1f;

	public AudioClip giveAllClickFx;

	public float giveAllFxVol = 1f;

	public AudioClip beepClickFx;

	public float beepFxVol = 1f;

	private GUIStyleState buttonActive;

	private GUIStyleState buttonInActive;

	private GUIStyle toggleButtonStyle1;

	private GUIStyle toggleButtonStyle2;

	private GUIStyle toggleButtonStyle3;

	private GUIStyle mainButtonSyle;

	private GUIStyle mapButtonStyle;

	private bool npcGoupsNull;

	private GUIStyle titleStyle;

	private bool hungerThirstActive;

	private void Start()
	{
		FPSPlayerComponent = Camera.main.GetComponent<CameraControl>().playerObj.GetComponent<FPSPlayer>();
		aSource = base.gameObject.AddComponent<AudioSource>();
		aSource.spatialBlend = 0f;
		aSource.volume = 1f;
		aSource.playOnAwake = false;
		hungerThirstActive = FPSPlayerComponent.usePlayerThirst;
		if (NpcGroups.Length > 0 && NpcGroups[0] != null && NpcGroups[1] != null && NpcGroups[2] != null)
		{
			GameObject[] npcGroups = NpcGroups;
			foreach (GameObject gameObject in npcGroups)
			{
				gameObject.SetActive(false);
			}
			NpcGroups[PlayerPrefs.GetInt("Game Type")].SetActive(true);
		}
		else
		{
			npcGoupsNull = true;
		}
		base.enabled = false;
	}

	private void Update()
	{
		if (resumePress)
		{
			Time.timeScale = FPSPlayerComponent.menuTime;
			resumePress = false;
			FPSPlayerComponent.paused = false;
			FPSPlayerComponent.menuDisplayed = false;
			base.enabled = false;
		}
	}

	private void OnGUI()
	{
		GUI.color = GuiTint;
		if (!initSyles)
		{
			toggleButtonStyle1 = new GUIStyle(GUI.skin.button);
			toggleButtonStyle2 = new GUIStyle(GUI.skin.button);
			toggleButtonStyle3 = new GUIStyle(GUI.skin.button);
			titleStyle = new GUIStyle(GUI.skin.box);
			mainButtonSyle = new GUIStyle(GUI.skin.button);
			mapButtonStyle = new GUIStyle(GUI.skin.button);
			if (!npcGoupsNull)
			{
				mapButtonStyle.normal.textColor = btnTextColor;
				mapButtonStyle.active.textColor = btnTextColor;
				mapButtonStyle.hover.textColor = Color.white;
			}
			else
			{
				mapButtonStyle.normal.textColor = Color.gray;
				mapButtonStyle.active.textColor = Color.gray;
				mapButtonStyle.hover.textColor = Color.gray;
			}
			GUI.skin.font = font;
			if (FPSPlayerComponent.invulnerable)
			{
				toggleButtonStyle1.normal.textColor = btnActiveTextColor;
				toggleButtonStyle1.active.textColor = btnActiveTextColor;
				toggleButtonStyle1.hover.textColor = btnActiveTextColor;
			}
			else
			{
				toggleButtonStyle1.normal.textColor = Color.gray;
				toggleButtonStyle1.active.textColor = Color.gray;
				toggleButtonStyle1.hover.textColor = Color.gray;
			}
			if (FPSPlayerComponent.WeaponPivotComponent.deadzoneZooming)
			{
				toggleButtonStyle3.normal.textColor = btnActiveTextColor;
				toggleButtonStyle3.active.textColor = btnActiveTextColor;
				toggleButtonStyle3.hover.textColor = btnActiveTextColor;
			}
			else
			{
				toggleButtonStyle3.normal.textColor = Color.gray;
				toggleButtonStyle3.active.textColor = Color.gray;
				toggleButtonStyle3.hover.textColor = Color.gray;
			}
			if (hungerThirstActive)
			{
				toggleButtonStyle2.normal.textColor = btnActiveTextColor;
				toggleButtonStyle2.active.textColor = btnActiveTextColor;
				toggleButtonStyle2.hover.textColor = btnActiveTextColor;
			}
			else
			{
				toggleButtonStyle2.normal.textColor = Color.gray;
				toggleButtonStyle2.active.textColor = Color.gray;
				toggleButtonStyle2.hover.textColor = Color.gray;
			}
			initSyles = true;
		}
		mainButtonSyle.normal.textColor = btnTextColor;
		mainButtonSyle.active.textColor = btnTextColor;
		mainButtonSyle.hover.textColor = Color.white;
		if (scaleWithScreenSize)
		{
			buttonHeightAmt = (float)Screen.height * buttonHeightScaled;
			buttonWidthAmt = (float)Screen.width * buttonWidthScaled;
			int num = Mathf.RoundToInt((float)Screen.width * fontSizeScaled);
			mainButtonSyle.fontSize = num;
			mapButtonStyle.fontSize = num;
			toggleButtonStyle1.fontSize = num;
			toggleButtonStyle2.fontSize = num;
			toggleButtonStyle3.fontSize = num;
			titleStyle.fontSize = Mathf.RoundToInt((float)Screen.width * titleFontSizeScaled);
			titleStyle.normal.textColor = Color.white;
		}
		else
		{
			buttonHeightAmt = buttonHeight;
			buttonWidthAmt = buttonWidth;
			mainButtonSyle.fontSize = fontSize;
			mapButtonStyle.fontSize = fontSize;
			toggleButtonStyle1.fontSize = fontSize;
			toggleButtonStyle2.fontSize = fontSize;
			toggleButtonStyle3.fontSize = fontSize;
			titleStyle.fontSize = titleFontSize;
			titleStyle.normal.textColor = Color.white;
		}
		buttonH = (float)Screen.width * menuPosH - buttonWidthAmt - buttonSpacing * 0.5f;
		buttonV = (float)Screen.height * menuPosV;
		GUI.Box(new Rect(buttonH - buttonSpacing, buttonV - buttonHeightAmt - buttonSpacing, buttonWidthAmt * 2f + buttonSpacing * 3f, (buttonHeightAmt + buttonSpacing) * 6f), string.Empty);
		GUI.Box(new Rect(buttonH - buttonSpacing, buttonV - buttonHeightAmt - buttonSpacing, buttonWidthAmt * 2f + buttonSpacing * 3f, buttonHeightAmt), "Realistic FPS Prefab - Main Menu", titleStyle);
		if (GUI.Button(new Rect(buttonH, buttonV, buttonWidthAmt, buttonHeightAmt), "Resume Game", mainButtonSyle))
		{
			resumePress = true;
			PlayButtonFx(buttonClickFx, buttonFxVol);
		}
		if (GUI.Button(new Rect(buttonH, buttonV + buttonHeightAmt + buttonSpacing, buttonWidthAmt, buttonHeightAmt), "Restart Map", mainButtonSyle))
		{
			FPSPlayerComponent.RestartMap();
			PlayButtonFx(buttonClickFx, buttonFxVol);
			base.enabled = false;
		}
		if (GUI.Button(new Rect(buttonH, buttonV + (buttonHeightAmt + buttonSpacing) * 2f, buttonWidthAmt, buttonHeightAmt), "Story Mode", mapButtonStyle))
		{
			if (!npcGoupsNull)
			{
				PlayerPrefs.SetInt("Game Type", 0);
				FPSPlayerComponent.RestartMap();
				PlayButtonFx(buttonClickFx, buttonFxVol);
				base.enabled = false;
			}
			else
			{
				PlayButtonFx(beepClickFx, beepFxVol);
			}
		}
		if (GUI.Button(new Rect(buttonH, buttonV + (buttonHeightAmt + buttonSpacing) * 3f, buttonWidthAmt, buttonHeightAmt), "Faction War", mapButtonStyle))
		{
			if (!npcGoupsNull)
			{
				PlayerPrefs.SetInt("Game Type", 1);
				FPSPlayerComponent.RestartMap();
				PlayButtonFx(buttonClickFx, buttonFxVol);
				base.enabled = false;
			}
			else
			{
				PlayButtonFx(beepClickFx, beepFxVol);
			}
		}
		if (GUI.Button(new Rect(buttonH, buttonV + (buttonHeightAmt + buttonSpacing) * 4f, buttonWidthAmt, buttonHeightAmt), "Wave Survival", mapButtonStyle))
		{
			if (!npcGoupsNull)
			{
				PlayerPrefs.SetInt("Game Type", 2);
				FPSPlayerComponent.RestartMap();
				PlayButtonFx(buttonClickFx, buttonFxVol);
				base.enabled = false;
			}
			else
			{
				PlayButtonFx(beepClickFx, beepFxVol);
			}
		}
		if (GUI.Button(new Rect(buttonH + buttonWidthAmt + buttonSpacing, buttonV, buttonWidthAmt, buttonHeightAmt), "Give All Weapons", mainButtonSyle))
		{
			FPSPlayerComponent.PlayerWeaponsComponent.GiveAllWeaponsAndAmmo();
			PlayButtonFx(giveAllClickFx, giveAllFxVol);
		}
		if (GUI.Button(new Rect(buttonH + buttonWidthAmt + buttonSpacing, buttonV + buttonHeightAmt + buttonSpacing, buttonWidthAmt, buttonHeightAmt), "Invulnerable", toggleButtonStyle1))
		{
			if (!FPSPlayerComponent.invulnerable)
			{
				FPSPlayerComponent.invulnerable = true;
				PlayButtonFx(invulClickFx, invulFxVol);
				toggleButtonStyle1.normal.textColor = btnActiveTextColor;
				toggleButtonStyle1.active.textColor = btnActiveTextColor;
				toggleButtonStyle1.hover.textColor = btnActiveTextColor;
			}
			else
			{
				PlayButtonFx(beepClickFx, beepFxVol);
				FPSPlayerComponent.invulnerable = false;
				toggleButtonStyle1.normal.textColor = Color.gray;
				toggleButtonStyle1.active.textColor = Color.gray;
				toggleButtonStyle1.hover.textColor = Color.gray;
			}
		}
		if (GUI.Button(new Rect(buttonH + buttonWidthAmt + buttonSpacing, buttonV + (buttonHeightAmt + buttonSpacing) * 2f, buttonWidthAmt, buttonHeightAmt), "Hunger and Thirst", toggleButtonStyle2))
		{
			if (hungerThirstActive)
			{
				hungerThirstActive = false;
				FPSPlayerComponent.usePlayerHunger = false;
				FPSPlayerComponent.usePlayerThirst = false;
				PlayButtonFx(beepClickFx, beepFxVol);
				toggleButtonStyle2.normal.textColor = Color.gray;
				toggleButtonStyle2.active.textColor = Color.gray;
				toggleButtonStyle2.hover.textColor = Color.gray;
			}
			else
			{
				PlayButtonFx(buttonClickFx, buttonFxVol);
				FPSPlayerComponent.usePlayerHunger = true;
				FPSPlayerComponent.usePlayerThirst = true;
				FPSPlayerComponent.UpdateHunger(0f - FPSPlayerComponent.maxHungerPoints);
				FPSPlayerComponent.UpdateThirst(0f - FPSPlayerComponent.maxThirstPoints);
				hungerThirstActive = true;
				toggleButtonStyle2.normal.textColor = btnActiveTextColor;
				toggleButtonStyle2.active.textColor = btnActiveTextColor;
				toggleButtonStyle2.hover.textColor = btnActiveTextColor;
			}
		}
		if (GUI.Button(new Rect(buttonH + buttonWidthAmt + buttonSpacing, buttonV + (buttonHeightAmt + buttonSpacing) * 3f, buttonWidthAmt, buttonHeightAmt), "Free Aim Zooming", toggleButtonStyle3))
		{
			FPSPlayerComponent.WeaponPivotComponent.ToggleDeadzoneZooming();
			if (FPSPlayerComponent.WeaponPivotComponent.deadzoneZooming)
			{
				PlayButtonFx(buttonClickFx, buttonFxVol);
				toggleButtonStyle3.normal.textColor = btnActiveTextColor;
				toggleButtonStyle3.active.textColor = btnActiveTextColor;
				toggleButtonStyle3.hover.textColor = btnActiveTextColor;
			}
			else
			{
				PlayButtonFx(beepClickFx, beepFxVol);
				toggleButtonStyle3.normal.textColor = Color.gray;
				toggleButtonStyle3.active.textColor = Color.gray;
				toggleButtonStyle3.hover.textColor = Color.gray;
			}
			aSource.Play();
		}
		if (GUI.Button(new Rect(buttonH + buttonWidthAmt + buttonSpacing, buttonV + (buttonHeightAmt + buttonSpacing) * 4f, buttonWidthAmt, buttonHeightAmt), "Exit Game", mainButtonSyle))
		{
			PlayerPrefs.SetInt("Game Type", 0);
			PlayButtonFx(buttonClickFx, buttonFxVol);
			Application.Quit();
		}
	}

	private void PlayButtonFx(AudioClip clip, float vol)
	{
		aSource.volume = vol;
		aSource.clip = clip;
		aSource.Play();
	}
}
