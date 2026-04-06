using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class FadeButtonInvoker : MonoBehaviour
{
    [SerializeField] private UnityEvent onFadeComplete;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(HandleClick);
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(HandleClick);
        }
    }

    private void HandleClick()
    {
        if (FadeTransitionManager.Instance == null)
        {
            Debug.LogWarning("FadeTransitionManager not found in scene.");
            onFadeComplete?.Invoke();
            return;
        }

        FadeTransitionManager.Instance.PlayTransition(() =>
        {
            onFadeComplete?.Invoke();
        });
    }
}