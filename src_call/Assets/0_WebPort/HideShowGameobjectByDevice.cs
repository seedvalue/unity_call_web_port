using System;
using _00_YaTutor;
using UnityEngine;

namespace _0_WebPort
{
    public class HideShowGameobjectByDevice : MonoBehaviour
    {
         public CtrlYa.YaDevice showByDeviceType;
        
        [Header("Just set scale zero")]
        public bool fakeHide;
        private void Start()
        {
            RefreshHidedStated();
        }


        private void RefreshHidedStated()
        {
            if (CtrlYa.Instance)
            {
                if (CtrlYa.Instance.GetDevice() == showByDeviceType) return;
                if (fakeHide) transform.localScale = Vector3.zero;
                else gameObject.SetActive(false);
            }
            else Debug.LogError("OnEnableWndHideShowCursor : CursorLock : CtrlGamePush.Instance == NULL");
            /*
            if (CtrlGamePush.Instance)
            {
                if (CtrlGamePush.Instance.GetDevice() == showByDeviceType) return;
                if (fakeHide) transform.localScale = Vector3.zero;
                else gameObject.SetActive(false);
            }
                        else Debug.LogError("OnEnableWndHideShowCursor : CursorLock : CtrlGamePush.Instance == NULL");
        */
        }
    }
}
