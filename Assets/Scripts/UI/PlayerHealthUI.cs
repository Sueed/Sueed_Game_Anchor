using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    Image HealthSlider;

    void Awake()
    {
        HealthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
    }

    void Update()
    {
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        float sliderPercent = (float)GameManager.Instance.playStatus.currentHealth / GameManager.Instance.playStatus.maxHealth ;
        // Debug.Log(sliderPercent);
        HealthSlider.fillAmount = sliderPercent;
    }
}