using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entering Jump State");
        player.JumpPlayer();
    }

    public override void UpdateState(PlayerStateManager player)
    {
        if (player.isGrounded)
        {
            player.IsJumping = false;
            player.SwitchState(player.idleState);
        }
    }
}
