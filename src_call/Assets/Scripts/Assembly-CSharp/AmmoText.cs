using UnityEngine;
using UnityEngine.UI;

public class AmmoText : MonoBehaviour
{
	[HideInInspector]
	public int ammoGui;

	[HideInInspector]
	public int ammoGui2;

	[HideInInspector]
	public bool showMags = true;

	private int oldAmmo = -512;

	private int oldAmmo2 = -512;

	[Tooltip("Color of GUIText.")]
	public Color textColor;

	[HideInInspector]
	public Text uiTextComponent;

	private void OnEnable()
	{
		uiTextComponent = GetComponent<Text>();
		oldAmmo = -512;
		oldAmmo2 = -512;
	}

	private void Update()
	{
		if (ammoGui != oldAmmo || ammoGui2 != oldAmmo2)
		{
			if (showMags)
			{
				uiTextComponent.text = "Ammo : " + ammoGui + " / " + ammoGui2;
			}
			else
			{
				uiTextComponent.text = "Ammo : " + ammoGui2;
			}
			oldAmmo = ammoGui;
			oldAmmo2 = ammoGui2;
		}
	}
}
