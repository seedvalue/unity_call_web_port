using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class ObjectCreator : MonoBehaviour
{
	public Transform ParentObject;

	public GameObject CellGameobject;

	public Transform ImageParentObject;

	public GameObject ImageGameobject;

	public ScrollSnap ref_ScrollSnap;

	public Image[] Small_Icon;

	public Image[] Large_Image;

	public Button[] SmallIcon_Buttons;

	public Button[] Large_Image_Buttons;

	[HideInInspector]
	public LoadAssets objectOfLoadAssest;

	public int Size;

	private int counter;

	private int Largecounter;

	private int selectedIndex;

	private void Start()
	{
		objectOfLoadAssest = Object.FindObjectOfType<LoadAssets>();
	}

	public void Init()
	{
		int num = 0;
		while (counter < Size)
		{
			InstantiateItem();
			num++;
		}
		int num2 = 0;
		while (Largecounter < Size)
		{
			Instantiate_Large_Item();
			num2++;
		}
	}

	public void InstantiateItem()
	{
		GameObject gameObject = Object.Instantiate(CellGameobject);
		gameObject.name = counter.ToString();
		gameObject.SetActive(true);
		gameObject.transform.SetParent(ParentObject, false);
		SmallIcon_Buttons[counter] = gameObject.GetComponent<Button>();
		int index = int.Parse(gameObject.name);
		gameObject.GetComponent<Button>().onClick.AddListener(delegate
		{
			selectedIndex = index;
			ref_ScrollSnap.TestScreen(index);
			OnRequired(index);
		});
		counter++;
		if (counter > 0 && counter <= Size)
		{
			Small_Icon[counter - 1] = gameObject.GetComponent<Image>();
		}
	}

	public int GetSelectedIndex()
	{
		return selectedIndex;
	}

	public void Instantiate_Large_Item()
	{
		GameObject gameObject = Object.Instantiate(ImageGameobject);
		gameObject.name = Largecounter.ToString();
		gameObject.SetActive(true);
		gameObject.transform.SetParent(ImageParentObject, false);
		Large_Image_Buttons[Largecounter] = gameObject.GetComponent<Button>();
		Largecounter++;
		if (Largecounter > 0 && Largecounter <= Size)
		{
			Large_Image[Largecounter - 1] = gameObject.GetComponent<Image>();
		}
	}

	public void SetArrayLength()
	{
		Large_Image = new Image[Size];
		Small_Icon = new Image[Size];
		Large_Image_Buttons = new Button[Size];
		SmallIcon_Buttons = new Button[Size];
	}

	public void Change_Small_Icons(Texture2D image, int index)
	{
		if (index < Small_Icon.Length)
		{
			if (image == null)
			{
				Debug.LogWarning("not be null");
			}
			Small_Icon[index].GetComponent<Image>().sprite = Sprite.Create(image, new Rect(0f, 0f, image.width, image.height), new Vector2(0f, 0f), 100f);
		}
	}

	public void Change_Large_Image(Texture2D image, int index, int panel)
	{
		if (index < Large_Image.Length)
		{
			Large_Image[index].GetComponent<Image>().sprite = Sprite.Create(image, new Rect(0f, 0f, image.width, image.height), new Vector2(0f, 0f), 100f);
			if (objectOfLoadAssest == null)
			{
				objectOfLoadAssest = Object.FindObjectOfType<LoadAssets>();
			}
			Large_Image_Buttons[index].GetComponent<Button>().onClick.RemoveAllListeners();
			Large_Image_Buttons[index].GetComponent<Button>().onClick.AddListener(delegate
			{
				objectOfLoadAssest.OnFeatureClick(panel, index);
			});
		}
	}

	private void OffAllSelectors()
	{
		for (int i = 0; i < SmallIcon_Buttons.Length; i++)
		{
			SmallIcon_Buttons[i].transform.gameObject.transform.GetChild(1).transform.gameObject.SetActive(false);
		}
	}

	public void OnRequired(int index)
	{
		OffAllSelectors();
		if ((bool)SmallIcon_Buttons[index].transform.gameObject.transform.GetChild(1).transform.gameObject)
		{
			SmallIcon_Buttons[index].transform.gameObject.transform.GetChild(1).transform.gameObject.SetActive(true);
		}
	}
}
