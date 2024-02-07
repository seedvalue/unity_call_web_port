using System.Collections.Generic;
//using GamePush;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _0_WebPort.Tutorial
{
    public class CtrlTutorial : MonoBehaviour
    {
        [SerializeField] private Button buttonSkipAll;
        [SerializeField] List<GameObject>  pcButtons;
        [SerializeField] List<GameObject> mobileButtons;
        
        private void Awake()
        {
            buttonSkipAll.onClick.AddListener(OnClickSkipTutorial);
        }
        
        private void Start()
        {
           Debug.LogError("CtrlTutorial : Start : GAME PUSH sdk. Нам надо яндекс!");
            // SetMobile(GP_Device.IsMobile());
        }

        private void SetMobile(bool isMobile)
        {
            Debug.Log("CtrlTutorial : SetMobile : isMobile = " + isMobile);
            foreach (var one in pcButtons)
            {
                one.SetActive(!isMobile);
            }
            foreach (var one in mobileButtons)
            {
                one.SetActive(isMobile);
            }
        }

        private void LoadGamePlay()
        {
            Debug.Log("CtrlTutorial : LoadGamePlay");
            //Cur scene 0, next scene gameplay
            SceneManager.LoadScene(1);
        }
        
        private void OnClickSkipTutorial()
        {
            Debug.Log("CtrlTutorial : OnClickSkipTutorial");
            LoadGamePlay();
        }
    }
}
