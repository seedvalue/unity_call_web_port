using System.Collections;
using _0_WebPort;
using _00_YaTutor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	public GameObject hudParent;

	public GameObject levelStartParent;

	public GameObject startMissionBtn;

	public GameObject missionCompleteParent;

	public GameObject hpParent;

	public GameObject gameOverParent;

	public GameObject hintMenuParent;

	public GameObject hintBtnObj;

	public GameObject hintDeatilObj;

	public Text hintDetailText;

	public Text missionDetailText;

	public GameObject levelClearHeading;

	public GameObject enemiesKilledRow;

	public GameObject headShotsRow;

	public GameObject healtBonusRow;

	public GameObject buttonParent;

	public GameObject totalRewardParent;

	public Text totalRewardText;

	public Text enemiesKilledVal;

	public Text headShotsVal;

	public Text healthRemainingVal;

	public Text enemiesKilledBonus;

	public Text headShotsBonus;

	public Text healthRemainingBonus;

	public GameObject centerLine;

	public GameObject hostageLevelCompleteParent;

	public GameObject loadingScreen;

	public GameObject timerObj;

	public GameObject flashbangBtn;

	public GameObject grenadeBtn;

	public GameObject flashBangInstructionPopup;

	public Text ammoText;

	public Image ammoFillImage;

	public Text HPText;

	public Text grenadeCountText;

	public Text flashbangCountText;

	public GameObject headShotEffect;

	public GameObject bombBtn;

	public GameObject plantBombCompletedParent;

	public GameObject pauseMenuParent;

	public AudioSource btnClick;

	public AudioSource counterSound;

	private int totalRewardEarned;

	public Button doubleRewardBtn;

	public Slider sensitivityslider;

	public GameController gc;

	public Image soundsImg;

	public Sprite soundsOn;

	public Sprite soundsOff;

	public GameObject joystickParent;

	public SmoothMouseLook smoothMouseLook;

	public InputControl inputControl;

	public Button autoAimBtn;

	public GameObject fireButton;

	public GameObject autoShootBtn;

	public Sprite autoShoot;

	public Sprite notAutoShoot;

	private bool isAutoAim = true;

	public static UIController instanse;

	public Text enemyCount;

	private int totalTemp;

	private void Awake()
	{
		instanse = base.gameObject.GetComponent<UIController>();
	}

	private void OnEnable()
	{
		if (PlayerPrefs.GetString("isManualShoot", "YES") == "YES")
		{
			isAutoAim = false;
			autoAimBtn.GetComponent<Image>().sprite = notAutoShoot;
			fireButton.SetActive(true);
		}
		else
		{
			isAutoAim = true;
			autoAimBtn.GetComponent<Image>().sprite = autoShoot;
			fireButton.SetActive(false);
		}
		btnClick = GetComponents<AudioSource>()[0];
		counterSound = GetComponents<AudioSource>()[1];
		SetSliderValue();
		autoAimBtn.onClick.AddListener(delegate
		{
			AutoAimPress();
		});
	}

	private void AutoAimPress()
	{
		if (PlayerPrefs.GetString("isManualShoot") == "YES")
		{
			PlayerPrefs.SetString("isManualShoot", "NO");
			PlayerPrefs.Save();
			isAutoAim = true;
			autoAimBtn.GetComponent<Image>().sprite = autoShoot;
			fireButton.SetActive(false);
		}
		else
		{
			PlayerPrefs.SetString("isManualShoot", "YES");
			PlayerPrefs.Save();
			isAutoAim = false;
			autoAimBtn.GetComponent<Image>().sprite = notAutoShoot;
			fireButton.SetActive(true);
		}
	}

	public IEnumerator showLevelStartDetails(string message)
	{
		missionDetailText.text = message;
		startMissionBtn.transform.localScale = Vector3.zero;
		yield return new WaitForSeconds(1.5f);
		levelStartParent.SetActive(true);
		startMissionBtn.SetActive(true);
		LeanTween.scale(startMissionBtn, Vector3.one, 0.3f).setEase(LeanTweenType.easeOutBack);
		if (CtrlYa.Instance)
		{
			CtrlYa.Instance.SetCursorLocked(false);
		}else Debug.LogError("CtrlGamePush.Instance == NULL");
	}

	public IEnumerator showHud()
	{
		if (CtrlYa.Instance)
		{
			CtrlYa.Instance.SetCursorLocked(true);
		} else Debug.LogError("CtrlYa.Instance == NULL");
		
		levelStartParent.SetActive(false);
		hudParent.SetActive(true);
		yield return null;
	}

	public void showGrenadeBtns()
	{
		grenadeBtn.SetActive(true);
		flashbangBtn.SetActive(true);
	}

	public void hideGrenadeBtns()
	{
		grenadeBtn.SetActive(false);
		flashbangBtn.SetActive(false);
	}

	private void setUpHUdButton()
	{
		if (Globals.currentLevelNumber < 5 || Globals.currentLevelNumber == 15)
		{
			grenadeBtn.SetActive(false);
			flashbangBtn.SetActive(false);
		}
		else if (Globals.currentLevelNumber == 5)
		{
			if (PlayerPrefs.GetInt("isLevel5FailedOnce", 0) == 1)
			{
				showFlashBangInstructions();
				grenadeBtn.SetActive(false);
			}
			else
			{
				grenadeBtn.SetActive(false);
				flashbangBtn.SetActive(false);
			}
		}
		if (Globals.currentLevelNumber > 5 && Globals.currentLevelNumber != 15)
		{
			grenadeBtn.SetActive(true);
			flashbangBtn.SetActive(true);
		}
	}

	public void showMissionCompleteUI(int enemiesKilled, int enemiesReward, int headShots, int headshotsReward, int healthRemaining, int healthBonusReward)
	{
		if (CtrlYa.Instance)
		{
			Debug.LogError("TO NEW SDK CtrlGamePush.Instance == NULL");
			//CtrlGamePush.Instance.SetCursorLocked(false);
		} else Debug.LogError("CtrlGamePush.Instance == NULL");
		
		
		int num = 0;
		float num2 = 0f;
		hudParent.SetActive(false);
		if (PlayerPrefs.GetInt("LevelsCleared", 0) < Globals.currentLevelNumber)
		{
			PlayerPrefs.SetInt("LevelsCleared", Globals.currentLevelNumber);
		}
		totalTemp = enemiesReward + healthBonusReward + headshotsReward;
		PlayerPrefs.SetInt("Dollars", PlayerPrefs.GetInt("Dollars", 0) + totalTemp);
		missionCompleteParent.SetActive(true);
		enemiesKilledVal.text = enemiesKilled.ToString();
		enemiesKilledBonus.text = enemiesReward.ToString();
		headShotsVal.text = headShots.ToString();
		headShotsBonus.text = headshotsReward.ToString();
		healthRemainingVal.text = healthRemaining + "%";
		healthRemainingBonus.text = healthBonusReward.ToString();
		//buttonParent.transform.localScale = Vector3.zero;
		buttonParent.SetActive(true);
		//LeanTween.scale(buttonParent, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack);
		//levelClearHeading.transform.localScale = Vector3.one * 2.5f;
		levelClearHeading.SetActive(true);
		//LeanTween.scale(levelClearHeading, Vector3.one, 0.05f).setEase(LeanTweenType.linear);
		//totalRewardParent.transform.localScale = Vector3.zero;
		totalRewardParent.SetActive(true);
		//LeanTween.scale(totalRewardParent, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack);
		//centerLine.transform.localScale = Vector3.zero;
		centerLine.SetActive(true);
		//LeanTween.scale(centerLine, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack);
		//enemiesKilledRow.transform.localScale = Vector3.zero;
		enemiesKilledRow.SetActive(true);
		//LeanTween.scale(enemiesKilledRow, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack);
		counterSound.Play();
		while (num != enemiesReward)
		{
			num2 = Mathf.MoveTowards(num2, enemiesReward, 500f * Time.deltaTime);
			num = (int)num2;
			totalRewardText.text = num.ToString();
		}
		counterSound.Stop();
		//headShotsRow.transform.localScale = Vector3.zero;
		headShotsRow.SetActive(true);
		//LeanTween.scale(headShotsRow, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack);
		int num3 = num + headshotsReward;
		num = num3;
		totalRewardText.text = num.ToString();
		//healtBonusRow.transform.localScale = Vector3.zero;
		healtBonusRow.SetActive(true);
		//LeanTween.scale(healtBonusRow, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack);
		num3 = num + healthBonusReward;
		counterSound.Play();
		while (num != num3)
		{
			num2 = Mathf.MoveTowards(num, num3, 100f * Time.deltaTime);
			num = (int)num2;
			totalRewardText.text = num.ToString();
		}
		counterSound.Stop();
		totalRewardEarned = num;
		

		Debug.Log("on game complite");
		
	}

	public void showGameOver()
	{
		Debug.Log("showGameOver");
		if (CtrlYa.Instance)
		{
			Debug.LogError("NEW SDK cursor");
			//CtrlGamePush.Instance.SetCursorLocked(false);
		} else Debug.LogError("CtrlGamePush.Instance == NULL");
		
		hudParent.SetActive(false);
		gameOverParent.SetActive(true);
		Globals.levelsFailedOnTrot++;
		if (Globals.levelsFailedOnTrot == 4)
		{
			Globals.levelsFailedOnTrot = 0;
		}
	}

	private void playVideoAd()
	{
	}

	public void showHintAndTimer()
	{
		hintMenuParent.SetActive(true);
		hudParent.SetActive(false);
		timerObj.SetActive(true);
	}

	public void showBombTimer()
	{
		timerObj.GetComponent<TimerController>().detonateBomb = true;
		timerObj.GetComponent<TimerController>().totalTime = 15;
		timerObj.GetComponent<TimerController>().addtionalText = " to move away from the bomb";
		timerObj.SetActive(true);
	}

	public void showHintDetails(string hintText)
	{
		hintDetailText.text = hintText;
		hintBtnObj.SetActive(false);
		hintDeatilObj.SetActive(true);
	}

	public void showHostageLevelComplete()
	{
		hudParent.SetActive(false);
		if (PlayerPrefs.GetInt("LevelsCleared") < Globals.currentLevelNumber)
		{
			PlayerPrefs.SetInt("LevelsCleared", Globals.currentLevelNumber);
		}
		hostageLevelCompleteParent.SetActive(true);
	}

	public void loadMainMenu()
	{
		Debug.Log("loadMainMenu");
		if (CtrlYa.Instance)
		{
			Debug.LogError("new sdk CURSOR");
			//CtrlGamePush.Instance.SetCursorLocked(false);
		} else Debug.LogError("CtrlGamePush.Instance == NULL");
		
		btnClick.Play();
		loadingScreen.SetActive(true);
		SceneManager.LoadScene("MainMenu");
	}

	public void restartLevel()
	{
		btnClick.Play();
		loadingScreen.SetActive(true);
		Debug.Log("restartLevel");
		SceneManager.LoadScene("Gameplay");
	}

	private void showFlashBangInstructions()
	{
		hudParent.SetActive(false);
		flashBangInstructionPopup.SetActive(true);
	}

	public void gotFlashBangInstructions()
	{
		hudParent.SetActive(true);
		flashBangInstructionPopup.SetActive(false);
		flashbangBtn.SetActive(true);
	}

	public void updateAmmoInfo(int bulletsLeft, int totalBullets)
	{
		ammoText.text = bulletsLeft + "/" + totalBullets;
		ammoFillImage.fillAmount = (float)bulletsLeft / (float)totalBullets;
	}

	public void updateHealth(int health)
	{
		HPText.text = health.ToString();
	}

	public void updateFlashCounter(int val)
	{
		flashbangCountText.text = val.ToString();
	}

	public void updateGrenadeCounter(int val)
	{
		grenadeCountText.text = val.ToString();
	}

	public void showBombBtn()
	{
		bombBtn.SetActive(true);
		bombBtn.GetComponent<Button>().interactable = false;
	}

	public void enableBombBtn()
	{
		bombBtn.GetComponent<Button>().interactable = true;
	}

	public void hideBombBtn()
	{
		bombBtn.SetActive(false);
	}

	public void showPauseMenu()
	{
		hudParent.SetActive(false);
		sensitivityslider.value = PlayerPrefs.GetFloat("Sensitivity", 0.5f);
		setUpSound();
		pauseMenuParent.SetActive(true);
	}

	public void hidePauseMenu()
	{
		hudParent.SetActive(true);
		pauseMenuParent.SetActive(false);
	}

	public void doubleReward()
	{
		totalRewardEarned *= 2;
		totalRewardText.text = totalRewardEarned.ToString();
		doubleRewardBtn.interactable = false;
		Debug.Log("totalTemp: " + totalTemp);
		PlayerPrefs.SetInt("Dollars", PlayerPrefs.GetInt("Dollars", 0) + totalTemp);
	}

	private void SetSliderValue()
	{
		sensitivityslider.value = PlayerPrefs.GetFloat("MySliderValue", 0.5f);
	}

	public void sensitivitySliderOnClick()
	{
		PlayerPrefs.SetFloat("Sensitivity", sensitivityslider.value);
		PlayerPrefs.SetFloat("MySliderValue", sensitivityslider.value);
		smoothMouseLook.updateSensitivity();
	}

	public void soundsBtnOnClick()
	{
		if (PlayerPrefs.GetInt("Sound", 1) == 1)
		{
			PlayerPrefs.SetInt("Sound", 0);
		}
		else
		{
			PlayerPrefs.SetInt("Sound", 1);
		}
		Debug.Log("Called");
		setUpSound();
	}

	private void setUpSound()
	{
		if (PlayerPrefs.GetInt("Sound", 1) == 1)
		{
			soundsImg.sprite = soundsOn;
			soundsImg.SetNativeSize();
			AudioListener.volume = 1f;
		}
		else
		{
			soundsImg.sprite = soundsOff;
			soundsImg.SetNativeSize();
			AudioListener.volume = 0f;
		}
	}

	public void OnClickMap(int MapId)
	{
		if (MapId < 0)
		{
			GameController.instance.noMap.SetActive(true);
		}
		else
		{
			GameController.instance.creaateIconOnMap();
		}
	}

	public void OnClickMapCross()
	{
		GameController.instance.noMap.SetActive(false);
	}
}
