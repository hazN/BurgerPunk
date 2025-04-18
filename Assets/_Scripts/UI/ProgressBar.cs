using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    Image foregroundImage;

    public void SetProgress(float progress)
    {
        foregroundImage.transform.localScale = new Vector3(progress, 1.0f, 1.0f);
    }
}
