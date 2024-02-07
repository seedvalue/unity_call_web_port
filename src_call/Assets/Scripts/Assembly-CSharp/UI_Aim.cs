using System;
using _0_WebPort;
using _00_YaTutor;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

public class UI_Aim : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IEventSystemHandler
{
	public SmoothMouseLook PlayerAim;

	public bool canMouseLook;

	private PointerEventData myData;

	public Vector2 deltaOfPointer = Vector2.zero;

	

	public void OnDrag(PointerEventData Data)
	{
		myData = Data;
	}

	public void OnBeginDrag(PointerEventData Data)
	{
		canMouseLook = true;
		myData = Data;
	}

	public void OnEndDrag(PointerEventData Data)
	{
		myData = Data;
		canMouseLook = false;
	}

	private void Awake()
	{
		PlayerAim = Object.FindObjectOfType<SmoothMouseLook>();
		Debug.Log("в апдейте кен майс лоок");
	}

	private void Start()
	{
		if (CtrlYa.Instance )
		{
			if (CtrlYa.Instance.GetDevice() == CtrlYa.YaDevice.PC)
			{
				// лок курсора
				CtrlYa.Instance.SetCursorLocked(true);
			}
			else
			{
				// анлок курсора
				CtrlYa.Instance.SetCursorLocked(false);
			}
		}
	}

	
	// c
	private void Update()
	{
		if (CtrlYa.Instance )
		{
			if (CtrlYa.Instance.GetDevice() == CtrlYa.YaDevice.PC)
			{
				//Это дает возможность без клика
				canMouseLook = true;
			}
		}
		else
		{
			Debug.LogError("PlayerAim : CtrlYa.Instance  == NULL");
		}
		
		if ((bool)PlayerAim)
		{
			PlayerAim.allowAim(canMouseLook);
			if (myData != null)
			{
				deltaOfPointer = myData.delta;
				PlayerAim.setMouseData(deltaOfPointer);
			}
		}
	}
}
