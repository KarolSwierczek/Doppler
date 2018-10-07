using UnityEngine;

public class Source : MonoBehaviour {

    #region Public Methods
    public float[] GetClipFragment(int startSample, int endSample)
    {
        var clipLength = (int)(_Clip.length * _Clip.frequency);

        //if the starting sample number is grater than total clip length,
        //return an empty array
        if(startSample > clipLength)
        {
            Debug.Log("Starting sample number greater than total clip length");
            return new float[0];
        }

        //if the end sample number is grater than total clip lenth,
        //set the endSample to the last sample in the clip
        if(endSample > clipLength)
        {
            Debug.Log("End sample number greater than total clip length");
            endSample = clipLength - 1;
        }

        var numOfSamples = endSample - startSample;
        var result = new float[numOfSamples];
        _Clip.GetData(result, startSample);
        return result;
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
    }
    #endregion Unity Methods
}
