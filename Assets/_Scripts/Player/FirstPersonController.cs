using UnityEngine;
using BurgerPunk.Inputs;
using BurgerPunk.Combat;
using System.Linq;
using BurgerPunk.UI;

namespace BurgerPunk.Movement
{
    public class FirstPersonController : MonoBehaviour
    {
        public static FirstPersonController Instance = null;
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

        public SpeechBubble CustomerOrderBubble;
        public GameObject currentTargeted;
        public bool enableController = true;

        private void Awake()
        {
            if(Instance != null)
            {
                throw new UnityException("There can only be one Player at a time");
            }
            Instance = this;
        }

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
            if (!enableController)
            {
                return;
            }
            HandleMovement();
            HandleLook();
            HandleJump();
            HandleFire();
            HandleScroll();
            HandleTargeted();
        }

        private void HandleTargeted() // what object is currently being looked at
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 4.0f))
            {
                UpdateTargeted(hit.collider.gameObject);
            }
            else
            {
                UpdateTargeted(null);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (currentTargeted && currentTargeted.TryGetComponent<Interactable>(out Interactable interactable))
                {
                    interactable.Interact();
                }
            }
        }

        private void UpdateTargeted(GameObject newTarget)
        {
            if (currentTargeted == newTarget)
            {
                return;
            }

            if (currentTargeted != null)
            {
                if (currentTargeted.TryGetComponent<Interactable>(out Interactable oldInteractable))
                {
                    oldInteractable.gameObject.layer = LayerMask.NameToLayer("Default");
                    oldInteractable.DisableText();
                }
            }

            currentTargeted = newTarget;

            if (currentTargeted)
            {
                if (currentTargeted.TryGetComponent<CustomerBehaviour>(out CustomerBehaviour customer))
                {
                    if (customer.FoodTypesList.Any())
                    {
                        CustomerOrderBubble.gameObject.SetActive(true);
                        CustomerOrderBubble.Setup();
                        CustomerOrderBubble.transform.position = customer.gameObject.transform.position + Vector3.up * 2f;
                        CustomerOrderBubble.SetOrder(customer.FoodTypesList);
                    }
                }
                else
                {
                    CustomerOrderBubble.gameObject.SetActive(false);
                }

                if (currentTargeted.TryGetComponent<Interactable>(out Interactable interactable))
                {
                    interactable.gameObject.layer = LayerMask.NameToLayer("InteractableHighlight");
                    interactable.EnableText();
                }
            }
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

            if (HealthPoints <= 0)
            {
                FindFirstObjectByType<GameOverUI>().GameOver();
                DisableController();
            }
        }
        public void AddSpeed(float multiplier)
        {
            speed *= (1f + multiplier);
            Debug.Log("Speed increased to: " + speed);
        }

        public void AddHealth(float multiplier)
        {
            HealthPoints *= (1f + multiplier);
            Debug.Log("Health increased by: " + multiplier);
        }
        public void EnableController()
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
            enableController = true;
        }
        public void DisableController()
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            enableController = false;
        }
    }
}
