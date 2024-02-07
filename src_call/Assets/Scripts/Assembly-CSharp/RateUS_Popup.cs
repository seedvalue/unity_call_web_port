using UnityEngine;
using UnityEngine.UI;

public class RateUS_Popup : MonoBehaviour
{
	public static RateUS_Popup instance;

	public int displayPopupAfter = 3;

	public GameObject popUp;

	private int rateUScount;

	private const string isRated = "isRated";

	private const string isPlayedOnce = "isPlayedOnce";

	public Text textObject;

	private void Awake()
	{
		if (instance == null)
		{
			instance = GetComponent<RateUS_Popup>();
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (instance != GetComponent<RateUS_Popup>())
		{
			Object.Destroy(base.gameObject);
		}
		
		
		Debug.Log("RateUS_Popup : Убиваем эту херню");
		gameObject.SetActive(false);
		
		//закоментил хуету
		//Invoke("Defaultpopup", 1f);
	}

	private void Start()
	{
		textObject.text = "Please take a moment to rate \n" + Application.productName + " !";
	}

	private void Defaultpopup()
	{
		if (PlayerPrefs.GetInt("isPlayedOnce", 0) == 1)
		{
			if (PlayerPrefs.GetInt("isRated", 0) == 0)
			{
				popUp.SetActive(true);
			}
		}
		else
		{
			PlayerPrefs.SetInt("isPlayedOnce", 1);
		}
	}

	public void CheckRateUS()
	{
		if (PlayerPrefs.GetInt("isRated", 0) == 0)
		{
			rateUScount++;
			if (rateUScount > displayPopupAfter)
			{
				rateUScount = 0;
				popUp.SetActive(true);
			}
		}
	}

	public void SetGameAsRated()
	{
		PlayerPrefs.SetInt("isRated", 1);
	}

	public void btnRateClick()
	{
		Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier);
		popUp.SetActive(false);
		popUp.SetActive(false);
		SetGameAsRated();
	}

	public void btnNoClick()
	{
		popUp.SetActive(false);
		popUp.SetActive(false);
	}
}
