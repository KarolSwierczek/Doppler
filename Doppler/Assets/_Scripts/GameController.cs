using UnityEngine;

namespace WaveTerrain
{
    public class GameController : MonoBehaviour
    {

        void Update()
        {
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
    }
}
