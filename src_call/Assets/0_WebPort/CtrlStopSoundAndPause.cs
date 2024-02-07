using UnityEngine;

namespace _0_WebPort
{
    public class CtrlStopSoundAndPause : MonoBehaviour
    {
        public static CtrlStopSoundAndPause Instance;
        private bool _isAdsShowingNow;
        private bool _isAppFocusedNow;
        
        public void SetAdsShowed()
        {
            Debug.Log("SetAdsShowed");
            _isAdsShowingNow = true;
            RefreshPauseAndSoundState();
        }
        
        public void SetAdsClosed()
        {
            Debug.Log("SetAdsClosed");
            _isAdsShowingNow = false;
            RefreshPauseAndSoundState();
        }

        private void OffAll(bool isOff)
        {
            if (isOff)
            {
                Time.timeScale = 0F;
                AudioListener.volume = 0F;
            }
            else
            {
                Time.timeScale = 1F;
                AudioListener.volume = 1F;
            }
        }

        private void RefreshPauseAndSoundState()
        {
            if (_isAdsShowingNow || _isAppFocusedNow == false)
            {
                // показывается реклама или потерян фокус
                OffAll(true);
            }
            else
            {
                // не показывается реклама и есть фокус
                if (_isAdsShowingNow == false && _isAppFocusedNow)
                {
                    OffAll(false);
                }
            }
            
            // не показывается реклама и есть фокус
            if (_isAdsShowingNow == false && _isAppFocusedNow)
            {
                OffAll(false);
            }
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            Debug.Log("OnApplicationFocus : hasFocus = " + hasFocus);
            _isAppFocusedNow = hasFocus;
            RefreshPauseAndSoundState();
        }

        
        private void OnEnable()
        {
            //GP_Game.OnPause += Pause;
            //GP_Game.OnResume += Resume;
            Debug.Log("CtrlStopSoundAndPause : OnEnable : /GP_Game.OnPause ");
        }

        private void Pause()
        {
            Debug.Log("is Pause");
            _isAppFocusedNow = false;
            RefreshPauseAndSoundState();
        }
        
        private void Resume()
        {
            Debug.Log("is resume");
            _isAppFocusedNow = true;
            RefreshPauseAndSoundState();
        }
        
        private void Awake()
        {
            if (CtrlStopSoundAndPause.Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
    }
}
