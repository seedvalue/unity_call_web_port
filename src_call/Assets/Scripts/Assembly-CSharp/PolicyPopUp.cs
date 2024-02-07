using UnityEngine;

public class PolicyPopUp : MonoBehaviour
{
	public GameObject popUpPrefab;

	public GameObject popUpParent;

	public static int PopUpEnablingCounter;

	[Tooltip("when to show next give int value e.g 1,2,3 etc")]
	public int DelayNextToShow = 3;

	private void OnEnable()
	{
		if (PlayerPrefs.GetFloat("PolicyAgreed") != 0f)
		{
			return;
		}
		if (PopUpEnablingCounter == 0)
		{
			GameObject gameObject = Object.Instantiate(popUpPrefab);
			if (popUpParent == null)
			{
				popUpParent = Object.FindObjectOfType<Canvas>().gameObject;
			}
			gameObject.GetComponent<RectTransform>().SetParent(popUpParent.transform, false);
			PopUpEnablingCounter++;
		}
		else
		{
			PopUpEnablingCounter++;
			if (PopUpEnablingCounter > DelayNextToShow)
			{
				PopUpEnablingCounter = 0;
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
