using UnityEngine;
using WaveTerrain.Audio;
using WaveTerrain.Gameplay;
using WaveTerrain.UI;

namespace WaveTerrain
{
    public class GameController : MonoBehaviour
    {
        #region Public Variables
        public bool Running = false;
        #endregion Public Variables

        #region Inspector Variables
        [SerializeField] private AudioController _AudioController;
        [SerializeField] private UIController _UI;
        [SerializeField] private FirstPersonController _FPController;
        #endregion Inspector Variables

        #region Public Methods
        public void PauseGame(bool pause)
        {
            Time.timeScale = pause ? 0f : 1f;
            _UI.Pause(pause);
            _AudioController.Pause(pause);
            _FPController.Pause(pause);
        }

        public void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        #endregion Public Methods

        #region Unity Methods
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Running) { Running = false; }
                PauseGame(true);               
            }
        }
        #endregion Unity Methods
    }
}
