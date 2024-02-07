using UnityEngine;
using UnityEngine.UI;

public class HelpText : MonoBehaviour
{
	[Tooltip("Color of GUIText.")]
	public Color textColor;

	private bool helpTextState = true;

	private bool helpTextEnabled;

	private float startTime;

	private bool initialHide = true;

	private bool moveState = true;

	private bool F1pressed;

	private bool fadeState;

	private float moveTime;

	private float fadeTime = 5f;

	[HideInInspector]
	public GameObject playerObj;

	private Text uiTextComponent;

	private void Start()
	{
		uiTextComponent = GetComponent<Text>();
		playerObj = Camera.main.transform.GetComponent<CameraControl>().playerObj;
		uiTextComponent.enabled = true;
		uiTextComponent.text = "Press F1 for controls";
		uiTextComponent.color = textColor;
		uiTextComponent.enabled = true;
		startTime = Time.time;
	}

	private void Update()
	{
		FPSRigidBodyWalker component = playerObj.GetComponent<FPSRigidBodyWalker>();
		InputControl component2 = playerObj.GetComponent<InputControl>();
		float inputX = component.inputX;
		float inputY = component.inputY;
		Color color = uiTextComponent.color;
		if (moveState && (Mathf.Abs(inputX) > 0.75f || Mathf.Abs(inputY) > 0.75f))
		{
			moveState = false;
			if (startTime + fadeTime < Time.time)
			{
				moveTime = Time.time;
			}
			else
			{
				moveTime = startTime + fadeTime;
			}
		}
		if (component2.helpPress && (moveState || moveTime > Time.time))
		{
			moveState = false;
			F1pressed = true;
			moveTime = Time.time;
		}
		if (!fadeState && !F1pressed)
		{
			if (!moveState && startTime + fadeTime < Time.time)
			{
				if (moveTime + 1f > Time.time)
				{
					color.a -= Time.deltaTime;
					uiTextComponent.color = color;
				}
				else
				{
					fadeState = true;
				}
			}
			return;
		}
		if (initialHide)
		{
			uiTextComponent.text = "Mouse 1 : fire weapon\nMouse 2 : raise sights or block\nMouse 3 or C : toggle camera mode, hold in third person to zoom and rotate when moving\nW : forward\nS : backward\nA : strafe left\nD : strafe right\nLeft Shift : sprint\nLeft Ctrl : crouch\nX : prone\nQ : lean left\nE : lean right\nSpace : jump\nF : use item, move item, move NPC\nR : reload\nB : fire mode\nH : holster weapon\nBackspace : drop weapon\nG : throw grenade\nBackslash : select grenade\nV : melee attack\nL : toggle flashlight (if weapon has one)\nT : enter bullet time\nZ : toggle deadzone aiming\nEsc or F2: Main Menu\nTab : Pause\n";
			uiTextComponent.enabled = false;
			color.a = 1f;
			uiTextComponent.color = color;
			initialHide = false;
		}
		if (component2.helpPress)
		{
			if (helpTextState)
			{
				if (!helpTextEnabled)
				{
					uiTextComponent.enabled = true;
					helpTextEnabled = true;
				}
				else
				{
					uiTextComponent.enabled = false;
					helpTextEnabled = false;
				}
				helpTextState = false;
			}
		}
		else
		{
			helpTextState = true;
		}
	}
}
