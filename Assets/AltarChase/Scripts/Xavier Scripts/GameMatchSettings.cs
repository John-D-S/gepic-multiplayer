using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMatchSettings : MonoBehaviour
{
    [SerializeField] private Slider slider;

    [SerializeField] private TextMeshProUGUI _sliderText;
    
    private float finalValue;
  
    private void Start()
    {
        slider.onValueChanged.AddListener((v) =>
        {
            _sliderText.text = v.ToString("0");
        });
    }

    public void ConfirmSetting()
    {
        slider.value = finalValue;
        Mathf.FloorToInt(finalValue);
    }
}
