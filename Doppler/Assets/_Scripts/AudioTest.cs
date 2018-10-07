using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioTest : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField] private float gain = 0.5F;
    [SerializeField] private float frequency = 440f;
    #endregion Inspector Variables

    #region Private Variables
    private bool running = false;
    private int sampleRate;
    private double increment;
    private double phase;
    #endregion Private Variables

    #region Unity Methods
    private void Start()
    {
        sampleRate = AudioSettings.outputSampleRate;
        running = true;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (!running)
            return;

        increment = frequency * 2 * System.Math.PI / sampleRate;

        var dataLen = data.Length / channels;

        for (var n = 0; n < dataLen; n++)
        {
            var x = (float)System.Math.Sin(phase) * gain;

            for(var i = 0; i < channels; i++)
            {
                data[n * channels + i] = x;
            }

            phase += increment;
        }
    }
    #endregion Unity Methods
}