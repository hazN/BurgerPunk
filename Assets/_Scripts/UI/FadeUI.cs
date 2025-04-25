using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour
{
    public static FadeUI Instance;
    [SerializeField] public Image fadeImage;
    public float fadeDuration = 1f;
    public float fadeDelay = 0f;

    Coroutine fadeRoutine = null;

    Queue<IEnumerator> fadeQueue = new Queue<IEnumerator>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0f, 0f, 0f, 1f);
            StartCoroutine(FadeFromBlackCoroutine());
        }
    }

    private void Update()
    {
        if (fadeQueue.Count > 0)
        {
            if (fadeRoutine == null)
            {
                fadeRoutine = StartCoroutine(fadeQueue.Dequeue());
            }
        }
    }

    public void FadeToBlack(System.Action onComplete = null)
    {
        fadeQueue.Enqueue(FadeToBlackCoroutine(onComplete));
    }

    public void FadeFromBlack(System.Action onComplete = null)
    {
        fadeQueue.Enqueue(FadeFromBlackCoroutine(onComplete));
    }

    public IEnumerator FadeFromBlackCoroutine(System.Action onComplete = null) // Fade from black to transparent
    {
        float time = 0f - fadeDelay;
        Color color = fadeImage.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            if (time > 0f)
            {
                float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
                fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            }
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, 0f);

        fadeRoutine = null;

        onComplete?.Invoke();
    }

    public IEnumerator FadeToBlackCoroutine(System.Action onComplete = null) // Fade to black
    {
        fadeImage.gameObject.SetActive(true); // In case it was turned off
        float time = 0f - fadeDelay;
        Color color = fadeImage.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            if (fadeDelay > 0f)
            {
                float alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);
                fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            }
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, 1f);

        fadeRoutine = null;

        onComplete?.Invoke();
    }
}