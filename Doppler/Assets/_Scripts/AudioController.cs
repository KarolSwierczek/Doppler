using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {
    #region Inspector Variables
    [SerializeField] private Source _Source;
    [SerializeField] private float _MaxVelocity;
    [SerializeField] private float gain = 0.5F;
    [SerializeField] private int _BufferSize = 4100;
    #endregion Inspector Variables

    #region Debug Variables
    private float _PrevDistFromSource = 0;
    #endregion Debug Variables

    #region Private Variables
    private Queue<float> _Buffer;
    private bool _CanRead = false;

    private bool running = false;
    private int _SampleRate;
    private int _FirstSample;
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

        //initialize the audio buffer
        _Buffer = new Queue<float>();
        _SampleRate = AudioSettings.outputSampleRate;

        //calculate initial distance from the sound source
        var distFromSource = Vector3.Distance(transform.position, _Source.transform.position);
        _FirstSample = _Source.ClipLength - (int)(distFromSource / _MaxVelocity * _SampleRate);

        running = true;

        //--variant with acceleration--  
        //_PrevSampleVelocity = 0;
    }

    private void FixedUpdate()
    {
        //pause if the application is not running
        if (!running) { return; }

        //time since the last update mesaured in samples
        var sampleTime = (int)(Time.fixedDeltaTime * _SampleRate);
        var timeSquared = sampleTime * sampleTime;

        //vector from listener position to the source
        var sourceDirection3 = _Source.transform.position - transform.position;
        var sourceDirection2 = new Vector2(sourceDirection3.x, sourceDirection3.z);

        //current distance from the source
        var distFromSource = sourceDirection2.magnitude;

        //index of the sample from source clip, that corresponds to the current position
        var lastSample = _Source.ClipLength - (int)(distFromSource / _MaxVelocity * _SampleRate);
        var deltaSample = _FirstSample - lastSample;
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
            var lastSampleValue = _Source.GetSample(lastSample);

            for (var n = 0; n < sampleTime; n++)
            {
                //left channel
                _Buffer.Enqueue(lastSampleValue * _LeftEarPolarPattern[angleIdx]);
                //right channel
                _Buffer.Enqueue(lastSampleValue * _LeftEarPolarPattern[(_LeftEarPolarPattern.Length - angleIdx) % 36]);
            }
            //--variant with acceleration--  
            //_PrevSampleVelocity = 0f;
        }
        else 
        {
            //a fragment of source clip based on distance travelled since last update
            var fragment = _Source.GetClipFragment(_FirstSample, lastSample);

            //velocity relative to max velocity
            var relativeVelocity = (distFromSource - _PrevDistFromSource) / (Time.fixedDeltaTime * _MaxVelocity);

            //--variant with acceleration-- 
            //approximate acceleration based on distance travelled since last update
            //acceleration is constant
            //var sampleAcceleration = 2 * (deltaSample - _PrevSampleVelocity * sampleTime) / timeSquared;

            //for each "output" sample
            for (var n = 0; n < sampleTime; n++)
            {
                //--variant with acceleration--               
                //var realSample = Mathf.Abs(n * (_PrevSampleVelocity + sampleAcceleration * n / 2));

                //--no acceleration vartiant--
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
                _Buffer.Enqueue(value * _LeftEarPolarPattern[angleIdx]);
                //right channel
                _Buffer.Enqueue(value * _LeftEarPolarPattern[_LeftEarPolarPattern.Length - angleIdx -1]);             
            }
            //--variant with acceleration--  
            //_PrevSampleVelocity += sampleTime * sampleAcceleration;
        }
        _FirstSample = lastSample;
        _PrevDistFromSource = distFromSource;
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
            //for two audio channels:
            //left channel
            data[n * channels] = gain * _Buffer.Dequeue();
            //right channel
            data[n * channels + 1] = gain * _Buffer.Dequeue();

            
            // for i - channel mono
            /*
            for (var i = 0; i < channels; i++)
            {
                data[n * channels + i] = gain * value;
            }
            */
        }
    }
    
    #endregion Unity Methods
}
