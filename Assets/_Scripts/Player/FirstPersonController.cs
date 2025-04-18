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
        public float HealthPoints = 100f;

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
            HandleScroll();
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
                Gun current = holster.GetCurrentGun().Gun;
                if (current != null)
                {
                    current.Fire();
                }
                else
                {
                    Debug.LogWarning("No gun equipped in holster!");
                }
            }
        }


        private void HandleScroll()
        {
            float scrollValue = InputManager.Instance.playerInput.Player.Scroll.ReadValue<Vector2>().y;

            if (scrollValue != 0 && Time.time - timeSinceLastScroll > 0.05f)
            {
                if (scrollValue > 0)
                {
                    holster.NextGun();
                }
                else if (scrollValue < 0)
                {
                    holster.PreviousGun();
                }

                timeSinceLastScroll = Time.time;
            }
        }

        public void TakeDamage(float hp)
        {
            HealthPoints -= hp;
        }
    }
}
