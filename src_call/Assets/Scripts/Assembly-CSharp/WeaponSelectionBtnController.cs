using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectionBtnController : MonoBehaviour
{
	public string weaponName;

	public int gunIndex;

	public int gunPrice;

	public GameObject selectButton;

	public GameObject selectedText;

	public GameObject buyObj;

	public GameObject gun3D;

	private void OnEnable()
	{
		if (PlayerPrefs.GetInt("Gun" + gunIndex + "Bought", 0) == 0)
		{
			buyObj.SetActive(true);
			selectButton.SetActive(false);
			selectedText.SetActive(false);
			return;
		}
		buyObj.SetActive(false);
		if (PlayerPrefs.GetInt("SelectedGunIndex", -1) == gunIndex)
		{
			selectedText.SetActive(true);
			buyObj.SetActive(false);
			selectButton.SetActive(false);
		}
		else
		{
			selectedText.SetActive(false);
			buyObj.SetActive(false);
			selectButton.SetActive(true);
		}
	}

	private void OnDisable()
	{
		gun3D.SetActive(false);
	}

	public void purchaseGun()
	{
	}

	public void selectGun()
	{
	}

	public void setUpNewIndex(int index)
	{
		if (index == gunIndex)
		{
			gun3D.SetActive(true);
		}
		else
		{
			gun3D.SetActive(false);
		}
		if (index != gunIndex)
		{
			StartCoroutine(fadeOutButton());
		}
		else
		{
			StartCoroutine(fadeInButton());
		}
	}

	private IEnumerator fadeOutButton()
	{
		float targetAlpha = 0.9f;
		while (targetAlpha != 0.5f)
		{
			targetAlpha = Mathf.MoveTowards(targetAlpha, 0.5f, 1f * Time.deltaTime);
			Color col = GetComponent<Image>().color;
			col.a = targetAlpha;
			GetComponent<Image>().color = col;
			yield return null;
		}
	}

	private IEnumerator fadeInButton()
	{
		float targetAlpha = 0.5f;
		while (targetAlpha != 0.9f)
		{
			targetAlpha = Mathf.MoveTowards(targetAlpha, 0.9f, 1f * Time.deltaTime);
			Color col = GetComponent<Image>().color;
			col.a = targetAlpha;
			GetComponent<Image>().color = col;
			yield return null;
		}
	}
}
