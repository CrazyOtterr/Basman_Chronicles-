using UnityEngine;

public class ButtonShower : MonoBehaviour
{

    private void Start()
    {
        gameObject.SetActive(Stats.Instance.conversationSaves[1]);
    }
    public void ShowExitButton()
    {
        gameObject.SetActive(true);
    }
}
