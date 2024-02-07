using System.Collections;
using _00_YaTutor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
	public GameObject mainMenuParent;

	public GameObject levelSelectionParent;

	public GameObject gunSelectionParent;

	public GameObject loadingScreen;

	public LevelBtnController[] levelBtns;

	public RectTransform topBar;

	public Text totalDollarsText;

	private int dollars;

	public CamBlurController camBlur;

	public Animator camAnimator;

	public ParticleSystem spendCurrenyParticles;

	public GameObject currencyParent;

	public GameObject welcomeMsgObj;

	public GameObject currencyToAdObj;

	public GameObject storeParent;

	private int menuState;

	public AudioSource btnClick;

	public AudioSource startGameSound;

	public Sprite soundsOn;

	public Sprite soundsOff;

	public Image soundsImg;

	private AudioSource music;

	public GameObject moreGamesObj;

	public GameObject settingsObj;

	public Slider sensitivitySlider;

	public bool isClearPlayerPrefs;

	public Button removeAds;

	public GameObject mainBg;

	public GameObject exitDialog;

	private void Awake()
	{
		if (isClearPlayerPrefs)
		{
			PlayerPrefs.DeleteAll();
		}
		Globals.levelsFailedOnTrot = 0;
		if (PlayerPrefs.GetString("Animation", "NotPlayed") == "NotPlayed")
		{
		}
		btnClick = GetComponents<AudioSource>()[0];
		startGameSound = GetComponents<AudioSource>()[1];
		music = GetComponents<AudioSource>()[2];
		Time.timeScale = 1f;
		hideTopBar();
		updateDollarsText();
		setUpSound();
		Invoke("BlurScreen", 2f);
		StartCoroutine(fadeInMusic());
		removeAds.onClick.AddListener(delegate
		{
			RemoveAdsOnClick();
		});
	}

	private void Start()
	{
		mainBg.SetActive(true);
	}

	public void updateDollarsText()
	{
		dollars = PlayerPrefs.GetInt("Dollars", 0);
		totalDollarsText.text = dollars.ToString();
	}

	public void showSpendCurrencyParticles()
	{
		spendCurrenyParticles.Emit(15);
		spendCurrenyParticles.GetComponent<AudioSource>().Play();
		currencyParent.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		LeanTween.scale(currencyParent, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack);
	}

	private IEnumerator fadeInMusic()
	{
		music.volume = 0f;
		music.Play();
		while (music.volume != 1f)
		{
			music.volume = Mathf.MoveTowards(music.volume, 1f, 0.5f * Time.deltaTime);
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

	public IEnumerator showAddCurrencyParticles(Vector3 startPos, int amountToAd, float startWait)
	{
		yield return new WaitForSeconds(startWait);
		currencyToAdObj.GetComponentInChildren<Text>().text = amountToAd.ToString();
		currencyToAdObj.transform.localScale = Vector3.one * 1.5f;
		currencyToAdObj.SetActive(true);
		LeanTween.scale(currencyToAdObj, Vector3.one, 0.5f).setEase(LeanTweenType.linear);
		LeanTween.moveLocal(currencyToAdObj, new Vector3(500f, 323f, 0f), 0.5f).setEase(LeanTweenType.easeInBack);
		yield return new WaitForSeconds(0.5f);
		spendCurrenyParticles.Emit(15);
		spendCurrenyParticles.GetComponent<AudioSource>().Play();
		currencyParent.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		LeanTween.scale(currencyParent, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack);
		currencyToAdObj.SetActive(false);
		int totalDollars2 = PlayerPrefs.GetInt("Dollars", 0);
		totalDollars2 += amountToAd;
		PlayerPrefs.SetInt("Dollars", totalDollars2);
		totalDollarsText.text = totalDollars2.ToString();
	}

	public void BlurScreen()
	{
	}

	private void RemoveAdsOnClick()
	{
		PlayerPrefs.SetString("RemoveAds", "true");
	}

	private void OnEnable()
	{
		StartCoroutine(showMainMenu(2f));
	}

	private IEnumerator showMainMenu(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		showTopBar();
		mainMenuParent.SetActive(true);
		if (PlayerPrefs.GetInt("StartCashAwarded", 0) == 0)
		{
			Invoke("GiveStartRewardPopup", 1f);
		}
	}

	private void GiveStartRewardPopup()
	{
		welcomeMsgObj.SetActive(true);
	}

	private void showTopBar()
	{
		LeanTween.move(topBar, new Vector3(0f, -30f, 0f), 0.15f).setEase(LeanTweenType.linear);
	}

	public void hideTopBar()
	{
		LeanTween.move(topBar, new Vector3(0f, 130f, 0f), 0.15f).setEase(LeanTweenType.linear);
	}

	public void nextFromMainMenu()
	{
		btnClick.Play();
		hideTopBar();
		mainMenuParent.SetActive(false);
		levelSelectionParent.SetActive(true);
	}

	public void nextFromlevelSelection()
	{
		btnClick.Play();
		menuState = 1;
		levelSelectionParent.SetActive(false);
		showTopBar();
		gunSelectionParent.SetActive(true);
		mainBg.SetActive(false);
	}

	public void nextFromGunSelection()
	{
		Debug.Log("OnLoadingGameplay");
		startGameSound.Play();
		loadingScreen.SetActive(true);
		StartCoroutine(fadeOutMusic());
		SceneManager.LoadSceneAsync("Gameplay");
	}

	public void backFomLevelSelection()
	{
		btnClick.Play();
		menuState = 0;
		levelSelectionParent.SetActive(false);
		StartCoroutine(showMainMenu(0.2f));
	}

	public void backeFromGunSelection()
	{
		btnClick.Play();
		gunSelectionParent.SetActive(false);
		hideTopBar();
		levelSelectionParent.SetActive(true);
		mainBg.SetActive(true);
	}

	public void levelSelected(int num)
	{
		btnClick.Play();
		for (int i = 0; i < levelBtns.Length; i++)
		{
			if (i != num - 1)
			{
				levelBtns[i].HightLight(1f);
			}
			else
			{
				levelBtns[i].HightLight(1.2f);
			}
		}
		Globals.currentLevelNumber = num;
	}

	public void takeStartCashOnClick()
	{
		btnClick.Play();
		PlayerPrefs.SetInt("StartCashAwarded", 1);
		welcomeMsgObj.SetActive(false);
		StartCoroutine(showAddCurrencyParticles(Vector3.zero, 300, 0f));
	}

	public void watchVideoOnClick()
	{
		Debug.Log("MainMenuController : watchVideoOnClick");
		btnClick.Play();
		if (CtrlYa.Instance)
		{
			CtrlYa.Instance.OnClickUiShowRewardAsk(() =>
				{
					MainMenuOnRewardSucces(100);
				}, true
				);
		}
		else
		{
			Debug.LogError("MainMenuController : CtrlYa.Instance == NULL");
		}
		
		/*
		if (IntegrationManager.Instance.HasRewardedVideo())
		{
			IntegrationManager.Instance.ShowRewardedVideo(100, "MainMenuCash");
		}
		*/
		
	}

	// Sergei
	private void MainMenuOnRewardSucces(int reward)
	{
		Debug.Log("MainMenuOnRewardSucces : " + reward);
		addRewardAfterWatchingAd(reward);
	}

	public void addRewardAfterWatchingAd(int amount)
	{
		StartCoroutine(showAddCurrencyParticles(Vector3.zero, amount, 0f));
	}

	private void OnRewardedVideoCallBack(int _valueOfReward, string _rewardAmountType)
	{
		if (_rewardAmountType.Equals("MainMenuCash"))
		{
			addRewardAfterWatchingAd(_valueOfReward);
		}
		//if(IntegrationManager.Instance)
		//	IntegrationManager.Instance.OnRewardedVideoCallBack -= OnRewardedVideoCallBack;
		//else Debug.LogError("IntegrationManager.Instance == NULL");
	}

	public void showStoreOnClick()
	{
		Debug.Log("showStoreOnClick");
		btnClick.Play();
		if (CtrlYa.Instance)
		{
			CtrlYa.Instance.OnClickUiShowRewardAsk(() =>
				{
					MainMenuOnRewardSucces(500);
				}, true
			);
		}
		else
		{
			Debug.LogError("MainMenuController : CtrlYa.Instance == NULL");
		}
		
		/*
		storeParent.SetActive(true);
		if (menuState == 0)
		{
			hideTopBar();
			mainMenuParent.SetActive(false);
		}
		if (menuState == 1)
		{
			hideTopBar();
			gunSelectionParent.SetActive(false);
		}
		mainBg.SetActive(true);
		*/
	}

	public void backFromStore()
	{
		btnClick.Play();
		storeParent.SetActive(false);
		if (menuState == 0)
		{
			showTopBar();
			mainMenuParent.SetActive(true);
			mainBg.SetActive(true);
		}
		if (menuState == 1)
		{
			showTopBar();
			gunSelectionParent.SetActive(true);
			mainBg.SetActive(false);
		}
	}

	public void inAppBtnOnclick(int bundelIndex)
	{
		PlayerPrefs.SetString("RemoveAds", "true");
		btnClick.Play();
		if (bundelIndex == 1)
		{
			backFromStore();
			addCurrency(4000);
		}
		if (bundelIndex == 2)
		{
			backFromStore();
			addCurrency(10000);
		}
		if (bundelIndex == 3)
		{
			backFromStore();
			addCurrency(20000);
		}
	}

	private void OnPurchaseSucceedCallBack(string sku)
	{
		if (sku.Equals("cp_4"))
		{
			backFromStore();
			addCurrency(4000);
		}
		else if (sku.Equals("cp_5"))
		{
			backFromStore();
			addCurrency(10000);
		}
		else if (sku.Equals("cp_6"))
		{
			backFromStore();
			addCurrency(20000);
		}
		Debug.Log(sku);
		PlayerPrefs.SetString("RemoveAds", "true");
		/*
		if(IntegrationManager.Instance)
			IntegrationManager.Instance.OnPurchaseSucceedCallBack -= OnPurchaseSucceedCallBack;
		else Debug.LogError("IntegrationManager.Instance == NULL");
		*/
	}

	public void OnClickExit()
	{
		Debug.Log("OnClickExit");
		/*
		if ((bool)IntegrationManager.Instance)
		{
			IntegrationManager.Instance.OnExit();
		}
		*/
		exitDialog.SetActive(true);
	}

	public void OnClickExitNO()
	{
		exitDialog.SetActive(false);
	}

	public void OnClickExitYES()
	{
		Application.Quit();
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

	public void rateUsOnClick()
	{
		RateUS_Popup.instance.CheckRateUS();
	}

	public void addCurrency(int val)
	{
		StartCoroutine(showAddCurrencyParticles(Vector3.zero, val, 1.5f));
	}

	public void moreGamesOnClick()
	{
		moreGamesObj.SetActive(true);
	}

	public void settingsOnClick()
	{
		btnClick.Play();
		sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity", 0.5f);
		mainMenuParent.SetActive(false);
		hideTopBar();
		settingsObj.SetActive(true);
		Debug.Log("settingsOnClick");
	}

	public void backFromSetting()
	{
		btnClick.Play();
		settingsObj.SetActive(false);
		showTopBar();
		mainMenuParent.SetActive(true);
	}

	public void OnSensitivityValueChanged()
	{
		PlayerPrefs.SetFloat("Sensitivity", sensitivitySlider.value);
	}

	public void tetsPuchaseOnClick()
	{
		/*
		if (IntegrationManager.Instance)
		{
			Debug.Log("tetsPuchaseOnClick");
			//IntegrationManager.Instance.PurchaseProduct("android.test.purchased");
		}
		else Debug.LogError("IntegrationManager.Instance == NULL");
		*/
	}
}
