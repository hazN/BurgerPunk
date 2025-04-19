using UnityEngine;
using UnityEngine.UI;

public class RadialProgress : MonoBehaviour
{
    [SerializeField]
    Image radialProgress;
    public event System.Action OnRadialProgressComplete;

    bool active = false;
    float progressTimer;
    float timerDuration = 9999999.0f;

    void Start()
    {
        active = false;
        progressTimer = 0.0f;
        SetProgress(0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            progressTimer += Time.deltaTime;
            SetProgress(progressTimer);

            if (progressTimer > timerDuration)
            {
                active = false;
                OnRadialProgressComplete.Invoke();
                SetProgress(0.0f);
            }
        }
    }

    public void SetProgress(float timeElapsed)
    {
        radialProgress.fillAmount = timeElapsed / timerDuration;
    }

    public void StartProgress(float duration)
    {
        active = true;
        progressTimer = 0.0f;
        timerDuration = duration;
        SetProgress(0.0f);
    }

    public void CancelProgress()
    {
        active = false;
        progressTimer = 0.0f;
        SetProgress(0.0f);
    }

    public bool IsActive()
    {
        return active;
    }
}
