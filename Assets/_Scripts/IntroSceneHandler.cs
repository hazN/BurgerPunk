using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] scenes;
    private int sceneIndex;

    public void NextScene()
    {
        if (sceneIndex == scenes.Length - 1)
        {
            GameManager.Instance.StartGame();
            return;
        }
        scenes[sceneIndex].gameObject.SetActive(false);
        sceneIndex++;
        scenes[sceneIndex].gameObject.SetActive(true);

        if (sceneIndex == 2)
        {
            AudioManager.Instance.introSFX.clip = AudioManager.Instance.sparkle;
            AudioManager.Instance.introSFX.Play();
        }
        else if (sceneIndex == 4)
        {
            AudioManager.Instance.introSFX.clip = AudioManager.Instance.notSoFast;
            AudioManager.Instance.introSFX.Play();
        }
        else if (sceneIndex == 5)
        {
            AudioManager.Instance.introSFX.clip = AudioManager.Instance.smash;
            AudioManager.Instance.introSFX.Play();
        }
        else if (sceneIndex == 6)
        {
            AudioManager.Instance.introSFX.clip = AudioManager.Instance.jumphim;
            AudioManager.Instance.introSFX.Play();
        }
        else if (sceneIndex == 7)
        {
            AudioManager.Instance.introSFX.clip = AudioManager.Instance.imDying;
            AudioManager.Instance.introSFX.Play();
        }
    }
}
