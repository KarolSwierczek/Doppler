using UnityEngine;

public class AudioController : MonoBehaviour {
    #region Inspector Variables
    [SerializeField] private Source _Source;
    [SerializeField] private DataLogger _Logger;
    [SerializeField] private float _MaxVelocity;
    [SerializeField] private float gain = 0.5F;
    #endregion Inspector Variables

    #region Private Variables
    private Vector3 _SourcePosition;
    private Vector3 _MyPosition;
    private bool running = false;
    private int _SampleRate;

    private int _FirstSample;
    private int _PrevSampleVelocity;
    private double _PrevTime;
    #endregion Private Variables

    #region Unity Methods
    private void Start()
    {
        _MyPosition = transform.position;
        _SourcePosition = _Source.transform.position;

        _SampleRate = AudioSettings.outputSampleRate;
        _PrevSampleVelocity = 0;

        var distFromSource = Vector3.Distance(_MyPosition, _SourcePosition);
        _FirstSample = (int)(distFromSource / _MaxVelocity * _SampleRate);

        running = true;
    }

    private void Update()
    {
        _MyPosition = transform.position;
        _SourcePosition = _Source.transform.position;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        //pause if the application is not running
        if (!running) { return; }

        //time since the last update mesaured in samples
        var sampleTime = data.Length / channels;
        var timeSquared = sampleTime * sampleTime;

        //current distance from the source
        var distFromSource = Vector3.Distance(_MyPosition, _SourcePosition);

        //index of the sample from source clip, that corresponds to the current position
        /*
        note: lastSample and _FirstSample are actually distances in samples from the source,
        which corresponds to the last sample of the clip.
        what we actually want to get is clip fragment <max - first, max - last>
        */
        var lastSample = (int)(distFromSource / _MaxVelocity * _SampleRate);
        var deltaSample = _FirstSample - lastSample;
        var absDeltaSample = Mathf.Abs(deltaSample);

        //if the listener stays in place
        if (deltaSample == 0)
        {
            var value = _Source.GetSampleAtDist(lastSample);
            for (var i = 0; i < 2 * sampleTime; i++)
            {
                data[i] = value;
            }
            return;
        }

        //a fragment of source clip based on distance travelled since last update
        var fragment = _Source.GetClipFragment(_FirstSample, lastSample);

        //approximate acceleration based on distance travelled since last update
        //acceleration is constant
        var sampleAcceleration = 2 * (deltaSample - _PrevSampleVelocity * sampleTime) / (float)timeSquared;

        //for each "output" sample
        for (var n = 0; n < sampleTime; n++)
        {
            float value;

            //index of a corresponding sample from original clip fragment
            //can be non-integer
            var realSample = Mathf.Abs(n * (_PrevSampleVelocity + sampleAcceleration * n / 2));
            var idx = (int)realSample;

            if(idx > absDeltaSample - 1)
            {
                value = fragment[absDeltaSample - 1];
            }
            else
            {
                value = fragment[idx];
            }

            /*
            //if we go over clip fragment length
            if(idx + 1 >= absDeltaSample)
            {
                //last sample of the clip fragment
                value = fragment[absDeltaSample - 1];
            }
            else
            {
                var factor = realSample - idx;
                //linear interpolation based on the two nearest samples from clip fragment
                value = fragment[idx] * (1 - factor) + fragment[idx + 1] * factor;
            }
            */

            //uncomment to export 40000 samples to txt file
            //_Logger.LogData(value);

            for (var i = 0; i < channels; i++)
            {
                data[n * channels + i] = gain * value;
            }
        }

        _FirstSample = lastSample;
        _PrevSampleVelocity += (int)(sampleTime * sampleAcceleration);
    }
    #endregion Unity Methods
}
