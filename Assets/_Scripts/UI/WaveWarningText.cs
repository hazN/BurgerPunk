using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BurgerPunk.UI
{
    public class WaveWarningText : MonoBehaviour
    {
        [SerializeField] private List<string> waveWarningLines = new List<string>();
        [SerializeField] private TextMeshProUGUI warningText;
        [SerializeField] private float displayDuration = 7f;

        private void Start()
        {
            warningText.gameObject.SetActive(false);
            GameManager.Instance.OnWaveSpawned.AddListener(ShowWarning);
        }

        private void ShowWarning()
        {
            warningText.gameObject.SetActive(true);
            warningText.text = waveWarningLines[Random.Range(0, waveWarningLines.Count)];
            StartCoroutine(HideWarning());
        }

        private IEnumerator HideWarning()
        {
            yield return new WaitForSeconds(displayDuration);
            warningText.gameObject.SetActive(false);
        }
    }
}