using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class TimeMachineGap
{
    public Button gapButton;
    public string correctAnswer;
    public List<string> options;
    [HideInInspector] public string currentAnswer;
}

public class TimeMachinePuzzleController : MonoBehaviour
{
    [Header("Puzzle Control")]
    public Puzzle puzzle; // сюда перетащишь тот же объект, что у Close

    [Header("Gaps")]
    public List<TimeMachineGap> gaps;

    [Header("Dropdown")]
    public GameObject dropdownPanel;
    public Transform optionsParent;
    public Button optionButtonPrefab;

    [Header("Buttons")]
    public Button checkButton;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip successClip;
    public AudioClip failClip;



    private TimeMachineGap currentGap;

    void Start()
    {
        dropdownPanel.SetActive(false);
        checkButton.gameObject.SetActive(false);

        foreach (var gap in gaps)
        {
            gap.currentAnswer = "";
            gap.gapButton.GetComponentInChildren<TMP_Text>().text = "____";
        }
    }

    // === ВОТ ЭТОТ МЕТОД ТЕПЕРЬ ВИДЕН В UNITY ===
    public void OnGapClicked(int gapIndex)
    {
        if (gapIndex < 0 || gapIndex >= gaps.Count)
            return;

        currentGap = gaps[gapIndex];
        OpenDropdown(currentGap);

        Debug.Log("Нажат Gap " + gapIndex);
    }

    void OpenDropdown(TimeMachineGap gap)
    {
        dropdownPanel.SetActive(true);
        ClearOptions();

        foreach (string option in gap.options)
        {
            Button btn = Instantiate(optionButtonPrefab, optionsParent);
            btn.GetComponentInChildren<TMP_Text>().text = option;

            btn.onClick.AddListener(() =>
            {
                gap.currentAnswer = option;
                gap.gapButton.GetComponentInChildren<TMP_Text>().text = option;
                dropdownPanel.SetActive(false);
                CheckAllFilled();
            });
        }
    }

    void ClearOptions()
    {
        foreach (Transform child in optionsParent)
            Destroy(child.gameObject);
    }

    void CheckAllFilled()
    {
        foreach (var gap in gaps)
        {
            if (string.IsNullOrEmpty(gap.currentAnswer))
                return;
        }

        checkButton.gameObject.SetActive(true);
    }

    public void OnCheckPressed()
    {
        foreach (var gap in gaps)
        {
            if (gap.currentAnswer != gap.correctAnswer)
            {
                OnFail();
                return;
            }
        }

        OnSuccess();
    }

    void OnSuccess()
    {
        if (audioSource && successClip)
            audioSource.PlayOneShot(successClip);
        puzzle.Disable();
        // закрытие UI / запуск кат-сцены
    }

    void OnFail()
    {
        if (audioSource && failClip)
            audioSource.PlayOneShot(failClip);

        foreach (var gap in gaps)
        {
            gap.currentAnswer = "";
            gap.gapButton.GetComponentInChildren<TMP_Text>().text = "____";
        }

        checkButton.gameObject.SetActive(false);
    }
}
