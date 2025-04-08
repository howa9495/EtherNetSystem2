using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerSystems
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputMapping : MonoBehaviour
    {
        PlayerInput playerInput;
        [SerializeField]InputActionAsset inputActions;

        [Header("Character Input Values")]

        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint = false;
        public bool crouch;
        public float zoom;
        public bool shoulderSwitch = true;
        public bool interact = false;
        public bool item = false;


        private InputAction moveInput, jumpInput, shoulderSwitchInput, lookInput, zoomInput, sprintInput, crounchInput, itemInput, interactInput;

        public event EventHandler<object> moveEvent, lookEvent;
        public event EventHandler<object> jumpEvent, sprintEvent, crouchEvent, interactEvent, itemEvent;
        public event EventHandler<object> zoomEvent;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;


        private void Awake()
        {
            Initialization();
        }
        public void Initialization()
        {
            playerInput ??= gameObject.GetComponent<PlayerInput>();
            playerInput.actions ??= inputActions;
            playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
        }

        private void OnEnable()
        {
            playerInput.actions.Enable();
        }
        private void OnDisable()
        {
            playerInput.actions.Disable();
        }
        private void Start()
        {
            SwitchModeInput(Enums.PlayerMode.Player);
        }
        public void SwitchModeInput(Enums.PlayerMode newModeState)
        {
            playerInput.SwitchCurrentActionMap(newModeState.ToString());
            MapInput(newModeState);
        }
        void MapInput(Enums.PlayerMode newMap)
        {
            moveInput = playerInput.actions.FindActionMap(newMap.ToString()).FindAction("Move");
            jumpInput = playerInput.actions.FindActionMap(newMap.ToString()).FindAction("Jump");
            lookInput = playerInput.actions.FindActionMap(newMap.ToString()).FindAction("Look");
            zoomInput = playerInput.actions.FindActionMap(newMap.ToString()).FindAction("CameraZoom");
            sprintInput = playerInput.actions.FindActionMap(newMap.ToString()).FindAction("Sprint");
            crounchInput = playerInput.actions.FindActionMap(newMap.ToString()).FindAction("Crounch");
            shoulderSwitchInput = playerInput.actions.FindActionMap(newMap.ToString()).FindAction("ShoulderSwitch");
            itemInput = playerInput.actions.FindActionMap(newMap.ToString()).FindAction("Item");
            interactInput = playerInput.actions.FindActionMap(newMap.ToString()).FindAction("Interact");
            if (moveInput != null)
            {
                moveInput.started += ctx => OnMove(ctx);
                moveInput.performed += ctx => OnMove(ctx);
                moveInput.canceled += ctx => OnMove(ctx);
            }
            if (jumpInput != null)
            {
                jumpInput.started += ctx => OnJump(ctx);
                jumpInput.performed += ctx => OnJump(ctx);
                jumpInput.canceled += ctx => OnJump(ctx);
            }
            if (lookInput != null)
            {
                lookInput.started += ctx => OnLook(ctx);
                lookInput.performed += ctx => OnLook(ctx);
                lookInput.canceled += ctx => OnLook(ctx);
            }
            if (zoomInput != null)
            {
                zoomInput.started += ctx => OnZoom(ctx);
                zoomInput.performed += ctx => OnZoom(ctx);
                zoomInput.canceled += ctx => OnZoom(ctx);
            }
            if (sprintInput != null)
            {
                sprintInput.started += ctx => OnSprint(ctx);
                sprintInput.performed += ctx => OnSprint(ctx);
                sprintInput.canceled += ctx => OnSprint(ctx);
            }
            if (crounchInput != null)
            {
                crounchInput.started += ctx => OnCrouch(ctx);
                crounchInput.performed += ctx => OnCrouch(ctx);
                crounchInput.canceled += ctx => OnCrouch(ctx);
            }
            if (interactInput != null)
            {
                interactInput.started += ctx => OnInteract(ctx);
                interactInput.performed += ctx => OnInteract(ctx);
                interactInput.canceled += ctx => OnInteract(ctx);
            }
            if (itemInput != null)
            {
                itemInput.started += ctx => OnItem(ctx);
                itemInput.performed += ctx => OnItem(ctx);
                itemInput.canceled += ctx => OnItem(ctx);
            }
            /*if (shoulderSwitchInput != null)
            {
                shoulderSwitchInput.started += ctx => OnShoulderSwitch(ctx);
            }*/

            /* if (itemInput != null)
             {
                 itemInput.started += ctx => OnItem(ctx);
             }*/
        }

        public void OnMove(InputAction.CallbackContext value)
        {
            MoveInput(value.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext value)
        {
            LookInput(value.ReadValue<Vector2>());
        }
        public void OnZoom(InputAction.CallbackContext value)
        {
                ZoomInput(value.ReadValue<float>());
        }
        public void OnJump(InputAction.CallbackContext value)
        {
            JumpInput(value.ReadValueAsButton());
        }

        public void OnSprint(InputAction.CallbackContext value)
        {
            SprintInput(value.ReadValueAsButton());
        }

        public void OnCrouch(InputAction.CallbackContext value)
        {
            CrounchInput(value.ReadValueAsButton());
        }

        public void OnInteract(InputAction.CallbackContext value)
        {
            InteractInput(value.ReadValueAsButton());
        }
        public void OnItem(InputAction.CallbackContext value)
        {
            ItemInput(value.ReadValueAsButton());
        }
        /*public void OnShoulderSwitch(InputAction.CallbackContext value)
        {
            ShoulderSwitchInput(shoulderSwitch = !shoulderSwitch);
        }*/

        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
            moveEvent?.Invoke(this,move);
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
            lookEvent?.Invoke(this,look);
        }
        public void ZoomInput(float newZoomInput)
        {
            zoom = newZoomInput;
            zoomEvent?.Invoke(this,zoom);
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
            jumpEvent?.Invoke(this, jump);
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
            sprintEvent?.Invoke(this, sprint);
        }
        public void CrounchInput(bool newCrouchState)
        {
            crouch = newCrouchState;
            crouchEvent?.Invoke(this, crouch);
        }
        public void InteractInput(bool newInteractState)
        {
            interact = newInteractState;
            interactEvent?.Invoke(this, interact);
        }
        public void ItemInput(bool newInteractState)
        {
            item = newInteractState;
            itemEvent?.Invoke(this, item);
        }
        /*public void ShoulderSwitchInput(bool newShoulderSwitchState)
        {
            CameraManager.Instance.shoulder = newShoulderSwitchState;
        }*/

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }

        /*private void OnItem(InputAction.CallbackContext value)
        {
            bool item = value.ReadValueAsButton();
            Debug.Log(item);
            if (item) GameManagers.Instance.PauseMode();
        }*/
    }
}