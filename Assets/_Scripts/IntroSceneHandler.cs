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
    }
}
