using UnityEngine;
using BurgerPunk.Inputs;
using BurgerPunk.Combat;

namespace BurgerPunk.Movement
{
    public class FirstPersonController : MonoBehaviour
    {
        private CharacterController controller;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Holster holster;

        [Header("Movement Settings")]
        [SerializeField] private float speed = 5.0f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float mouseSensitivity = 0.5f;
        [SerializeField] private float jumpHeight = 1.0f;

        private float verticalVelocity = 0f;
        private float cameraPitch = 0f;
        private float timeSinceLastScroll = 0f;
        private void Start()
        {
            controller = GetComponent<CharacterController>();
            if (controller == null)
            {
                Debug.LogError("CharacterController not found");
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            HandleMovement();
            HandleLook();
            HandleJump();
            HandleFire();
        }

        private void HandleMovement()
        {
            Vector2 input = InputManager.Instance.playerInput.Player.Movement.ReadValue<Vector2>();

            Vector3 move = (transform.right * input.x + transform.forward * input.y).normalized;

            if (controller.isGrounded && verticalVelocity < 0)
            {
                verticalVelocity = -2f;
            }

            verticalVelocity += gravity * Time.deltaTime;

            move.y = verticalVelocity;

            controller.Move(move * speed * Time.deltaTime);
        }

        private void HandleLook()
        {
            Vector2 mouseDelta = InputManager.Instance.playerInput.Player.Mouse.ReadValue<Vector2>() * mouseSensitivity;

            cameraPitch -= mouseDelta.y;
            cameraPitch = Mathf.Clamp(cameraPitch, -89f, 89f);
            playerCamera.transform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);

            transform.Rotate(Vector3.up * mouseDelta.x);
        }

        private void HandleJump()
        {
            if (controller.isGrounded && InputManager.Instance.playerInput.Player.Jump.triggered)
            {
                verticalVelocity = Mathf.Sqrt(-2f * gravity * jumpHeight);
            }
        }

        private void HandleFire()
        {
            if (InputManager.Instance.playerInput.Player.Fire.triggered || InputManager.Instance.playerInput.Player.Fire.inProgress)
            {
                if (holster.GetCurrentGun().Gun != null)
                {
                    holster.GetCurrentGun().Gun.Fire();
                }
                else
                {
                    Debug.LogError("Gun not assigned");
                }
            }
        }

        private void HandleScroll()
        {
            // Only scroll every 0.1 seconds
            if (Time.time - timeSinceLastScroll > 0.1f)
            {
                float scrollValue = InputManager.Instance.playerInput.Player.Scroll.ReadValue<Vector2>().y;

                if (scrollValue > 0)
                {
                    holster.EquipGun(holster.GetCurrentGun().GunID + 1);
                }
                else if (scrollValue < 0)
                {
                    holster.EquipGun(holster.GetCurrentGun().GunID - 1);
                }

                timeSinceLastScroll = Time.time;
            }
        }
    }
}
