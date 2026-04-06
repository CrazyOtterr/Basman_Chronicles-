using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class FadeTransitionManager : MonoBehaviour
{
    public static FadeTransitionManager Instance;

    [SerializeField] private Image fadeOverlay;
    [SerializeField] private float fadeDuration = 0.25f;

    private bool isTransitioning;
    private bool shouldFadeInAfterLoad;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        SetAlpha(0f);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void LoadLevelWithFade(int levelIndex)
    {
        if (isTransitioning) return;
        StartCoroutine(LoadLevelRoutine(levelIndex));
    }

    public void PlayTransition(System.Action actionAfterFadeOut)
    {
        if (isTransitioning) return;
        StartCoroutine(PlayTransitionRoutine(actionAfterFadeOut));
    }

    private IEnumerator LoadLevelRoutine(int levelIndex)
    {
        isTransitioning = true;

        yield return fadeOverlay.DOFade(1f, fadeDuration).WaitForCompletion();

        shouldFadeInAfterLoad = true;
        SceneManager.LoadScene(levelIndex);
    }

    private IEnumerator PlayTransitionRoutine(System.Action actionAfterFadeOut)
    {
        isTransitioning = true;

        yield return fadeOverlay.DOFade(1f, fadeDuration).WaitForCompletion();

        actionAfterFadeOut?.Invoke();

        isTransitioning = false;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!shouldFadeInAfterLoad) return;

        shouldFadeInAfterLoad = false;
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        SetAlpha(1f);

        yield return fadeOverlay.DOFade(0f, fadeDuration).WaitForCompletion();

        isTransitioning = false;
    }

    private void SetAlpha(float alpha)
    {
        if (fadeOverlay == null) return;

        Color color = fadeOverlay.color;
        color.a = alpha;
        fadeOverlay.color = color;
    }
}