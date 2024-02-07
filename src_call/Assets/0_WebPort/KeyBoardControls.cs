using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class KeyBoardControls : MonoBehaviour
{
    public static KeyBoardControls Instance;

    public float vertical = 0F;
    public float horizontal = 0F;

    private float lerpSpeed = 10F;

    public float rawVertical = 0;
    public float rawHorizontal = 0;
    
    public bool isSomePressed = false;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        KeyForwardBack();
        KeyLeftRight();
        LerpValues();

        if (rawVertical == 0 && rawHorizontal == 0)
        {
            isSomePressed = false;
        }
        else
        {
            isSomePressed = true;
        }
    }

    


    private bool isVertical = false;

    private void KeyForwardBack()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            rawVertical = 1F;
        }
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
        {
            rawVertical = 0F;
        }
        
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            rawVertical = -1F;
        }
        if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            rawVertical = 0F;
        }
    }
    
    private void KeyLeftRight()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            rawHorizontal = -1F;
        }
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            rawHorizontal = 0F;
        }
        
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            rawHorizontal = 1F;
        }
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            rawHorizontal = 0F;
        }
    }

    private void LerpValues()
    {
        vertical = Mathf.Lerp(vertical, rawVertical, Time.deltaTime * lerpSpeed);
        horizontal = Mathf.Lerp(horizontal, rawHorizontal, Time.deltaTime * lerpSpeed);
    }
    
}