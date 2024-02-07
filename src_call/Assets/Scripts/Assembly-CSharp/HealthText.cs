using UnityEngine;
using UnityEngine.UI;

public class HealthText : MonoBehaviour
{
	[HideInInspector]
	public float healthGui;

	private float oldHealthGui = -512f;

	[Tooltip("Color of GUIText.")]
	public Color textColor;

	[Tooltip("True if negative HP should be shown, otherwise, clamp at zero.")]
	public bool showNegativeHP = true;

	private Text guiTextComponent;

	private void Start()
	{
		guiTextComponent = GetComponent<Text>();
		guiTextComponent.color = textColor;
		oldHealthGui = -512f;
	}

	private void Update()
	{
		if (healthGui != oldHealthGui)
		{
			if (healthGui < 0f && !showNegativeHP)
			{
				guiTextComponent.text = "Health : 0";
			}
			else
			{
				guiTextComponent.text = "Health : " + healthGui;
			}
			oldHealthGui = healthGui;
		}
	}
}
