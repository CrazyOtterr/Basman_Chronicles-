using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public UnityEvent ToTheNextLevel;
    public Camera mainCamera;
    public Transform targetPosition;

    private void Start()
    {
        Debug.Log($"targetPosition: {targetPosition.position}");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Левая кнопка мыши
        {
            // Создаем луч из камеры в точку клика
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            Debug.Log("Левая кнопка нажата!");

            if (hit.collider != null)
            {
                Debug.Log("Во что-то попали...");
                if (hit.collider.gameObject == gameObject)
                {
                    Debug.Log("Идём к двери!");
                    
                    PnC_Player.inst.controller.MoveTo(targetPosition.position, () => { ToTheNextLevel.Invoke(); });
                }
            }
        }
    }
}
