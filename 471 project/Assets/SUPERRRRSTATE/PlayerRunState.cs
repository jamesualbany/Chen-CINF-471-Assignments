using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entering Run State");
    }

    public override void UpdateState(PlayerStateManager player)
    {
        player.MovePlayer(player.run_speed);
        
        if (player.movement.magnitude < 0.1)
        {
            player.SwitchState(player.idleState);
        }
        else if (!player.IsRunning)
        {
            player.SwitchState(player.walkState);
        }
    }
}
