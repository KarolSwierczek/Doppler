using UnityEngine;
using UnityEngine.UI;

namespace WaveTerrain.UI
{
    public class SliderValueUpdate : MonoBehaviour
    {
        #region Private Variables
        private Text _Text;
        #endregion Private Variables

        #region Public Methods
        public void UpdateValue(float value)
        {
            _Text.text = value.ToString();
        }
        #endregion Public Methods

        #region Unity Methods
        private void Start()
        {
            _Text = GetComponent<Text>();
        }
        #endregion Unity Methods
    }
}
