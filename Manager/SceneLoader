using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    public bool IsLoading { get; private set; }

    public event Action OnLoadStarted;
    public event Action<float> OnLoadProgress;
    public event Action OnLoadCompleted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        if (IsLoading)
            return;

        SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(int buildIndex)
    {
        if (IsLoading)
            return;

        SceneManager.LoadScene(buildIndex);
    }

    public void LoadSceneAsync(string sceneName)
    {
        if (IsLoading)
            return;

        StartCoroutine(LoadSceneAsyncRoutine(sceneName));
    }

    public void LoadSceneAsync(int buildIndex)
    {
        if (IsLoading)
            return;

        StartCoroutine(LoadSceneAsyncRoutine(buildIndex));
    }

    public void ReloadScene()
    {
        if (IsLoading)
            return;

        Scene currentScene = SceneManager.GetActiveScene();

        LoadScene(currentScene.buildIndex);
    }

    public void LoadNextScene()
    {
        if (IsLoading)
            return;

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("No next scene available.");
            return;
        }

        LoadScene(nextSceneIndex);
    }

    private IEnumerator LoadSceneAsyncRoutine(string sceneName)
    {
        IsLoading = true;
        OnLoadStarted?.Invoke();

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            OnLoadProgress?.Invoke(operation.progress);

            yield return null;
        }

        IsLoading = false;

        OnLoadProgress?.Invoke(1f);
        OnLoadCompleted?.Invoke();
    }

    private IEnumerator LoadSceneAsyncRoutine(int buildIndex)
    {
        IsLoading = true;
        OnLoadStarted?.Invoke();

        AsyncOperation operation = SceneManager.LoadSceneAsync(buildIndex);

        while (!operation.isDone)
        {
            OnLoadProgress?.Invoke(operation.progress);

            yield return null;
        }

        IsLoading = false;

        OnLoadProgress?.Invoke(1f);
        OnLoadCompleted?.Invoke();
    }
}
