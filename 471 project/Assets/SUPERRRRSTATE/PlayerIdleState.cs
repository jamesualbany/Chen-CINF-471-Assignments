using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entering Idle State");
    }

    public override void UpdateState(PlayerStateManager player)
    {
        //What are you doing during this state?
        //Nothing!

        //On what condition do we leave this state?
        if (player.movement.magnitude > 0.1)
        {
            if (player.IsSneaking)
            {
                player.SwitchState(player.sneakState);
            } else
            {
                player.SwitchState(player.walkState);
            }
        }
    }
}
