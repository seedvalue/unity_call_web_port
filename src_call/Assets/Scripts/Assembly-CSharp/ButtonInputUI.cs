using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInputUI : MonoBehaviour
{
	public Text getButtonDownText;

	public Text getButtonText;

	public Text getButtonTimeText;

	public Text getButtonUpText;

	private void Update()
	{
		if (ETCInput.GetButton("Button"))
		{
			getButtonText.text = "YES";
			getButtonTimeText.text = ETCInput.GetButtonValue("Button").ToString();
		}
		else
		{
			getButtonText.text = string.Empty;
			getButtonTimeText.text = string.Empty;
		}
		if (ETCInput.GetButtonDown("Button"))
		{
			getButtonDownText.text = "YES";
			StartCoroutine(ClearText(getButtonDownText));
		}
		if (ETCInput.GetButtonUp("Button"))
		{
			getButtonUpText.text = "YES";
			StartCoroutine(ClearText(getButtonUpText));
		}
	}

	private IEnumerator ClearText(Text textToCLead)
	{
		yield return new WaitForSeconds(0.3f);
		textToCLead.text = string.Empty;
	}

	public void SetSwipeIn(bool value)
	{
		ETCInput.SetControlSwipeIn("Button", value);
	}

	public void SetSwipeOut(bool value)
	{
		ETCInput.SetControlSwipeOut("Button", value);
	}

	public void setTimePush(bool value)
	{
		ETCInput.SetAxisOverTime("Button", value);
	}
}
