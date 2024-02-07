using System;
using UnityEngine;

namespace _00_YaTutor
{
    public class YaSetEnabledByDevice : MonoBehaviour
    {
        [SerializeField] private CtrlYa.YaDevice deviceToEnabled;
        
        private void RefreshNeedEnabled()
        {
            if (CtrlYa.Instance)
            {
                var device = CtrlYa.Instance.GetDevice();
                if(device != deviceToEnabled) gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("YaSetEnabledByDevice : CtrlYa.Instance == null");
            }
        }
        
        private void OnEnable()
        {
            RefreshNeedEnabled();
        }
    }
}
