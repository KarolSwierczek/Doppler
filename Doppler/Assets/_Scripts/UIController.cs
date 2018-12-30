using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WaveTerrain
{
    /// <summary>
    /// this class controlls the ui and transfers data from ui to the settings
    /// </summary>
    public class UIController : MonoBehaviour
    {
        [SerializeField] private Settings _Settings;
        [SerializeField] private GameObject _PauseMenu;
        [SerializeField] private List<Text> _Statuses;
        [SerializeField] private Slider _BufferSizeSlider;
        [SerializeField] private List<GameObject> _Sources;


        void Start()
        {

        }

        void Update()
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
        }

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

        
        public void UpdateBufferSize()
        {
            _Settings.BufferSize = (int)_BufferSizeSlider.value;
        }

    }
}
