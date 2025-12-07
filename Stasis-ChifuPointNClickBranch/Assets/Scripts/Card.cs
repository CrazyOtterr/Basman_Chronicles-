using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int id;
    public bool wasUsed = false;
    [HideInInspector] public UnityEngine.UI.Image img;

    private void Awake() { img = GetComponent<UnityEngine.UI.Image>(); }

    public void Select() => PuzzleController.Instance.SelectCard(this);
}
