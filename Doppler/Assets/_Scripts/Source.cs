using UnityEngine;

namespace WaveTerrain
{
    /// <summary>
    /// this class represents a sound source 
    /// and is responcible for reading samples from its assigned audio clip
    /// </summary>
    public class Source : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private AudioClip _Clip;
        #endregion Inspector Variables

        #region Public Properties
        public int ClipLength { get { return _Clip.samples; } }
        #endregion Public Properties

        #region Private Variables
        private float[] _ClipArray;
        #endregion Private Variables

        #region Public Methods
        /// <summary>
        /// returns an array of samples from the clip
        /// </summary>
        /// <param name="startSample">first sample index</param>
        /// <param name="endSample">last sample index</param>
        public float[] GetClipFragment(int startSample, int endSample)
        {
            //if requested samples are out of range
            if (startSample < 0 || endSample < 0)
            {
                throw new System.ArgumentOutOfRangeException("requested samples have negative indexes, which means the player is too far from the source");
            }
            if (startSample > ClipLength || endSample > ClipLength)
            {
                throw new System.ArgumentOutOfRangeException("requested samples are out of range");
            }

            //leng and direction of the requested clip fragment
            var length = endSample - startSample;
            var sign = length < 0 ? -1 : 1;
            length = length * sign;

            var result = new float[length];

            for (var i = 0; i < length; i++)
            {
                result[i] = _ClipArray[startSample + i * sign];
            }

            return result;
        }

        /// <summary>
        /// returns a sample from the clip at the given index
        /// </summary>
        public float GetSample(int index)
        {
            if (index < 0 || index > ClipLength)
            {
                throw new System.ArgumentOutOfRangeException();
            }

            return _ClipArray[index];
        }
        #endregion Public Methods

        #region Unity Methods
        private void Start()
        {
            _ClipArray = new float[ClipLength];
            _Clip.GetData(_ClipArray, 0);
        }
        #endregion Unity Methods
    }
}
