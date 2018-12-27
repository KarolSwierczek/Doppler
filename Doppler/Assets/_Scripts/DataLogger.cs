using System.IO;
using UnityEngine;

public class DataLogger : MonoBehaviour {
    private int idx;
    private float[] data = new float[40000];

	public void LogData(float value)
    {
        if(idx < 40000)
        {
            data[idx] = value;
        }
        idx++;
    }

    private void OnDisable()
    {
        StreamWriter writer = new StreamWriter("Assets/Data/data.txt", true);
        foreach (var d in data)
        {
            writer.Write(d + ", ");
        }
    }
}
