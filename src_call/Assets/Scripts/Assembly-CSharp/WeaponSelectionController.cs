using _00_YaTutor;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectionController : MonoBehaviour
{
	private int currentGunIndex;

	public Button nextGunBtn;

	public Button previosGunBtn;

	public RectTransform buttonsParent;

	public WeaponSelectionBtnController[] weapons;

	public GameObject weaponsMainPopupObj;

	public MainMenuController mainMenu;

	public Text smgPriceText;

	public Text ak47PriceText;

	public Text m4PriceText;

	public Text sniperPriceText;

	public static WeaponSelectionController instance;

	public int[] weaponPrice;

	private void OnEnable()
	{
		if (instance == null)
		{
			instance = this;
		}
		updateButtonsInfo();
		if (!PlayerPrefs.HasKey("UserFirstTime"))
		{
			PlayerPrefs.SetInt("UserFirstTime", 0);
			purchaseWeapon(0);
		}
		GetLocalizePrice();
	}

	public void nextGunOnClick()
	{
		mainMenu.btnClick.Play();
		currentGunIndex++;
		Debug.Log(currentGunIndex);
		LeanTween.move(buttonsParent, new Vector3(-50f, currentGunIndex * 280, -100f), 0.4f).setEase(LeanTweenType.easeOutBack);
		updateButtonsInfo();
	}

	public void previosGunOnClick()
	{
		mainMenu.btnClick.Play();
		Debug.Log("clicked");
		currentGunIndex--;
		LeanTween.move(buttonsParent, new Vector3(-50f, currentGunIndex * 280, -100f), 0.4f).setEase(LeanTweenType.easeOutBack);
		updateButtonsInfo();
	}

	private void updateButtonsInfo()
	{
		if (currentGunIndex == 0)
		{
			previosGunBtn.interactable = false;
		}
		else
		{
			previosGunBtn.interactable = true;
		}
		if (currentGunIndex == weapons.Length - 1)
		{
			nextGunBtn.interactable = false;
		}
		else
		{
			nextGunBtn.interactable = true;
		}
		for (int i = 0; i < weapons.Length; i++)
		{
			weapons[i].setUpNewIndex(currentGunIndex);
		}
	}

	public void doneSelection()
	{
		mainMenu.btnClick.Play();
		base.gameObject.SetActive(false);
		weaponsMainPopupObj.SetActive(true);
	}

	public void selectWeapon(int index)
	{
		mainMenu.btnClick.Play();
		PlayerPrefs.SetInt("SelectedGunIndex", index);
		weapons[index].selectButton.SetActive(false);
		weapons[index].selectedText.SetActive(true);
		for (int i = 0; i < weapons.Length; i++)
		{
			if (i != index && !weapons[i].buyObj.activeSelf)
			{
				weapons[i].selectButton.SetActive(true);
				weapons[i].selectedText.SetActive(false);
			}
		}
	}

	public void purchaseWeapon(int index)
	{
		Debug.Log("prefs disabled");
		int @int = CtrlYa.Instance.GetDollars();
		if (@int <= weapons[index].gunPrice)
		{
			return;
		}
		mainMenu.btnClick.Play();
		PlayerPrefs.SetInt("SelectedGunIndex", index);
		PlayerPrefs.SetInt("Gun" + index + "Bought", 1);
		CtrlYa.Instance.SaveDollars(-weapons[index].gunPrice);
		mainMenu.updateDollarsText();
		mainMenu.showSpendCurrencyParticles();
		weapons[index].selectButton.SetActive(false);
		weapons[index].selectedText.SetActive(true);
		weapons[index].buyObj.SetActive(false);
		for (int i = 0; i < weapons.Length; i++)
		{
			if (i != index && !weapons[i].buyObj.activeSelf)
			{
				weapons[i].selectButton.SetActive(true);
				weapons[i].selectedText.SetActive(false);
			}
		}
	}

	private void GetLocalizePrice()
	{
	}

	public void PurchaseGun(int index)
	{
		Debug.Log("PurchaseGun : index = " + index);
		int doll = CtrlYa.Instance.GetDollars();
		if (weaponPrice[index] <= doll)
		{
			CtrlYa.Instance.SaveDollars(-weaponPrice[index]);
			PlayerPrefs.Save();
			GunPurchased(index);
		}
		/*
		else if ((bool)IntegrationManager.Instance)
		{
			switch (index)
			{
			case 1:
				instance.GunPurchased(1);
				break;
			case 2:
				instance.GunPurchased(2);
				break;
			case 3:
				instance.GunPurchased(3);
				break;
			case 4:
				instance.GunPurchased(4);
				break;
			}
			
		}
		*/
	}

	private void OnPurchaseSucceedCallBack(string sku)
	{
		if (sku.Equals("smg_pack"))
		{
			if ((bool)instance)
			{
				instance.GunPurchased(1);
			}
		}
		else if (sku.Equals("ak47_pack"))
		{
			if ((bool)instance)
			{
				instance.GunPurchased(2);
			}
		}
		else if (sku.Equals("m4_pack"))
		{
			if ((bool)instance)
			{
				instance.GunPurchased(3);
			}
		}
		else if (sku.Equals("sniper_pack") && (bool)instance)
		{
			instance.GunPurchased(4);
		}
	}

	public void GunPurchased(int index)
	{
		mainMenu.btnClick.Play();
		PlayerPrefs.SetInt("SelectedGunIndex", index);
		PlayerPrefs.SetInt("Gun" + index + "Bought", 1);
		mainMenu.updateDollarsText();
		mainMenu.showSpendCurrencyParticles();
		weapons[index].selectButton.SetActive(false);
		weapons[index].selectedText.SetActive(true);
		weapons[index].buyObj.SetActive(false);
		for (int i = 0; i < weapons.Length; i++)
		{
			if (i != index && !weapons[i].buyObj.activeSelf)
			{
				weapons[i].selectButton.SetActive(true);
				weapons[i].selectedText.SetActive(false);
			}
		}
	}
}
