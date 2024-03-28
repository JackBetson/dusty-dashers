using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name References")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Name References")]
    [SerializeField] private string move = "Move";

    private InputAction moveAction;

    public Vector2 MoveInput { get; private set; }

    // Set as Singleton
    public static PlayerInputHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("PlayerInputHandler instance already exists. Destroying object.");
            Destroy(gameObject);
        }

        moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
        RegisterInputActions();
    }

    void RegisterInputActions()
    {
        // Look if action performed, give intermediary method, look at MoveInput, read value
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        // If not moving, set to 0
        moveAction.canceled += context => MoveInput = Vector2.zero;
    }

    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();

    }
}
