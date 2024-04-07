using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PlayerInputVirtualCursorBehaviour : MonoBehaviour
{

    VirtualMouseInput virtualMouseInput;
    private void Awake()
    {
        BindInputActions();
        MovePlayerCursorToManager();
    }

    void BindInputActions()
    {
        // Bind newly assigned playerInput actions to the virtual mouse input component
        InputActionAsset actions = GetComponent<UnityEngine.InputSystem.PlayerInput>().actions;

        virtualMouseInput = GetComponent<VirtualMouseInput>();

        virtualMouseInput.stickAction = new InputActionProperty(actions["Cursor Movement"]);
        virtualMouseInput.leftButtonAction = new InputActionProperty(actions["Select"]);
    }

    void MovePlayerCursorToManager()
    {
        var parent = GameObject.FindGameObjectWithTag("InputManager").transform.parent;
        transform.SetParent(parent);
    }
}
