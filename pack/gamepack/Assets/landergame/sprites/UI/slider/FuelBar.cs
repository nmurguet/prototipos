﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelBar : MonoBehaviour
{
    public Slider slider;

    public Gradient gradient;

    public Image fill; 


    public void SetMaxFuel(float fuel)
    {
        slider.maxValue = fuel;
        slider.value = fuel;

        fill.color = gradient.Evaluate(1f); 
        
    }

    public void SetFuel(float fuel)
    {
        slider.value = fuel;
        gradient.Evaluate(1f);
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
