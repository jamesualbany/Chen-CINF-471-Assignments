using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateManager : MonoBehaviour
{

    [HideInInspector]
    public PlayerBaseState currentState;

    [HideInInspector]
    public PlayerIdleState idleState = new PlayerIdleState();
    [HideInInspector]
    public PlayerWalkState walkState = new PlayerWalkState();
    [HideInInspector]
    public PlayerSneakState sneakState = new PlayerSneakState();
    [HideInInspector]
    public Vector2 movement;

    public float default_speed = 1;

    public bool IsSneaking = false;
    CharacterController controller;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();

        SwitchState(idleState);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }

    //Handle Input

    void OnMove(InputValue moveVal)
    {
        movement = moveVal.Get<Vector2>();
    }

    void OnSprint(InputValue sprintVal) //IS sneaking
    {
        if (sprintVal.isPressed)
        {
            IsSneaking = true;
        } else
        {
            IsSneaking = false;
        }
    }

    //Helper Functions
    public void MovePlayer(float speed)
    {
        float moveX = movement.x;
        float moveZ = movement.y;

        Vector3 actual_movement = new Vector3(moveX, 0, moveZ);
        controller.Move(actual_movement * Time.deltaTime * speed);
    }

    public void SwitchState(PlayerBaseState newState)
    {
        currentState = newState;
        currentState.EnterState(this);
    }
}
