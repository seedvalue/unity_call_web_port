using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

namespace _00_YaTutor
{
    public class CtrlYa : MonoBehaviour
    {
        [SerializeField] private GameObject wndTutor;
        [SerializeField] private GameObject wndLoading;
        [SerializeField] private Slider sliderLoading;

        public static CtrlYa Instance;
        public YaDevice device;


        #region Saves
        
        public void GetLoad()
        {
           // var text = YandexGame.savesData.money.ToString();
           
           Debug.LogError("GetLoad : CHECK!");
           //Update UI levels
           //CtrlUi.Instance.OnSavingSGetedFromSdk();
        }


        public void MySave(int money)
        {
            //YandexGame.savesData.money = money;
            //YandexGame.SaveProgress();
        }

        public void UnlockLevelAndSave(int num)
        {
            Debug.Log("CtrlYa : UnlockLevelAndSave : num = " + num);
            var levels = YandexGame.savesData.unlockedLevels;
            Debug.LogError("CHECK : OnEnableWndHideShowCursor UnlockLevelAndSave");
            /*
            for (int i = 0; i < levels.Length; i++)
            {
                if (i <= num) levels[i] = true;
            }
            */
            YandexGame.savesData.unlockedLevels = levels;
            YandexGame.SaveProgress();
        }

        public void SetLevelStarsAndSave(int level, int stars)
        {
            Debug.Log("CtrlYa : UnlockLevelAndSave : level = " + level + " stars = " + stars);
            //YandexGame.savesData.levelStars2[level] = stars;
            YandexGame.SaveProgress();
        }

        public int GetLastUnlockedLevel()
        {
            var levels = YandexGame.savesData.unlockedLevels;
            // invert for --
            Debug.LogError("YA GetLastUnlockedLevel");
            /*
            for (int i = levels.Length-1; i >= 0; i--)
            {
                if (levels[i] == true) return i;
            }*/
            return 0;
        }

        public int GetStarsByLevel(int levelNum)
        {
            Debug.LogError("YA GetLastUnlockedLevel");
            /*
            var length = YandexGame.savesData.levelStars2.Length;
            Debug.Log("CtrlYa : GetStarsByLevel : levelNum = " + levelNum + " length = " + length);
           // if (length <= 0) return 0;
            return YandexGame.savesData.levelStars2[levelNum];
            */
            return 0;
        }
        
        
        
        // Start()
        private void SaveGetLoad()
        {
            // Проверяем запустился ли плагин
            if (YandexGame.SDKEnabled == true)
            {
                // Если запустился, то выполняем Ваш метод для загрузки
                GetLoad();

                // Если плагин еще не прогрузился, то метод не выполнится в методе Start,
                // но он запустится при вызове события GetDataEvent, после прогрузки плагина
            }
        }
        
        
        private void SubscribeGetSavedData()
        {
            YandexGame.GetDataEvent += GetLoad;
        }
        
        private void UnSubscribeGetSavedData()
        {
            YandexGame.GetDataEvent -= GetLoad;
        }
        
        

        #endregion


        #region Reward

        [SerializeField] private GameObject wndRewardAsk;
        [SerializeField] private Button buttonRewardAskYes;
        [SerializeField] private Button buttonRewardAskNo;

        private Action _onRewardSuccess;

        public void OnClickUiShowRewardAsk(Action onActionOk, bool isSkipAsk)
        {
            Debug.Log("CtrlYa : OnClickUiShowRewardAsk");
            if (isSkipAsk)
            {
                OnClickRewardAskYes();
            }
            else
            {
                if(wndRewardAsk) wndRewardAsk.SetActive(true);
                else Debug.LogError("CtrlYa : OnClickUiShowRewardAsk : wndRewardAsk == NULL");
            }
            _onRewardSuccess = onActionOk;
        }
        
        private void OnClickRewardAskYes()
        {
            Debug.Log("CtrlYa : OnClickRewardAskYes");
            YandexGame.RewVideoShow(0);
            if(wndRewardAsk)wndRewardAsk.SetActive(false);
        }
        

        private void OnClickRewardAskNo()
        {
            Debug.Log("CtrlYa : OnClickRewardAskNo");
            wndRewardAsk.SetActive(false);
        }

        void Rewarded(int id)
        {
            _onRewardSuccess?.Invoke();
        }

        private void SetupButtonsRewardAsk()
        {
            if(buttonRewardAskYes)buttonRewardAskYes.onClick.AddListener(OnClickRewardAskYes);
            else Debug.LogError("null button");
           
            if(buttonRewardAskNo)buttonRewardAskNo.onClick.AddListener(OnClickRewardAskNo);
            else Debug.LogError("null button");
        }

        // OnEnable
        private void SubscribeReward()
        {
            YandexGame.RewardVideoEvent += Rewarded;
        }

        // OnDisable
        private void UnSubscribeReward()
        {
            YandexGame.RewardVideoEvent -= Rewarded;
        }

        #endregion

        #region Cursor

