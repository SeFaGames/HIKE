using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HikeLoader : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider slider;
    public int sceneIndex = 1;

    void Start()
    {
        StartCoroutine(LoadAsync(sceneIndex));
    }

    IEnumerator LoadAsync(int sceneIndex)
    {
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(sceneIndex);

        loadingScreen.SetActive(true);

        while(!loadingOperation.isDone)
        {
            float progress = Mathf.Clamp01(loadingOperation.progress / 0.9f);
            Debug.Log(loadingOperation.progress);
            slider.value = loadingOperation.progress;
            yield return null;
        }
    }
}
