using _00_YaTutor;
using UnityEngine;
using UnityEngine.UI;

public class LevelBtnController : MonoBehaviour
{
	public int levelNumber;

	public Sprite unlockedLevel;

	public Sprite lockedLevel;

	public Image btn_sprite;

	private void Start()
	{
		var levelCleared = 0;
		if (CtrlYa.Instance)
		{
			levelCleared = CtrlYa.Instance.GetFinishedLevel();
		}
		else
		{
			Debug.LogError("LevelBtnController : Start :  CtrlYa.Instance == null. when get level cleared");
		}
		
		if (levelCleared + 1 < levelNumber)
		{
			InitLevelBtn(lockedLevel, false);
		}
		else if (levelCleared == levelNumber - 1)
		{
			HightLight(1.2f);
			InitLevelBtn(unlockedLevel, true);
			Globals.currentLevelNumber = levelNumber;
		}
		
		Debug.Log("Sergei : prefs level cleared button setup");
		/*
		if (PlayerPrefs.GetInt("LevelsCleared", 0) + 1 < levelNumber)
		{
			InitLevelBtn(lockedLevel, false);
		}
		else if (PlayerPrefs.GetInt("LevelsCleared", 0) == levelNumber - 1)
		{
			HightLight(1.2f);
			InitLevelBtn(unlockedLevel, true);
			Globals.currentLevelNumber = levelNumber;
		}*/
	}

	private void InitLevelBtn(Sprite spr, bool status)
	{
		btn_sprite.sprite = spr;
		GetComponent<Button>().interactable = status;
		if (!status)
		{
			base.transform.GetChild(0).GetComponentInChildren<Text>().gameObject.SetActive(false);
		}
	}

	public void HightLight(float v)
	{
		Vector3 localScale = new Vector3(v, v, v);
		base.transform.localScale = localScale;
	}
}
