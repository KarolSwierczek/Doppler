using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WaveTerrain.UI
{
    /// <summary>
    /// this class controlls the ui and transfers data from ui to the settings
    /// </summary>
    public class UIController : MonoBehaviour
    {
        #region Inspector Variables
        //source controll
        [SerializeField] private GameObject       _SourceSelection;
        [SerializeField] private List<GameObject> _Sources;
        [SerializeField] private List<Text>       _Statuses;

        //settings 
        [SerializeField] private Settings         _Settings;
        [SerializeField] private GameObject       _PauseMenu;

        //settings sliders
        [SerializeField] private Slider           _GainSlider;
        [SerializeField] private Slider           _BufferSizeSlider;
        [SerializeField] private Slider           _SoundSpeedSlider;
        [SerializeField] private Slider           _MaxSpeedSlider;
        [SerializeField] private Slider           _AccelerationSlider;
        [SerializeField] private Slider           _FrictionSlider;
        [SerializeField] private Slider           _SpeedCutoffSlider;
        #endregion Inspectoir Variables

        #region Public Types
        public enum Setting
        {
            Gain,
            BufferSize,
            SoundSpeed,
            MaxSpeed,
            Acceleration,
            Friction,
            SpeedCutoff,
        }
        #endregion Public Types

        #region Public Methods
        public void Pause(bool pause)
        {
            _PauseMenu.SetActive(pause);
            _SourceSelection.SetActive(!pause);
        }

        public void UpdateSettingFromSlider(string settingName)
        {
            Setting setting = (Setting)Enum.Parse(typeof(Setting), settingName, true);           
            float value;

            switch (setting)
            {                
                case Setting.Gain:
                    value = Mathf.Floor(_GainSlider.value * 100)/100;
                    _Settings.Gain = value;
                    _GainSlider.GetComponentInChildren<SliderValueUpdate>().UpdateValue(value);
                    break;
                case Setting.BufferSize:
                    value = _BufferSizeSlider.value;
                    _Settings.BufferSize = (int)value;
                    _BufferSizeSlider.GetComponentInChildren<SliderValueUpdate>().UpdateValue(value);
                    break;
                case Setting.SoundSpeed:
                    value = Mathf.Floor(_SoundSpeedSlider.value*10)/10;
                    _Settings.SoundSpeed = value;
                    _SoundSpeedSlider.GetComponentInChildren<SliderValueUpdate>().UpdateValue(value);
                    break;
                case Setting.MaxSpeed:
                    value = Mathf.Floor(_MaxSpeedSlider.value*10)/10;
                    _Settings.MaxSpeed = value;
                    _MaxSpeedSlider.GetComponentInChildren<SliderValueUpdate>().UpdateValue(value);
                    break;
                case Setting.Acceleration:
                    value = Mathf.Floor(_AccelerationSlider.value*10)/10;
                    _Settings.Acceleration = value;
                    _AccelerationSlider.GetComponentInChildren<SliderValueUpdate>().UpdateValue(value);
                    break;
                case Setting.Friction:
                    value = Mathf.Floor(_FrictionSlider.value*10)/10;
                    _Settings.Friction = value;
                    _FrictionSlider.GetComponentInChildren<SliderValueUpdate>().UpdateValue(value);
                    break;
                case Setting.SpeedCutoff:
                    value = Mathf.Floor(Mathf.Pow(10, _SpeedCutoffSlider.value)*10000)/10000;
                    _Settings.SpeedCutoff = value;
                    _SpeedCutoffSlider.GetComponentInChildren<SliderValueUpdate>().UpdateValue(value);
                    break;
            }
        }
        #endregion Public Methods

        #region Unity Methods
        void Update()
        {
            if (_Sources.Count < 7)
            {
                Debug.LogError("there should be 7 sources");
                return;
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ToggleSource(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ToggleSource(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ToggleSource(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                ToggleSource(3);
                ToggleSource(4, false);
                ToggleSource(5, false);
                ToggleSource(6, false);
            }
        }
        #endregion Unity Methods

        #region Pricvate Methods
        /// <summary>
        /// toggles the source active state
        /// </summary>
        /// <param name="idx">index of the source in _Sources list</param>
        private void ToggleSource(int idx, bool setStatus = true)
        {
            var active = _Sources[idx].activeSelf;
            _Sources[idx].SetActive(!active);
            if (setStatus) { SetStatus(_Statuses[idx], !active); }
        }

        /// <summary>
        /// sets a source status UI to active or inactive
        /// </summary>
        private void SetStatus(Text text, bool isActive)
        {
            text.text = isActive ? "Active" : "Inactive";
            text.color = isActive ? Color.green : Color.red;
        }
        #endregion Pricvate Methods
    }
}
