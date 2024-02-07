using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ControlUIEvent : MonoBehaviour
{
	public Text moveStartText;

	public Text moveText;

	public Text moveSpeedText;

	public Text moveEndText;

	public Text touchStartText;

	public Text touchUpText;

	public Text downRightText;

	public Text downDownText;

	public Text downLeftText;

	public Text downUpText;

	public Text rightText;

	public Text downText;

	public Text leftText;

	public Text upText;

	private bool isDown;

	private bool isLeft;

	private bool isUp;

	private bool isRight;

	private void Update()
	{
		if (isDown)
		{
			downText.text = "YES";
			isDown = false;
		}
		else
		{
			downText.text = string.Empty;
		}
		if (isLeft)
		{
			leftText.text = "YES";
			isLeft = false;
		}
		else
		{
			leftText.text = string.Empty;
		}
		if (isUp)
		{
			upText.text = "YES";
			isUp = false;
		}
		else
		{
			upText.text = string.Empty;
		}
		if (isRight)
		{
			rightText.text = "YES";
			isRight = false;
		}
		else
		{
			rightText.text = string.Empty;
		}
	}

	public void MoveStart()
	{
		moveStartText.text = "YES";
		StartCoroutine(ClearText(moveStartText));
	}

	public void Move(Vector2 move)
	{
		moveText.text = move.ToString();
	}

	public void MoveSpeed(Vector2 move)
	{
		moveSpeedText.text = move.ToString();
	}

	public void MoveEnd()
	{
		if (moveEndText.enabled)
		{
			moveEndText.text = "YES";
			StartCoroutine(ClearText(moveEndText));
			StartCoroutine(ClearText(touchUpText));
			StartCoroutine(ClearText(moveText));
			StartCoroutine(ClearText(moveSpeedText));
		}
	}

	public void TouchStart()
	{
		touchStartText.text = "YES";
		StartCoroutine(ClearText(touchStartText));
	}

	public void TouchUp()
	{
		touchUpText.text = "YES";
		StartCoroutine(ClearText(touchUpText));
		StartCoroutine(ClearText(moveText));
		StartCoroutine(ClearText(moveSpeedText));
	}

	public void DownRight()
	{
		downRightText.text = "YES";
		StartCoroutine(ClearText(downRightText));
	}

	public void DownDown()
	{
		downDownText.text = "YES";
		StartCoroutine(ClearText(downDownText));
	}

	public void DownLeft()
	{
		downLeftText.text = "YES";
		StartCoroutine(ClearText(downLeftText));
	}

	public void DownUp()
	{
		downUpText.text = "YES";
		StartCoroutine(ClearText(downUpText));
	}

	public void Right()
	{
		isRight = true;
	}

	public void Down()
	{
		isDown = true;
	}

	public void Left()
	{
		isLeft = true;
	}

	public void Up()
	{
		isUp = true;
	}

	private IEnumerator ClearText(Text textToCLead)
	{
		yield return new WaitForSeconds(0.3f);
		textToCLead.text = string.Empty;
	}
}
