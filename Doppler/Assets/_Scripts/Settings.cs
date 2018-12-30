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
        #region Public Variables    
        public float Gain = 0.5f;
        public int BufferSize = 4100;
        public float SoundSpeed = 6f;
        public float MaxSpeed = 6f;
        public float Acceleration = 30f;
        public float Friction = 5f;
        public float SpeedCutoff = 0.001f;
        #endregion Public Variables
    }
}
