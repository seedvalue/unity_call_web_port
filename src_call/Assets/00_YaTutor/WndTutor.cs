using System;
using UnityEngine;
using UnityEngine.UI;

namespace _00_YaTutor
{
    public class WndTutor : MonoBehaviour
    {
        public Button buttonOk;


        private void OnClickOk()
        {
            Debug.Log("WndTutor : OnClickOk");
            gameObject.SetActive(false);
        }
        
        private void Awake()
        {
            buttonOk.onClick.AddListener(OnClickOk);
        }
    }
}
