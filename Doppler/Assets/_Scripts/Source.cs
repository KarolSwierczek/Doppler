using UnityEngine;

namespace WaveTerrain
{
    public class Source : MonoBehaviour
    {
        #region Public Variables
        [HideInInspector]
        public int ClipLength { get { return _Clip.samples; } }
        #endregion Public Variables

        #region Public Methods
        public float[] GetClipFragment(int startSample, int endSample)
        {

            if (startSample < 0 || endSample < 0)
            {
                throw new System.ArgumentOutOfRangeException("requested samples have negative indexes, which means the player is too far from the source");
            }

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

        public float GetSample(int index)
        {
            if (index < 0 || index > ClipLength)
            {
                throw new System.ArgumentOutOfRangeException();
            }

            return _ClipArray[index];
        }
        #endregion Public Methods

        #region Inspector Variables
        [SerializeField] private AudioClip _Clip;
        #endregion Inspector Variables

        #region Unity Methods
        private void Start()
        {
            _ClipArray = new float[ClipLength];
            _Clip.GetData(_ClipArray, 0);
        }
        #endregion Unity Methods

        #region Private Variables
        private float[] _ClipArray;
        #endregion Private Variables
    }
}
