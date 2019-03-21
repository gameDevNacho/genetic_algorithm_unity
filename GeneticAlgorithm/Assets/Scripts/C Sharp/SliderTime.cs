using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderTime : MonoBehaviour
{
    [SerializeField]
    private Text timeScaleNumber;

    public void OnValueChanged(float value)
    {
        Time.timeScale = value;
        timeScaleNumber.text = Time.timeScale.ToString();
    }
}
