﻿using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {
    #region Inspector Variables
    [SerializeField] private List<Source> _Sources;
    [SerializeField] private float _MaxVelocity;
    [SerializeField] private float gain = 0.5F;
    [SerializeField] private int _BufferSize = 4100;
    #endregion Inspector Variables

    #region Private Variables
    private Queue<float> _Buffer = new Queue<float>();
    private bool _CanRead = false;

    private bool running = false;
    private int _SampleRate;
    private List<int> _FirstSamples = new List<int>();
    private List<float> _PrevDistFromSource = new List<float>();
    #endregion Private Variables

    #region Constants
    /*
     * this array represents ratio of output sample value to the initial value based on the direction from source (0 - 350 deg)
     * measurments are approximated for every 10 degrees
     * original measurments were taken every 22,5 degrees
     * source: http://www.canadianaudiologist.ca/measuring-directionality-of-modern-hearing-aids/
    */
    private readonly float[] _LeftEarPolarPattern = new float[36]
    {
        0.56f, 0.50f, 0.45f, 0.40f, 0.40f, 0.45f, 0.45f, 0.50f, 0.56f,
        0.56f, 0.56f, 0.50f, 0.45f, 0.45f, 0.40f, 0.40f, 0.45f, 0.50f,
        0.56f, 0.63f, 0.71f, 0.79f, 0.89f, 1.00f, 1.00f, 1.00f, 1.00f,
        1.00f, 1.00f, 1.00f, 1.00f, 1.00f, 0.89f, 0.79f, 0.71f, 0.63f
    };
    #endregion Constants

    #region Unity Methods
    private void Start()
    {
        //check for invalid audio settings
        if (AudioSettings.GetConfiguration().speakerMode != AudioSpeakerMode.Stereo)
        {
            throw new System.Exception("The audio speaker mode is not set to Stereo. Please change your audio settings");
        }

        _SampleRate = AudioSettings.outputSampleRate;

        //calculate initial sample indexes based on distances from the sound sources
        for (var i = 0; i <_Sources.Count; i++)
        {
            _FirstSamples.Add(GetCurrentSampleIndex(_Sources[i]));
            _PrevDistFromSource.Add(GetDistanceFromSource(_Sources[i]));
        }

        running = true;
    }

    private void FixedUpdate()
    {
        //pause if the application is not running
        if (!running) { return; }

        //time since the last update mesaured in samples
        var sampleTime = (int)(Time.fixedDeltaTime * _SampleRate);

        //an array of values that will be added to the buffer
        var values = new float[2 * sampleTime];

        //summing up sample values from all sources
        for(var i = 0; i < _Sources.Count; i++)
        {
            var samples = GetSamples(i, sampleTime);
            for(var j = 0; j < values.Length; j++)
            {
                values[j] += samples[j];
            }
        }

        //adding final values to the buffer
        foreach(var value in values)
        {
            _Buffer.Enqueue(value);
        }           

        //check if the data from buffer can be read       
        if (_Buffer.Count > _BufferSize) { _CanRead = true; }
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
            //for two audio channels:
            //left channel
            data[n * channels] = gain * _Buffer.Dequeue();
            //right channel
            data[n * channels + 1] = gain * _Buffer.Dequeue();
        }        
    }
    #endregion Unity Methods

    #region Private Methods
    /// <summary>
    /// Returns an array of sample values based on players velocity relative to the given source
    /// </summary>
    private float[] GetSamples(int sourceIdx, int numOfSamples)
    {
        var source = _Sources[sourceIdx];
        var result = new float[2*numOfSamples];
        var firstSample = _FirstSamples[sourceIdx];

        //vector from listener position to the source
        var sourceDirection3 = source.transform.position - transform.position;
        var sourceDirection2 = new Vector2(sourceDirection3.x, sourceDirection3.z);

        //current distance from the source
        var distFromSource = sourceDirection2.magnitude;

        //index of the sample from source clip, that corresponds to the current position
        var lastSample = source.ClipLength - (int)(distFromSource / _MaxVelocity * _SampleRate);
        var deltaSample = firstSample - lastSample;
        var absDeltaSample = Mathf.Abs(deltaSample);

        //how the sound source is angled relative to the listener (0 to 350 deg, where 0 is forward)
        var angle = Vector2.SignedAngle(sourceDirection2, new Vector2(transform.forward.x, transform.forward.z));
        //convert angle range from <-180; 180> to <0; 360)
        angle = (angle + 360) % 360f;
        //decimate the angle and cast it to int to transform it into and index for _LeftEarPolarPattern
        var angleIdx = (int)(angle / 10f);

        //if the player hasn't moved
        if (absDeltaSample == 0)
        {
            //get the value of the sample corresponding to the position of the player
            var lastSampleValue = source.GetSample(lastSample);

            for (var n = 0; n < numOfSamples; n++)
            {
                //left channel
                result[2*n] = lastSampleValue * _LeftEarPolarPattern[angleIdx];
                //right channel
                result[2*n+1] = lastSampleValue * _LeftEarPolarPattern[(_LeftEarPolarPattern.Length - angleIdx) % 36];
            }
        }
        else
        {
            //a fragment of source clip based on distance travelled since last update
            var fragment = source.GetClipFragment(firstSample, lastSample);

            //velocity relative to max velocity
            var relativeVelocity = (distFromSource - _PrevDistFromSource[sourceIdx]) / (Time.fixedDeltaTime * _MaxVelocity);

            //for each "output" sample
            for (var n = 0; n < numOfSamples; n++)
            {
                //index of a corresponding sample from original clip fragment
                //can be non-integer
                var realSample = Mathf.Abs(n * relativeVelocity);

                var idx = (int)realSample;
                float value;

                if (idx > absDeltaSample - 2)
                {
                    value = fragment[absDeltaSample - 1];
                }
                else
                {
                    var factor = realSample - idx;
                    //linear interpolation based on the two nearest samples from clip fragment
                    value = fragment[idx] * (1 - factor) + fragment[idx + 1] * factor;
                }
                //left channel
                result[2*n] = value * _LeftEarPolarPattern[angleIdx];
                //right channel
                result[2*n+1] = value * _LeftEarPolarPattern[_LeftEarPolarPattern.Length - angleIdx - 1];
            }
        }
        //update first sample index and distance from source
        _FirstSamples[sourceIdx] = lastSample;
        _PrevDistFromSource[sourceIdx] = distFromSource;

        return result;
    }

    /// <summary>
    /// returns current sample index based on players location relative to the given source
    /// </summary>
    private int GetCurrentSampleIndex(Source source)
    {
        var distFromSource = Vector3.Distance(transform.position, source.transform.position);
        return source.ClipLength - (int)(distFromSource / _MaxVelocity * _SampleRate);
    }

    /// <summary>
    /// returns current distance to the given source
    /// used in initialisation
    /// </summary>
    private float GetDistanceFromSource(Source source)
    {
        //vector from listener position to the source
        var sourceDirection3 = source.transform.position - transform.position;
        var sourceDirection2 = new Vector2(sourceDirection3.x, sourceDirection3.z);

        //current distance from the source
        return sourceDirection2.magnitude;
    }
    #endregion Private Methods
}
