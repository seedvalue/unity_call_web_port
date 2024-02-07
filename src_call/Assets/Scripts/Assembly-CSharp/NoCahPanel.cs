using _00_YaTutor;
using UnityEngine;
using UnityEngine.UI;

public class NoCahPanel : MonoBehaviour
{
	public Button backBtn;

	public Button watchVideoBtn;

	public GameObject noCashEffect;

	private void OnEnable()
	{
		backBtn.onClick.AddListener(delegate
		{
			BackButtonPress();
		});
		watchVideoBtn.onClick.AddListener(delegate
		{
			WatchVideoPress();
		});
	}

	private void BackButtonPress()
	{
		base.gameObject.SetActive(false);
	}

	private void WatchVideoPress()
	{
		if (CtrlYa.Instance)
		{
			CtrlYa.Instance.OnClickUiShowRewardAsk(OnActionOk,true);
		}
		else
		{
			Debug.LogError("NoCahPanel : WatchVideoPress : CtrlYa.Instance == NULL");
		}
		
		return;
		
		/*
		else
		{
			noCashEffect.SetActive(true);
			Invoke("DisableEffect", 2f);
		}
		*/
	}
	
	private void OnActionOk()
	{
		Debug.Log("WeaponPopupController : OnActionOk");
		OnRewardedVideoCallBack(100, "Reward");
	}

	private void DisableEffect()
	{
		noCashEffect.SetActive(false);
	}

	private void OnRewardedVideoCallBack(int _valueOfReward, string _rewardAmountType)
	{
		if (_rewardAmountType.Equals("Reward"))
		{
			Object.FindObjectOfType<MainMenuController>().addRewardAfterWatchingAd(_valueOfReward);
			/*
			if(IntegrationManager.Instance)
				IntegrationManager.Instance.OnRewardedVideoCallBack -= OnRewardedVideoCallBack;
			else Debug.LogError("IntegrationManager.Instance == NULL");
			*/
		}
	}
}
