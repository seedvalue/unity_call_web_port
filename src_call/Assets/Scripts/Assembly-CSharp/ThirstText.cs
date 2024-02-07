using UnityEngine;
using UnityEngine.UI;

public class ThirstText : MonoBehaviour
{
	[HideInInspector]
	public float thirstGui;

	private float oldThirstGui = -512f;

	[Tooltip("Color of GUIText.")]
	public Color textColor;

	private Text uiTextComponent;

	private void Start()
	{
		uiTextComponent = GetComponent<Text>();
		uiTextComponent.color = textColor;
		oldThirstGui = -512f;
	}

	private void Update()
	{
		if (thirstGui != oldThirstGui)
		{
			uiTextComponent.text = "Thirst : " + thirstGui;
			oldThirstGui = thirstGui;
		}
	}
}
