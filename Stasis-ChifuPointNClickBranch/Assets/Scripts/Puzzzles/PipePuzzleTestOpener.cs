using UnityEngine;

/// <summary>
/// Для тестирования: нажмите P чтобы открыть головоломку с трубами.
/// Добавьте на любой объект в сцене (например, на Canvas или пустой GameObject).
/// </summary>
public class PipePuzzleTestOpener : MonoBehaviour
{
    [SerializeField] private Puzzle pipePuzzleWindow;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && pipePuzzleWindow != null)
        {
            pipePuzzleWindow.gameObject.SetActive(true);
        }
    }
}
