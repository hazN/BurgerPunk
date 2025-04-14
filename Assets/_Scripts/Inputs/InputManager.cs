using UnityEngine;

namespace BurgerPunk.Inputs
{
    public class InputManager : MonoBehaviour
    {
        public PlayerInput playerInput { get; private set; }
        private PlayerInput.PlayerActions playerActions;
        public static InputManager Instance { get; private set; }
        private void Awake()
        {
            playerInput = new PlayerInput();
            playerActions = playerInput.Player;
        }
        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
        }
        private void OnEnable()
        {
            playerActions.Enable();
        }
        private void OnDisable()
        {
            playerActions.Disable();
        }
    }
}