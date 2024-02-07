using UnityEngine;
using UnityEngine.UI;

public class EnemyLocator : MonoBehaviour
{
	public GameObject healthPanel;

	public float healthPanelOffset = 0.35f;

	private GameObject HP;

	public GameObject canvas;

	public Image Bar;

	public bool isPlayer;

	private CanvasGroup CG;

	private bool isUpdate;

	public bool isStar = true;

	public Camera View;

	private float timeToStay = 2f;

	private float timer;

	private void Start()
	{
		canvas = GameObject.FindGameObjectWithTag("MainCanvas");
		if ((bool)Camera.main)
		{
			View = Camera.main;
		}
		if ((bool)canvas)
		{
			HP = Object.Instantiate(healthPanel);
			HP.name = "EnemyHealthBar";
			HP.transform.SetParent(canvas.transform, false);
			if (Bar == null)
			{
				Bar = HP.transform.Find("Bar").GetComponent<Image>();
			}
			Bar.fillAmount = 1f;
			CG = HP.GetComponent<CanvasGroup>();
			CG.alpha = 0f;
		}
	}

	public void DestroyHealthBar()
	{
		if (HP != null)
		{
			Object.Destroy(HP);
		}
	}

	public void updateBarValue(float value)
	{
		if ((bool)Bar)
		{
			Bar.fillAmount = value;
			isUpdate = true;
			if (isStar)
			{
				CG.alpha = 0f;
				isStar = false;
			}
			else
			{
				CG.alpha = 1f;
			}
		}
	}

	public void changeMainCamera(Camera cam)
	{
		MonoBehaviour.print("camera is assigned");
		View = cam;
	}

	private void Update()
	{
		if (!(HP != null) || !(View != null))
		{
			return;
		}
		Vector3 position = new Vector3(base.transform.position.x, base.transform.position.y + healthPanelOffset, base.transform.position.z);
		Vector3 vector = View.WorldToScreenPoint(position);
		HP.transform.position = new Vector3(vector.x, vector.y, vector.z);
		if (isUpdate)
		{
			timer += Time.deltaTime;
			if (timer >= timeToStay)
			{
				timer = 0f;
				CG.alpha = 0f;
				isUpdate = false;
			}
			if (CG.alpha != 0f)
			{
				CG.alpha = Mathf.MoveTowards(CG.alpha, 0f, Time.deltaTime);
			}
		}
	}
}
