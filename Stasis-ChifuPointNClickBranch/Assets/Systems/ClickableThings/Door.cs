using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public UnityEvent ToTheNextLevel;
    public int sceneIndex;
    public Camera mainCamera;
    public Transform targetPosition;


    public void Enter()
    {
        SceneManager.LoadScene(sceneIndex);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Левая кнопка мыши
        {
            // Создаем луч из камеры в точку клика
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject == gameObject)
                {
                    PnC_Player.inst.controller.MoveTo(targetPosition.position, () => { ToTheNextLevel.Invoke(); });
                }
            }
        }
    }
}
