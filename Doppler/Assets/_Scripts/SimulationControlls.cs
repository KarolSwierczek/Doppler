using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationControlls : MonoBehaviour {

    [SerializeField] private List<GameObject> _Sources;
    [SerializeField] private List<Text> _Statuses;

    private void Update()
    {
        if(_Sources.Count < 3)
        {
            Debug.LogError("there should be 3 sources");
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToggleSource(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ToggleSource(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ToggleSource(2);
        }
    }

    private void ToggleSource(int idx)
    {
        var active = _Sources[idx].activeSelf;
        _Sources[idx].SetActive(!active);
        SetStatus(_Statuses[idx], !active);
    }

    private void SetStatus(Text text, bool isActive)
    {
        text.text = isActive ? "Active" : "Inactive";
        text.color = isActive ? Color.green : Color.red;
    }
}
