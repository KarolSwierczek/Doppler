using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WaveTerrain
{
    /// <summary>
    /// This class stores and updates all simulation settings
    /// </summary>
    //todo: ui controller class
    public class Settings : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private List<GameObject> _Sources;
        [SerializeField] private List<Text> _Statuses;
        [SerializeField] private Slider _BufferSizeSlider;
        [SerializeField] private AudioController _AudioController;
        [SerializeField] private GameObject _PauseMenu;
        //settings
        [SerializeField] private float _SoundSpeed;
        [SerializeField] private float _Gain = 0.5F;
        [SerializeField] private int _BufferSize = 4100;
        [SerializeField] private float _MaxSpeed;
        [SerializeField] private float _Acceleration;
        [SerializeField] private float _Friction;
        [SerializeField] private float _Epsilon;
        #endregion Inspector Variables

        #region Unity Methods
        private void Update()
        {
            if (_Sources.Count < 3)
            {
                Debug.LogError("there should be 3 sources");
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

            if (Input.GetKeyDown(KeyCode.Escape))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
                /*
                var paused = _PauseMenu.activeSelf;
                _AudioController.Pause(!paused);
                _PauseMenu.SetActive(!paused);
                */
            }

        }
        #endregion Unity Methods

        #region Private Methods
        /// <summary>
        /// toggles the source active state
        /// </summary>
        /// <param name="idx">index of the source in _Sources list</param>
        private void ToggleSource(int idx)
        {
            var active = _Sources[idx].activeSelf;
            _Sources[idx].SetActive(!active);
            SetStatus(_Statuses[idx], !active);
        }

        /// <summary>
        /// sets a source status UI to active or inactive
        /// </summary>
        private void SetStatus(Text text, bool isActive)
        {
            text.text = isActive ? "Active" : "Inactive";
            text.color = isActive ? Color.green : Color.red;
        }

        /*
        public void UpdateBufferSize()
        {
            _AudioController.SetBufferSize((int)_BufferSizeSlider.value);
        }
        */
        #endregion Private Methods
    }
}
