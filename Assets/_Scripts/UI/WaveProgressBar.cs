using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class WaveProgressBar : ProgressBar
{
    //[SerializeField] Image foregroundImage;

    /*public void SetProgress(float progress)
    {
        foregroundImage.transform.localScale = new Vector3(progress, 1.0f, 1.0f);
    }*/

    [SerializeField] Image burgerProgressImage;
    [SerializeField] Image backgroundImage;
    [SerializeField] Image pizzaIcon;
    float barLength;

    List<Image> pizzaIcons;

    private void Start()
    {
        barLength = backgroundImage.rectTransform.rect.width;
        GameManager.Instance.onDayStarted += SetNewWaves;
    }

    private void Update()
    {
        
    }

    public void SetNewWaves()
    {
        if (pizzaIcons != null)
        {
            for (int x = 0; x < pizzaIcons.Count; x++)
            {
                Destroy(pizzaIcons[x]);
            }
        }
        else
        {
            pizzaIcons = new List<Image>();
        }
        pizzaIcons.Clear();

        EnemyWave[] waves = GameManager.Instance.enemyDayWaves[GameManager.Instance.currentDay].waves;
        for (int x = 0; x < waves.Length; x++)
        {
            Image icon = Instantiate(pizzaIcon);

            icon.transform.SetParent(backgroundImage.transform, false);
            icon.transform.localPosition = new Vector3(barLength * waves[x].spawnTime, 80.0f, 0.0f);
            pizzaIcons.Add(icon);
        }
    }

    public override void SetProgress(float progress)
    {
        base.SetProgress(progress);

        burgerProgressImage.rectTransform.anchoredPosition = new Vector3(barLength * progress, 0.0f, 0.0f);
    }
}
