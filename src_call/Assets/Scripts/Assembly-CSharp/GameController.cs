using System.Collections;
using _0_WebPort;
using _00_YaTutor;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
	public WeaponBehavior[] weapons;

	public PlayerWeapons playerWeapons;

	private int currentWeaponIndex;

	public MobleInputController input;

	public LevelController level;

	public Transform mainPlayer;

	public Transform fpsPlayer;

	public UIController ui;

	public GameObject[] levels;

	private int playerHP = 100;

	private int headShots;

	private bool isGrenadeSelected;

	private int mainGunIndex;

	public GameObject falseEffectObj;

	private int grenadeCount;

	private int flashCount;

	public GameObject[] segments;

	public GameObject[] subsegments;

	public AudioSource music;

	public AudioClip[] deathClips;

	public static GameController instance;

	public GameObject noMap;

	public GameObject mapBtn;

	public GameObject noMapBtn;

	public GameObject shootBtn;

	private int checkCounter;

	public AudioClip[] flashSound;

	public FPSPlayer _fpsPlayer;

	[HideInInspector]
	public bool isIconOnMapCreated;

	private void Awake()
	{
		if (PlayerPrefs.GetString("isManualShoot", "YES") == "YES")
		{
			shootBtn.SetActive(true);
		}
		else
		{
			shootBtn.SetActive(false);
		}
		for (int i = 0; i < levels.Length; i++)
		{
			levels[i].SetActive(false);
		}
		mainGunIndex = PlayerPrefs.GetInt("SelectedGunIndex", 0);
		
		playerWeapons.firstWeapon = mainGunIndex + 1;
		Time.timeScale = 1f;
		levels[Globals.currentLevelNumber - 1].SetActive(true);
		GameObject gameObject = levels[Globals.currentLevelNumber - 1];
		level = gameObject.GetComponent<LevelController>();
		level.gc = this;
		mainPlayer.position = level.playerStartingPosition.transform.position;
		Quaternion identity = Quaternion.identity;
		identity.eulerAngles = level.startRotationAngles;
		mainPlayer.rotation = identity;
		StartCoroutine(ui.showLevelStartDetails(level.missionDetails));
		ui.updateAmmoInfo(weapons[mainGunIndex].bulletsLeft, weapons[mainGunIndex].bulletsToReload);
		if (PlayerPrefs.GetInt("Sound", 1) == 1)
		{
			AudioListener.volume = 1f;
		}
		else
		{
			AudioListener.volume = 0f;
		}
		StartCoroutine(fadeInMusic());

		
		
		if (instance == null)
		{
			instance = this;
		}
		if (level.NoMap)
		{
			mapBtn.SetActive(false);
			noMapBtn.SetActive(true);
		}
		else
		{
			mapBtn.SetActive(true);
			noMapBtn.SetActive(false);
		}
	}

	private void checkForBanner()
	{
	}

	private IEnumerator fadeInMusic()
	{
		music.volume = 0f;
		music.Play();
		while (music.volume != 0.4f)
		{
			music.volume = Mathf.MoveTowards(music.volume, 0.4f, 0.5f * Time.deltaTime);
			yield return null;
		}
	}

	private IEnumerator fadeOutMusic()
	{
		while (music.volume > 0f)
		{
			music.volume = Mathf.MoveTowards(music.volume, 0f, 0.5f * Time.deltaTime);
			yield return null;
		}
	}

	private void setUpEnvoirment()
	{
		if (Globals.currentLevelNumber <= 5)
		{
			for (int i = 1; i < segments.Length; i++)
			{
				segments[i].SetActive(false);
			}
			if (Globals.currentLevelNumber == 1)
			{
				subsegments[0].SetActive(false);
			}
			else if (Globals.currentLevelNumber > 3)
			{
				subsegments[1].SetActive(false);
			}
		}
		else if (Globals.currentLevelNumber == 6)
		{
			segments[2].SetActive(false);
			segments[3].SetActive(false);
		}
		else if (Globals.currentLevelNumber < 8)
		{
			segments[0].SetActive(false);
			segments[2].SetActive(false);
			segments[3].SetActive(false);
		}
		else if (Globals.currentLevelNumber == 8)
		{
			segments[0].SetActive(false);
			segments[3].SetActive(false);
		}
		else if (Globals.currentLevelNumber == 9)
		{
			segments[0].SetActive(false);
			segments[3].SetActive(false);
		}
		else if (Globals.currentLevelNumber == 10)
		{
			segments[0].SetActive(false);
			segments[1].SetActive(false);
			segments[3].SetActive(false);
		}
		else if (Globals.currentLevelNumber > 10 && Globals.currentLevelNumber < 17)
		{
			segments[0].SetActive(false);
			segments[1].SetActive(false);
		}
		else if (Globals.currentLevelNumber >= 17 && Globals.currentLevelNumber <= 19)
		{
			segments[3].SetActive(false);
			segments[0].SetActive(false);
		}
		else if (Globals.currentLevelNumber == 20)
		{
			segments[2].SetActive(false);
			segments[3].SetActive(false);
		}
	}

	public void startMissionOnClick()
	{
		ui.btnClick.Play();
		if ((bool)level)
		{
			level.ChangePlayerPosition();
		}
		StartCoroutine(ui.showHud());
	}

	private void Update()
	{
		updateAmmoInfo();
		ui.updateGrenadeCounter(PlayerPrefs.GetInt("Grenades"));
		UIController.instanse.enemyCount.text = (level.totalEnemiesCount - level.enemiesKilled).ToString();
		playerHP = (int)_fpsPlayer.hitPoints;
		ui.updateHealth(playerHP);

		if (CtrlYa.Instance && CtrlYa.Instance.GetDevice() == CtrlYa.YaDevice.PC)
		{
			if (Time.timeScale < 1F)
			{
				//Debug.Log("Ignoring input when game Time.timeScale < 1F");
				return;
			}
			UpdatePCInputs();
		}
	}

	private void UpdatePCInputs()
	{
		// JUMP
		if (Input.GetKeyDown(KeyCode.Space))
		{
			this.Jump(true);
		}
		if (Input.GetKeyUp(KeyCode.Space))
		{
			this.Jump(false);
		}
		// SIT
		if (Input.GetKeyDown(KeyCode.C))
		{
			this.Crouch(true);
		}
		if (Input.GetKeyUp(KeyCode.C))
		{
			this.Crouch(false);
		}
		// FIRE
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			this.OnFireBtnDown();
		}
		if (Input.GetKeyUp(KeyCode.Mouse0))
		{
			this.OnFireBtnUp();
		}
		// ZOOM
		if (Input.GetKeyDown(KeyCode.Mouse1))
		{
			this.Zooming();
		}
		// GRENADE
		if (Input.GetKeyDown(KeyCode.G))
		{
			this.throughGrenade(true);
		}
		if (Input.GetKeyUp(KeyCode.G))
		{
			this.throughGrenade(false);
		}
		// RELOAD
		if (Input.GetKeyUp(KeyCode.R))
		{
			this.reloadWeapon();
		}
		// PAUSE
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			this.pauseGameOnClick();
		}
	}


	private EventSystem _curEventSys;


	private bool IsHaveUiWhenFire()
	{
		if (CtrlYa.Instance)
		{
			if (CtrlYa.Instance.GetDevice() == CtrlYa.YaDevice.Mobile) return false;
		}
		if (_curEventSys == null)
		{
			_curEventSys = GameObject.FindObjectOfType<EventSystem>();
		}
		if (_curEventSys == null)
		{
			Debug.LogError("IsHaveUiWhenFire : cant fond event sys.");
			return false;
		}
		return _curEventSys.IsPointerOverGameObject();
	}
	
	
	public void OnFireBtnDown()
	{
		Debug.Log("onFireBtnDown");
		if (IsHaveUiWhenFire())
		{
			Debug.Log("IsHaveUiWhenFire IGNORE = " + IsHaveUiWhenFire().ToString());
			return;
		}
		
		if (!isGrenadeSelected)
		{
			input.input.fireHold = true;
			updateAmmoInfo();
			return;
		}
		if (Globals.isFlashSelected)
		{
			flashCount--;
			PlayerPrefs.SetInt("Flashbang", flashCount);
			ui.updateFlashCounter(flashCount);
			Invoke("CustomFlashBang", 2f);
		}
		isGrenadeSelected = false;
		input.deselectGrenade();
		StartCoroutine(playerWeapons.SelectWeapon(mainGunIndex));
		if (Globals.currentLevelNumber > 5)
		{
			ui.showGrenadeBtns();
		}
	}

	public void CustomFlashBang()
	{
		showFlashEffect();
		music.PlayOneShot(flashSound[Random.Range(0, flashSound.Length)]);
	}

	public void OnFireBtnUp()
	{
		Debug.Log("OnFireBtnUp");
		input.input.fireHold = false;
	}

	public void selectMainGun()
	{
		input.deselectGrenade();
		StartCoroutine(playerWeapons.SelectWeapon(mainGunIndex + 1));
	}

	public void throughGrenade(bool status)
	{
		Debug.Log("throughGrenade : status = " + status);
		if (PlayerPrefs.GetInt("Grenades") > 0)
		{
			input.input.grenadeHold = status;
			if (!status)
			{
				PlayerPrefs.SetInt("Grenades", PlayerPrefs.GetInt("Grenades") - 1);
			}
		}
	}

	public void selectBomb(int index)
	{
		if ((index != 2 || flashCount != 0) && (index != 1 || grenadeCount != 0))
		{
			isGrenadeSelected = true;
			if (index == 2)
			{
				Globals.isFlashSelected = true;
			}
			else
			{
				Globals.isFlashSelected = false;
			}
			playerWeapons.currentGrenade = index - 1;
			playerWeapons.GrenadeWeaponBehaviorComponent = playerWeapons.grenadeOrder[index - 1].GetComponent<WeaponBehavior>();
			playerWeapons.grenadeWeapon = playerWeapons.GrenadeWeaponBehaviorComponent.weaponNumber;
			input.selectGrenade();
		}
	}

	public void hitEnemy(int damage, GameObject enemyObj, Vector3 direction)
	{
		if (!level.allEnemiesActive && !level.isSaperatelyTriggered)
		{
			level.playerEnterInRange(fpsPlayer.gameObject);
		}
		if (enemyObj.tag == "EnemyHead")
		{
			ui.headShotEffect.SetActive(true);
			headShots++;
		}
		EnemyBase componentInParent = enemyObj.GetComponentInParent<EnemyBase>();
		if (componentInParent != null)
		{
			componentInParent.takeHit(damage, enemyObj, direction);
		}
	}

	public void AddHeadShot()
	{
		ui.headShotEffect.SetActive(true);
		headShots++;
	}

	public void SetMissionCompleted()
	{
		input.input.fireHold = false;
		if(music) music.volume = 0;
		else Debug.LogError("setMissionCompleted : music == NULL");
		Debug.Log("Sergei disabled coroutine");
		//StartCoroutine(fadeOutMusic());
		if (ui)
		{
			ui.showMissionCompleteUI(level.enemiesKilled, level.enemiesKilled * 100, headShots, headShots * 50, playerHP, (int)((float)playerHP * 0.5f));
		}
		else
		{
			Debug.LogError("UI == NULL");
		}
	}

	public void takePlayerDamage(int damageVal)
	{
		playerHP -= damageVal;
		playerHP = Mathf.Clamp(playerHP, 0, 100);
		ui.updateHealth(playerHP);
		if (playerHP == 0)
		{
			setGameOver();
		}
	}

	public void setGameOver()
	{
		music.volume = 0f;
		Invoke("freezeGame", 1.5f);
		ui.showGameOver();
	}

	private void freezeGame()
	{
		Time.timeScale = 0f;
	}

	public void showHintMenu()
	{
		ui.showHintAndTimer();
	}

	public void takePlayerInput()
	{
		input.walkBtnUnPressed();
		input.runBtnUnPressed();
		ui.hudParent.SetActive(false);
	}

	public void showHintOnClick()
	{
		ui.btnClick.Play();
		Time.timeScale = 0f;
		ui.showHintDetails(level.hintText);
	}

	public void hintGotItOnClick()
	{
		Time.timeScale = 1f;
		ui.hintMenuParent.SetActive(false);
		ui.hudParent.SetActive(true);
	}

	public void hitPipe()
	{
		if (level.GetComponent<HostageLevelController>().molotovFired)
		{
			ui.timerObj.SetActive(false);
			StartCoroutine(level.GetComponent<HostageLevelController>().breakThePipe());
		}
	}

	public void setHostageLevelCompleted()
	{
		Time.timeScale = 0f;
		ui.showHostageLevelComplete();
	}

	public void hostagesBurnt()
	{
		setGameOver();
	}

	public void showFlashEffect()
	{
		falseEffectObj.SetActive(true);
	}

	public void updateAmmoInfo()
	{
		ui.updateAmmoInfo(weapons[mainGunIndex].bulletsLeft, weapons[mainGunIndex].bulletsToReload);
	}

	public void reloadWeapon()
	{
		Debug.Log("reloadWeapon");
		ui.btnClick.Play();
		input.input.reloadPress = true;
		Invoke("stopReload", 0.2f);
	}

	private void stopReload()
	{
		input.input.reloadPress = false;
	}

	public void enablePlantBomb()
	{
		ui.enableBombBtn();
	}

	public void showBombBtn()
	{
		ui.showBombBtn();
	}

	public void plantBombOnClick()
	{
		ui.btnClick.Play();
		ui.hpParent.SetActive(false);
		ui.hideBombBtn();
		level.plantedBomb.SetActive(true);
		ui.showBombTimer();
		Invoke("ChangePlayerPosition", 2f);
	}

	private void ChangePlayerPosition()
	{
		level.ChangePlayerPosition();
	}

	public void detotanteBomb()
	{
		ui.timerObj.SetActive(false);
		float num = Vector3.Distance(fpsPlayer.position, level.plantedBomb.transform.position);
		Debug.Log(num);
		if (num < 7f)
		{
			level.bombParticles.SetActive(true);
			level.GetComponent<AudioSource>().Play();
			fpsPlayer.GetComponent<FPSPlayer>().ApplyDamage(200f);
			return;
		}
		level.bombParticles.SetActive(true);
		level.GetComponent<AudioSource>().Play();
		AI[] array = Object.FindObjectsOfType<AI>();
		if (array.Length > 0)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.GetComponent<CharacterDamage>().ApplyDamage(100f, fpsPlayer.position, fpsPlayer.position, fpsPlayer.transform, true, true);
			}
		}
		Invoke("clearBombLevel", 2f);
	}

	private void clearBombLevel()
	{
		Time.timeScale = 0f;

		int levelsCleared = 0;
		if (CtrlYa.Instance)
		{
			levelsCleared = CtrlYa.Instance.GetFinishedLevel();
		}
		else
		{
			Debug.LogError("clearBombLevel : CtrlYa.Instance == NULL");
		}
		
		
		if (levelsCleared < Globals.currentLevelNumber)
		{
			CtrlYa.Instance.SaveFinishedLevel(Globals.currentLevelNumber);
		}
		ui.plantBombCompletedParent.SetActive(true);
	}

	public void pauseGameOnClick()
	{
		Debug.Log("GameController : pauseGameOnClick");
		ui.btnClick.Play();
		Time.timeScale = 0f;
		ui.showPauseMenu();
	}

	public void resumeGameOnclick()
	{
		ui.btnClick.Play();
		Time.timeScale = 1f;
		ui.hidePauseMenu();
	}

	public void doubleRewardOnClick()
	{
		if (CtrlYa.Instance)
		{
			CtrlYa.Instance.OnClickUiShowRewardAsk(() =>
				{
					Debug.LogError("хз что куда");
					MainMenuOnRewardSucces(100);
					
				}, true
			);
		}
	}
	
	private void MainMenuOnRewardSucces(int reward)
	{
		Debug.Log("MainMenuOnRewardSucces : " + reward);
		addRewardAfterWatchingAd(reward);
	}
	
	public void addRewardAfterWatchingAd(int amount)
	{
		StartCoroutine(showAddCurrencyParticles(Vector3.zero, amount, 0f));
	}
	
	public IEnumerator showAddCurrencyParticles(Vector3 startPos, int amountToAd, float startWait)
	{
		Debug.Log("Тут скопировано с мейн меню контроллера, может не обновлться UI.Yj lj,fdkztncz");
		yield return new WaitForSeconds(startWait);
		//currencyToAdObj.GetComponentInChildren<Text>().text = amountToAd.ToString();
		//currencyToAdObj.transform.localScale = Vector3.one * 1.5f;
		//currencyToAdObj.SetActive(true);
		//LeanTween.scale(currencyToAdObj, Vector3.one, 0.5f).setEase(LeanTweenType.linear);
		//LeanTween.moveLocal(currencyToAdObj, new Vector3(500f, 323f, 0f), 0.5f).setEase(LeanTweenType.easeInBack);
		yield return new WaitForSeconds(0.5f);
		//spendCurrenyParticles.Emit(15);
		//spendCurrenyParticles.GetComponent<AudioSource>().Play();
		//currencyParent.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		//LeanTween.scale(currencyParent, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack);
		//currencyToAdObj.SetActive(false);
		CtrlYa.Instance.SaveDollars(amountToAd);
		//totalDollarsText.text = totalDollars2.ToString();
	}

	public void Zooming()
	{
		Debug.Log("Zooming");
		if (!_fpsPlayer.IronsightsComponent.reloading)
		{
			input.input.zoomHold = !input.input.zoomHold;
		}
	}

	
	
	private void OnRewardedVideoCallBack(int _valueOfReward, string _rewardAmountType)
	{
		if (_rewardAmountType.Equals("DoubleReward"))
		{
			doubltTheReward();
		}
	}

	public void Crouch(bool status)
	{
		Debug.Log("Crouch : status = " + status);
		input.input.Crouch(status);
	}

	public void Jump(bool status)
	{
		Debug.Log("Jump : status = " + status);
		input.input.jumpPress = status;
	}

	private void doubltTheReward()
	{
		ui.doubleReward();
	}

	public void creaateIconOnMap()
	{
		if (isIconOnMapCreated)
		{
			return;
		}
		foreach (AI levelEnemy in level.levelEnemies)
		{
			if (levelEnemy != null)
			{
				levelEnemy.SendMessage("createIconOnActiveMap");
			}
		}
		isIconOnMapCreated = true;
	}
}
