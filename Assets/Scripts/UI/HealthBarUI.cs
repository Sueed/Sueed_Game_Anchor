using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject HealthUIPrefab;
    public Transform BarPos;
    public bool isAlwaysVisible;
    public float VisibleTime;
    private float timeLeft;
    Image HealthSlider;
    Transform UIBar;
    Transform cam;

    CharacterStatus currentStatus;

    void Awake()
    {
        currentStatus = GetComponent<CharacterStatus>();
        currentStatus.UpdateHealthBarOnAttack += UpdateHealthBar;
    }


    void OnEnable()
    {
        cam = Camera.main.transform;
        
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if(canvas.renderMode == RenderMode.WorldSpace)
            {
                UIBar = Instantiate(HealthUIPrefab, canvas.transform).transform;
                HealthSlider = UIBar.GetChild(0).GetComponent<Image>();
                UIBar.gameObject.SetActive(isAlwaysVisible);

            }
        }

    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if(currentHealth <= 0)
        {
            Destroy(UIBar.gameObject);
            this.enabled = false;
        }
        UIBar.gameObject.SetActive(true);
        timeLeft = VisibleTime;

        float sliderPercent = (float)currentHealth / maxHealth;
        HealthSlider.fillAmount = sliderPercent;
    }

    void LateUpdate()
    {
        if(UIBar != null)
        {
            UIBar.position = BarPos.position;
            UIBar.forward = -cam.forward;

            if(!isAlwaysVisible && timeLeft <= 0)
            {
                UIBar.gameObject.SetActive(false);
            }else
            {
                timeLeft -= Time.deltaTime;
            }
        }
    }
}
