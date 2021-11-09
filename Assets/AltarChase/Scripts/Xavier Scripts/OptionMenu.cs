using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace AltarChase.Scripts.Xavier_Scripts
{
    public class OptionMenu : MonoBehaviour
    {
        [Header("Audio Stuff")]
        public AudioMixer audioMixer;
    
        [Header("Resolution Set")]
        public TMP_Dropdown resolutionDropdown;
        Resolution[] resolutions;
        public Toggle fullscreenToggle;
    
        [Header("Quality Set")]
        public TMP_Dropdown qualityDropdown;
    
        [Header("Texture Set")]
        public TMP_Dropdown textureDropdown;

        [Header("Anti-Aliasing Set")] 
        public TMP_Dropdown aaDropdown;

        [Header("Volume Set")]
        public Slider masterVolume;
        public Slider sfxVolumeSlider;
        float currentVolume;

        [SerializeField] private GameObject pausemenuUI;
        public static bool isPaused = false;
        public static bool loadData = false;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isPaused = true;
                if (isPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }
        
        public void Resume()
        {
            pausemenuUI.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
        }

        void Pause()
        {
            pausemenuUI.SetActive(true);
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            isPaused = true;
        }

        public void ResolutionAwake()
        {
            #region Resolution Start
            resolutionDropdown.ClearOptions();
            List<string> resOptions = new List<string>();
            resolutions = Screen.resolutions;
            int currentResolutionIndex = 0;

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " +
                                resolutions[i].height;
                resOptions.Add(option);
                if (resolutions[i].width == Screen.currentResolution.width
                    && resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i; 
                }
            }

            resolutionDropdown.AddOptions(resOptions);
            if (PlayerPrefs.HasKey("Resolution"))
            {
                int resIndex = PlayerPrefs.GetInt("Resolution");
                resolutionDropdown.value = resIndex;
                resolutionDropdown.RefreshShownValue();
                SetResolution(resIndex);
            }
            else
            {
                resolutionDropdown.value = currentResolutionIndex;
                resolutionDropdown.RefreshShownValue();
            }
            #endregion
        }
        

        private void Start()
        {
#if UNITY_STANDALONE
            ResolutionAwake();

            LoadSettings();
#endif
        }

        #region Volume Stuff

        public void SetMasterVolume(float volume)
        {
            PlayerPrefs.SetFloat("MasterVolume", volume);
            volume = VolumeRemapping(volume);
            audioMixer.SetFloat("masterVolume",volume);
        }

        public void SFXVolume(float volume)
        {
            PlayerPrefs.SetFloat("sfxVolume",volume);
            volume = VolumeRemapping(volume);
            audioMixer.SetFloat("sfxVolume", volume);
        }

        private float VolumeRemapping(float _value)
        {
            return -40 + (_value - 0) * (20 - -40) / (1 - 0);
        }

        #endregion
   

        public void SetFullScreen(bool isFullscreen)
        {
            PlayerPrefs.SetInt("Fullscreen",(isFullscreen ? 1 : 0));
            Screen.fullScreen = isFullscreen;
        }

        public void SetResolution(int resolutionIndex)
        {
            PlayerPrefs.SetInt("Resolution", resolutionIndex);
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        public void SetTextureQuality(int textureIndex)
        {
            QualitySettings.masterTextureLimit = textureIndex;
            qualityDropdown.value = 6;
            PlayerPrefs.SetInt("TextureQuality", textureIndex);
        }
        
        /// <summary>
        /// Method that will find all the AA settings and place it in the Dropdown!
        /// </summary>
        /// <param name="aaIndex"> How many AA settings </param>
        public void SetAntiAliasing(int aaIndex)
        {
            QualitySettings.antiAliasing = aaIndex;
            qualityDropdown.value = 6;
        }
        
        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
            PlayerPrefs.SetInt("Quality", qualityIndex);
        }

        private void LoadSettings()
        {
            if (PlayerPrefs.HasKey("Resolution"))
            {
                int resIndex = PlayerPrefs.GetInt("Resolution");
                resolutionDropdown.value = resIndex;
                resolutionDropdown.RefreshShownValue();
                SetResolution(resIndex);

            }
        
            if (PlayerPrefs.HasKey("Quality"))
            {
                int quality = PlayerPrefs.GetInt("Quality");
                qualityDropdown.value = quality;
                SetTextureQuality(quality);
            }

            if (PlayerPrefs.HasKey("TextureQuality"))
            {
                int textureQuality = PlayerPrefs.GetInt("TextureQuality");
                textureDropdown.value = textureQuality;
                SetTextureQuality(textureQuality);
            }

            if (PlayerPrefs.HasKey("MasterVolume"))
            {
                float volume = PlayerPrefs.GetFloat("MasterVolume");
                masterVolume.value = volume;
                SetMasterVolume(volume);
            }

            if (PlayerPrefs.HasKey("SFXVolume"))
            {
                float sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
                sfxVolumeSlider.value = sfxVolume;
                SFXVolume(sfxVolume);
            }

            if (PlayerPrefs.HasKey("Fullscreen"))
            {
                bool _fullscreen = true;
                int fullscreen = PlayerPrefs.GetInt("Fullscreen");
                if (fullscreen == 1)
                {
                    fullscreenToggle.isOn = true;
                }
                else if (fullscreen == 0)
                {
                    _fullscreen = false;
                    fullscreenToggle.isOn = false;
                }

                SetFullScreen(_fullscreen);

            }
        }
    }
}