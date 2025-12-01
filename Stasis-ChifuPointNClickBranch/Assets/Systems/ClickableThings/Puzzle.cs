using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Puzzle : MonoBehaviour
{
    [SerializeField] private GameObject _puzzlePanel;
    [SerializeField] private Image _puzzleBackground;
    private void OnEnable()
    {
        _puzzlePanel.transform.localScale = Vector3.zero;
        _puzzlePanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        _puzzleBackground.DOFade(0.7f, 0.5f).From(0f);
    }
    public void Disable()
    {
        _puzzlePanel.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);

        _puzzleBackground.DOFade(0f, 0.5f).OnComplete(() => gameObject.SetActive(false));
    }
}
