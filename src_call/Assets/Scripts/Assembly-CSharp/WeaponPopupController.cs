using _00_YaTutor;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPopupController : MonoBehaviour
{
	public Button startButton;

	public GameObject gunsWarningSign;

	public GameObject[] gunsIcons;

	private int selectedGunIndex;

	public GameObject gunsSlector;

	public GameObject grenadesUnlockBlock;

	public GameObject grandesLockedBlock;

	public Text grenadeQuanityText;

	public Text flashbangQuantityText;

	public MainMenuController mainMenu;

	public static WeaponPopupController instance;

	public GameObject noCashPanel;

	public GameObject noVideoEffect;

	private void OnEnable()
	{
		if (instance == null)
		{
			instance = this;
		}
		for (int i = 0; i < gunsIcons.Length; i++)
		{
			gunsIcons[i].SetActive(false);
		}
		if (!PlayerPrefs.HasKey("FirstTime"))
		{
			PlayerPrefs.SetInt("FirstTime", 0);
			PlayerPrefs.SetInt("SelectedGunIndex", 0);
			PlayerPrefs.SetInt("Grenades", 3);
		}
		checkGunStats();
	}

	public void openGunSelector()
	{
		mainMenu.btnClick.Play();
		base.gameObject.SetActive(false);
		gunsSlector.SetActive(true);
	}

	private void checkGunStats()
	{
		selectedGunIndex = PlayerPrefs.GetInt("SelectedGunIndex", -1);
		if (selectedGunIndex == -1)
		{
			gunsIcons[0].SetActive(true);
			startButton.interactable = false;
			gunsWarningSign.SetActive(true);
		}
		else
		{
			gunsIcons[selectedGunIndex + 1].SetActive(true);
			gunsWarningSign.SetActive(false);
			startButton.interactable = true;
			if (Globals.currentLevelNumber >= 3 && selectedGunIndex == 0)
			{
				gunsWarningSign.SetActive(true);
			}
			if (Globals.currentLevelNumber >= 9 && selectedGunIndex < 2)
			{
				gunsWarningSign.SetActive(true);
			}
		}

		int levelCleared = 0;
		if (CtrlYa.Instance)
		{
			levelCleared = CtrlYa.Instance.GetFinishedLevel();
		}
		else
		{
			Debug.LogError("WeaponPopupController : checkGunStats :  CtrlYa.Instance == null. when get level cleared");
		}
		
		if (levelCleared >= 0)
		{
			grandesLockedBlock.SetActive(false);
			grenadesUnlockBlock.SetActive(true);
			grenadeQuanityText.text = PlayerPrefs.GetInt("Grenades", 0).ToString();
			flashbangQuantityText.text = PlayerPrefs.GetInt("Flashbang", 0).ToString();
		}
		else
		{
			grandesLockedBlock.SetActive(true);
			grenadesUnlockBlock.SetActive(false);
		}
		
		/*
		if (PlayerPrefs.GetInt("LevelsCleared", 0) >= 0)
		{
			grandesLockedBlock.SetActive(false);
			grenadesUnlockBlock.SetActive(true);
			grenadeQuanityText.text = PlayerPrefs.GetInt("Grenades", 0).ToString();
			flashbangQuantityText.text = PlayerPrefs.GetInt("Flashbang", 0).ToString();
		}
		else
		{
			grandesLockedBlock.SetActive(true);
			grenadesUnlockBlock.SetActive(false);
		}
		*/
	}

	public void buyFlashbangOnClick()
	{
		mainMenu.btnClick.Play();
		int @int = PlayerPrefs.GetInt("Dollars", 0);
		if (@int >= 600)
		{
			PlayerPrefs.SetInt("Dollars", @int - 600);
			mainMenu.updateDollarsText();
			mainMenu.showSpendCurrencyParticles();
			int int2 = PlayerPrefs.GetInt("Flashbang", 0);
			int2++;
			PlayerPrefs.SetInt("Flashbang", int2);
			flashbangQuantityText.text = int2.ToString();
		}
		else
		{
			noCashPanel.SetActive(true);
		}
	}

	public void buyGrenadeOnClick()
	{
		mainMenu.btnClick.Play();
		int @int = PlayerPrefs.GetInt("Dollars", 0);
		if (@int >= 600)
		{
			PlayerPrefs.SetInt("Dollars", @int - 600);
			mainMenu.updateDollarsText();
			mainMenu.showSpendCurrencyParticles();
			int int2 = PlayerPrefs.GetInt("Grenades", 0);
			int2++;
			PlayerPrefs.SetInt("Grenades", int2);
			grenadeQuanityText.text = int2.ToString();
		}
		else
		{
			noCashPanel.SetActive(true);
		}
	}

	public void AddGrenade(bool isFlash, int amount)
	{
		Debug.Log("bool is : " + isFlash + "amount is : " + amount);
		if (isFlash)
		{
			mainMenu.showSpendCurrencyParticles();
			int @int = PlayerPrefs.GetInt("Flashbang", 0);
			@int += amount;
			PlayerPrefs.SetInt("Flashbang", @int);
			flashbangQuantityText.text = @int.ToString();
		}
		else
		{
			mainMenu.showSpendCurrencyParticles();
			int int2 = PlayerPrefs.GetInt("Grenades", 0);
			int2 += amount;
			PlayerPrefs.SetInt("Grenades", int2);
			grenadeQuanityText.text = int2.ToString();
		}
	}

	public void WatchVideo(bool isFlashBang)
	{
		Debug.Log("WatchVideo");
		if (CtrlYa.Instance)
		{
			CtrlYa.Instance.OnClickUiShowRewardAsk(OnActionOk,true);
		}
		else
		{
			Debug.LogError("WeaponPopupController : WatchVideo : CtrlYa.Instance == NULL");
		}
		/*
		if (IntegrationManager.Instance.HasRewardedVideo())
		{
			IntegrationManager.Instance.OnRewardedVideoCallBack += OnRewardedVideoCallBack;
			IntegrationManager.Instance.ShowRewardedVideo(3, "Grenade");
		}
		else
		{
			noVideoEffect.SetActive(true);
			Invoke("OffEffect", 2f);
		}*/
	}

	private void OnActionOk()
	{
		Debug.Log("WeaponPopupController : OnActionOk");
		OnRewardedVideoCallBack(3, "Grenade");
	}

	private void OnRewardedVideoCallBack(int _valueOfReward, string _rewardAmountType)
	{
		if (_rewardAmountType.Equals("Grenade"))
		{
			AddGrenade(false, _valueOfReward);
			//IntegrationManager.Instance.OnRewardedVideoCallBack -= OnRewardedVideoCallBack;
		}
	}

	private void OffEffect()
	{
		noVideoEffect.SetActive(false);
	}
}
