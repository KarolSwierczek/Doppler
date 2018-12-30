using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WaveTerrain
{
    public class Settings : MonoBehaviour
    {

        [SerializeField] private List<GameObject> _Sources;
        [SerializeField] private List<Text> _Statuses;
        [SerializeField] private Slider _BufferSizeSlider;
        [SerializeField] private AudioController _AudioController;
        [SerializeField] private GameObject _PauseMenu;

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

        private void ToggleSource(int idx)
        {
            var active = _Sources[idx].activeSelf;
            _Sources[idx].SetActive(!active);
            SetStatus(_Statuses[idx], !active);
        }

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
    }
}
