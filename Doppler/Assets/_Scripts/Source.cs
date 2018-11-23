using UnityEngine;

public class Source : MonoBehaviour {

    #region Public Methods
    public float[] GetClipFragment(int startSample, int endSample)
    {
        //actual indexes requested
        var start = ClipLength - startSample;
        var end = ClipLength - endSample;

        if(start < 0 || end < 0)
        {
            throw new System.ArgumentOutOfRangeException("requested samples have negative indexes, which means the player is too far from the source");
        }

        var length = end - start;
        var sign = length < 0 ? -1 : 1;
        length = length * sign;
        
        var result = new float[length];
       
        for(var i = 0; i < length; i++)
        {
            result[i] = _ClipArray[start + i * sign];
        }

        return result;
    }

    public float GetSampleAtDist(int distance)
    {
        var index = ClipLength - distance;

        if (index < 0 || index > ClipLength)
        {
            throw new System.ArgumentOutOfRangeException();
        }

        return _ClipArray[index];
    }
    #endregion Public Methods

    #region Inspector Variables
    [SerializeField]
    private AudioClip _Clip;

    [SerializeField]
    private string _Name = null;
    #endregion Inspector Variables

    #region Unity Methods
    private void Start()
    {
        if(_Name == null)
        {
            _Name = _Clip.name;
        }

        ClipLength = (int)(_Clip.length * _Clip.frequency);
        _ClipArray = new float[ClipLength];
        _Clip.GetData(_ClipArray, 0);
    }
    #endregion Unity Methods

    #region Private Variables
    public int ClipLength;
    private float[] _ClipArray;
    #endregion Private Variables
}
