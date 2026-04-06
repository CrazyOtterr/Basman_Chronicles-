using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour
{
    public UnityEvent ToTheNextLevel;
    public Camera mainCamera;
    public Transform targetPosition;
    public Stats.DoorPoses CurrentDoorPose;

    private bool isTransitionStarted;

    private void Start()
    {
        Debug.Log($"targetPosition: {targetPosition.position}");
    }

    private void Update()
    {
        if (isTransitionStarted) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isTransitionStarted = true;

                PnC_Player.inst.controller.MoveTo(targetPosition.position, () =>
                {
                    ToTheNextLevel.Invoke();
                    Stats.Instance.door = CurrentDoorPose;
                });
            }
        }
    }
}