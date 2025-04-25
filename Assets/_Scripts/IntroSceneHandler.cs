using UnityEngine;

public class IntroSceneHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] scenes;
    private int sceneIndex;

    public void NextScene()
    {
        // if sceneindex == size of scenes - 1, load next scene
        scenes[sceneIndex].gameObject.SetActive(false);
        sceneIndex++;
        scenes[sceneIndex].gameObject.SetActive(true);
    }
}
