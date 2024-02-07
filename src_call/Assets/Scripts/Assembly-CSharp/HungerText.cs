using UnityEngine;
using UnityEngine.UI;

public class HungerText : MonoBehaviour
{
	[HideInInspector]
	public float hungerGui;

	private float oldHungerGui = -512f;

	[Tooltip("Color of GUIText.")]
	public Color textColor;

	private Text uiTextComponent;

	private void Start()
	{
		uiTextComponent = GetComponent<Text>();
		uiTextComponent.color = textColor;
		oldHungerGui = -512f;
	}

	private void Update()
	{
		if (hungerGui != oldHungerGui)
		{
			uiTextComponent.text = "Hunger : " + hungerGui;
			oldHungerGui = hungerGui;
		}
	}
}