        public void SetCursorLocked(bool isLocked)
        {
            if (isLocked)
            {
                if (GetDevice() == YaDevice.Mobile)
                {
                    Debug.Log("CtrlYa : SetCursorLocked : mobile device, ignoring lock");
                    return;
                }
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            
            // Ограничение внутри экрана
            /*
              //Press this button to confine the Cursor within the screen
        if (GUI.Button(new Rect(125, 0, 100, 50), "Confine Cursor"))
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
             */
        }

        #endregion
        

        public YaDevice GetDevice()
        {
#if UNITY_EDITOR
            return device;
            //   Debug.LogError("OVVERIDE DEVICE FOR TEST EDITOR " + device);
#endif
            
            if(YandexGame.EnvironmentData.isDesktop)device = YaDevice.PC;
            if(YandexGame.EnvironmentData.isMobile)device = YaDevice.Mobile;
            

            
            Debug.Log("CtrlYa : GetDevice : device = " + device);
            return device;
        }
        
        // Писали, что яндекс не корректно дает эту инфу
        public bool IsRewardReady()
        {
            Debug.LogError("CtrlYa : IsRewardReady");
            return true;
        }
        
        public void OnShowUiStartApp()
        {
            Debug.LogError("CtrlYa : OnShowUiStartApp");
        }

        public void OnShowUiLevelFinish()
        {
            Debug.LogError("CtrlYa : OnShowUiLevelFinish");
        }

        public void OnShowUiLevelSelection()
        {
            Debug.LogError("CtrlYa : OnShowUiLevelSelection");
        }

        public void OnShowUiLevelFail()
        {
            Debug.LogError("CtrlYa : OnShowUiLevelFail");
        }

        public void OnShowUiGamePlayDefault()
        {
            Debug.LogError("CtrlYa : OnShowUiGamePlayDefault");
        }

        public void OnShowUiGamePause()
        {
            Debug.LogError("CtrlYa : OnShowUiGamePause");
        }

        public void OnClickUiLevelRestart()
        {
            Debug.LogError("CtrlYa : OnClickUiLevelRestart");
        }

        public void OnClickUiLevelNext()
        {
            Debug.LogError("CtrlYa : OnClickUiLevelNext");
        }


        private bool SetupInstance()
        {
            if (CtrlYa.Instance == null)
            {
                Instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
                return true;
            }
            else
            {
                Destroy(gameObject);
                return false;
            }
            return false;
        }

        private void OnEnable()
        {
            SubscribeReward();
            SubscribeGetSavedData();
        }

        private void OnDisable()
        {
            UnSubscribeReward();
            UnSubscribeGetSavedData();
        }

        private void Awake()
        {
            if(wndTutor)wndTutor.gameObject.SetActive(false);
            else Debug.LogError("CtrlYa : wndTutor == null");
            if(wndRewardAsk)wndRewardAsk.SetActive(false);
            else Debug.LogError("CtrlYa : wndRewardAsk == null");
            if(wndLoading)wndLoading.SetActive(false);
            else Debug.LogError("CtrlYa : wndLoading == null");
            bool isOk = SetupInstance();
            if(isOk)SetupButtonsRewardAsk();
          //  _audioListener = FindObjectOfType<AudioListener>();
        }

      // private AudioListener _audioListener;
        
        //Когда нажат след уровень и пора грузить
        //Поставим плашу загрузка и вырубим звук
        public void OnSceneBeginLoad()
        {
            Debug.Log("CtrlYa : OnSceneLoaded");
            ShowLoadingWnd(true);
            Debug.LogError("CtrlYa : OnSceneLoaded. CHECK!");
            //SoundCtrl.Inst.MuteAllAudio();
        }
        
        private void OnSceneLoaded(Scene scene,LoadSceneMode loadSceneMode)
        {
            Debug.Log("CtrlYa : OnSceneLoaded");
            //ShowLoadingWnd(false);
            //SoundCtrl.Inst.UnMuteAllAudio();
            StartCoroutine(DelayedSliderLoadingCo(2F));
        }

        
        //Ждем немного, звук не ломается, но реклама моргает при виде геймплея.
        private IEnumerator DelayedSliderLoadingCo(float time)
        {
            if (sliderLoading == null)
            {
                Debug.LogError("sliderLoading == NULL");
                yield break;
            }
            sliderLoading.value = 0.5F;
            yield return new WaitForSeconds(0.5F);
            sliderLoading.value = 0.6F;
            yield return new WaitForSeconds(0.5F);
            sliderLoading.value = 0.7F;
            yield return new WaitForSeconds(0.4F);
            sliderLoading.value = 0.8F;
            yield return new WaitForSeconds(0.3F);
            sliderLoading.value =  0.9F;
            yield return new WaitForSeconds(0.2F);
            sliderLoading.value =  1F;
            yield return new WaitForSeconds(0.1F);
            ShowLoadingWnd(false);
            Debug.LogError("UNMUTTED SOUNDS");
            //SoundCtrl.Inst.UnMuteAllAudio();
        }

        private void ShowLoadingWnd(bool isShow)
        {
            wndLoading.SetActive(isShow);
        }
        

        private void Start()
        {
            SaveGetLoad();
            if(wndTutor) wndTutor.gameObject.SetActive(true);
            else Debug.LogError("CtrlYa : wndTutor == NULL"); 
            
            
            // Подписка тут, что б не тргать текущую главную
            SceneManager.sceneLoaded += OnSceneLoaded;
            
        }

        public enum YaDevice
        {
            Mobile,
            PC
        }
    }
}