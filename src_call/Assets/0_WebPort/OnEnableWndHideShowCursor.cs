using _00_YaTutor;
using UnityEngine;

namespace _0_WebPort
{
    public class OnEnableWndHideShowCursor : MonoBehaviour
    {
        private void OnEnable()
        {
            CursorLock(false);
        }

        private void OnDisable()
        {
            CursorLock(true);
        }

        private void CursorLock(bool isLock)
        {
            if (CtrlYa.Instance)
            {
                CtrlYa.Instance.SetCursorLocked(isLock);
            }
            else
            {
                Debug.LogError("OnEnableWndHideShowCursor : CursorLock : CtrlYa.Instance == NULL, will UNLOCK forever");
                SetCursorUnlockedForced();
            }
        }
        
        private void SetCursorUnlockedForced()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
