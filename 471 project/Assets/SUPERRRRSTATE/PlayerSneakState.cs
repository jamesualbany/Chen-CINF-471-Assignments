using UnityEngine;

public class PlayerSneakState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entering Sneak State");
    }

    public override void UpdateState(PlayerStateManager player)
    {
        //What are you doing during this state?
        player.MovePlayer(player.default_speed / 2);


        //On what condition do we leave this state
        if (player.movement.magnitude < 0.1)
        {
            player.SwitchState(player.idleState);
        } else if (player.IsSneaking == false)
        {
            player.SwitchState(player.walkState);
        }
    }
}
