using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {
    #region Inspector Variables
    [SerializeField] private Source _Source;
    [SerializeField] private float _MaxVelocity;
    [SerializeField] private float gain = 0.5F;
    [SerializeField] private int _BufferSize = 4100;
    #endregion Inspector Variables

    #region Private Variables
    private Queue<float> _Buffer;
    private bool _CanRead = false;

    private bool running = false;
    private int _SampleRate;
    private int _FirstSample;
    private float _PrevSampleVelocity;
    #endregion Private Variables

    #region Unity Methods
    private void Start()
    {
        _Buffer = new Queue<float>();

        _SampleRate = AudioSettings.outputSampleRate;
        _PrevSampleVelocity = 0;

        var distFromSource = Vector3.Distance(transform.position, _Source.transform.position);
        _FirstSample = _Source.ClipLength - (int)(distFromSource / _MaxVelocity * _SampleRate);

        running = true;
    }

    private void FixedUpdate()
    {
        //pause if the application is not running
        if (!running) { return; }

        //time since the last update mesaured in samples
        var sampleTime = (int)(Time.fixedDeltaTime * _SampleRate);
        var timeSquared = sampleTime * sampleTime;

        //current distance from the source
        var distFromSource = Vector3.Distance(transform.position, _Source.transform.position);

        //index of the sample from source clip, that corresponds to the current position
        var lastSample = _Source.ClipLength - (int)(distFromSource / _MaxVelocity * _SampleRate);
        var deltaSample = _FirstSample - lastSample;
        var absDeltaSample = Mathf.Abs(deltaSample);

        if (absDeltaSample == 0)
        {
            var lastSampleValue = _Source.GetSample(lastSample);

            for (var n = 0; n < sampleTime; n++)
            {
                _Buffer.Enqueue(lastSampleValue);
            }

            _PrevSampleVelocity = 0f;
        }
        else {

            //a fragment of source clip based on distance travelled since last update
            var fragment = _Source.GetClipFragment(_FirstSample, lastSample);

            //approximate acceleration based on distance travelled since last update
            //acceleration is constant
            var sampleAcceleration = 2 * (deltaSample - _PrevSampleVelocity * sampleTime) / timeSquared;

            //var normDist = sampleAcceleration*1000f;
            //Debug.Log("A: " + sampleAcceleration*1000 + " V0: "+ _PrevSampleVelocity + " dS: " + deltaSample);



            //for each "output" sample
            for (var n = 0; n < sampleTime; n++)
            {
                //index of a corresponding sample from original clip fragment
                //can be non-integer
                var realSample = Mathf.Abs(n * (_PrevSampleVelocity + sampleAcceleration * n / 2));
                var idx = (int)realSample;


                if (idx > absDeltaSample - 2)
                {
                    _Buffer.Enqueue(fragment[absDeltaSample - 1]);
                }
                else
                {
                    var factor = realSample - idx;
                    //linear interpolation based on the two nearest samples from clip fragment
                    var value = fragment[idx] * (1 - factor) + fragment[idx + 1] * factor;
                    _Buffer.Enqueue(value);
                }

                //_Buffer.Enqueue(normDist);
            }

            _PrevSampleVelocity += sampleTime * sampleAcceleration;
        }
        _FirstSample = lastSample;       
        
        //check if the data from buffer can be read
        if(_Buffer.Count > _BufferSize) { _CanRead = true; }
        else if(_Buffer.Count < _BufferSize/2) { _CanRead = false; }
    }

    
    private void OnAudioFilterRead(float[] data, int channels)
    {
        //pause if the application is not running
        if (!running || !_CanRead) { return; }

        //time since the last update mesaured in samples
        var sampleTime = data.Length / channels;

        //for each "output" sample
        for (var n = 0; n < sampleTime; n++)
        {
            var value = _Buffer.Dequeue();
            for (var i = 0; i < channels; i++)
            {
                data[n * channels + i] = gain * value;
            }
        }
    }
    
    #endregion Unity Methods
}
