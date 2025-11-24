using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using UnityEngine.UI;
public class PnC_PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float minTargetDistance;
    [Header("Perspective")]
    [SerializeField] private float baseYPosition;
    [SerializeField] private float roomPerspectiveYModifier;
    [SerializeField] private float roomPerspectiveScaleModifier;
    [SerializeField] private Vector2 scaleRange;
    /*[Header("Animation")]
    [SerializeField] private CharacterAnimator animator;
    [SerializeField] private AnimationDataSO idleAnimation;
    [SerializeField] private AnimationDataSO walkRightAnimation;
    [SerializeField] private AnimationDataSO walkLeftAnimation;
    
    [Header("Imports")]
    [SerializeField] private CursorStateSO cursor;
    [SerializeField] private CursorStateSO overridableCursor;
    */
    [SerializeField] private Transform appearanceTransform;
    [SerializeField] private Rigidbody2D physicalBody;
    [SerializeField] private Transform legsPosition;

    //[SerializeField]private AudioQuery walkQuery;
    //private float walkQueryTime = 0.5f;
    //private readonly Timer walkQueryTimer = new Timer();

    private MovementType currentMovementType;
    private bool isInputLocked = false;
    private bool isForcedInputLock = false;
    private Vector2 targetPosition;
    private UnityAction arrivalCallback;
    //Pathing
    private NavMeshPath path;
    private int pathIndex;

    /*public void SetSound(AudioQuery _walkQuery) {
        walkQuery = _walkQuery;
    }*/

    public void MoveTo(Vector2 _targetPosition, UnityAction callback) {
        HandleSettingTarget(_targetPosition, callback);
        isInputLocked = true;
    }

    public void TeleportTo(Vector2 position) {
        transform.position = (Vector3)position - legsPosition.localPosition;
    }

    public Vector2 GetPosition() {
        return legsPosition.position;
    }

    private void Update() {
        HandleInput();
        HandleMovement();
        UpdatePerspective();
        //UpdateCursor();
    }

    private void HandleInput() {
        if (!isInputLocked && !isForcedInputLock && Input.GetMouseButtonDown(0) && CanWalkToMouse() && !ScreenUtils.IsMouseOverUI()) {
            HandleSettingTarget(ScreenUtils.WorldMouse(), null);
        }
        isInputLocked = false;
    }

    private void HandleSettingTarget(Vector2 _targetPosition, UnityAction callback) {
        arrivalCallback = callback;

        path = new NavMeshPath();
        bool isSuccessful = NavMesh.CalculatePath(GetPosition(), (Vector3)_targetPosition - legsPosition.localPosition,NavMesh.AllAreas,path);
        currentMovementType = isSuccessful ? MovementType.Mouse : MovementType.None;
        pathIndex = 1;
    }

    private void HandleMovement() {
        if (currentMovementType == MovementType.None) {
            physicalBody.linearVelocity = Vector2.zero;
            //animator.SetAnimation(idleAnimation, false);
            return;
        }
        //Move to next point in path
        targetPosition = path.corners[pathIndex];

        Vector2 direction = targetPosition - (Vector2)transform.position;
        //Modify direction to account for perspective
        direction.y /= roomPerspectiveYModifier;
        physicalBody.linearVelocity = movementSpeed * direction.normalized * new Vector2(1, roomPerspectiveYModifier);
        //animator.SetAnimation(direction.normalized.x > 0 ? walkRightAnimation : walkLeftAnimation, false);
        //PlayWalkingSounds();
        if (Vector2.Distance(targetPosition, transform.position) < minTargetDistance) {
            //If point is last
            if (pathIndex >= path.corners.Length - 1) {
                arrivalCallback?.Invoke();
                currentMovementType = MovementType.None;
            } else {
                //Goto next point
                pathIndex++;
            }
        }
    }

    /*private void PlayWalkingSounds() {
        if (walkQueryTimer.Execute()) {
            walkQueryTimer.SetTime(walkQueryTime);
            AudioManager.inst.Play(walkQuery);
        }
    }*/

    /*private void UpdateCursor() {
        if (CanWalkToMouse()) {
            if (HardwareCursorManager.inst.GetCurrentCursor() == overridableCursor) {
                HardwareCursorManager.inst.SetCursor(cursor);
            }
        } else {
            if (HardwareCursorManager.inst.GetCurrentCursor() == cursor) {
                HardwareCursorManager.inst.ResetCursor();
            }
        }
    }*/

    /*public void ExternalCursorReset() {
        if (HardwareCursorManager.inst.GetCurrentCursor() != cursor) HardwareCursorManager.inst.ResetCursor();
    }*/

    private void UpdatePerspective() {
        float dy = baseYPosition - transform.position.y;
        float scale = Mathf.Clamp(1 + dy * roomPerspectiveScaleModifier, scaleRange.x, scaleRange.y);
        appearanceTransform.localPosition = new Vector3(0, scale);
        appearanceTransform.localScale = Vector3.one * scale;
    }

    private bool CanWalkToMouse() {
        NavMeshPath testPath = new NavMeshPath();
        bool isSuccessful = NavMesh.CalculatePath(GetPosition(), (Vector3)ScreenUtils.WorldMouse() - legsPosition.localPosition,NavMesh.AllAreas,testPath);
        return isSuccessful;
    }

    private enum MovementType {
        None,
        Mouse
    }
    public void ChangeInputLocked(bool v) => isForcedInputLock = v;
}
