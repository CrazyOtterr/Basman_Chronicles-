using UnityEngine;

public class TimeMachineLever : MonoBehaviour
{
    private PnC_InteractiveItem item;

    public Puzzle congratsPanel;
    void Start()
    {
        item = GetComponent<PnC_InteractiveItem>();
    }

    public void TurnLever()
    {
        if (Stats.Instance.isDocsPicked && Stats.Instance.isMapSolved)
        {
            item.CallPuzzle(congratsPanel);
        }
    }
}
