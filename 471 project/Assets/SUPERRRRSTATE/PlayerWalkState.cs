using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{ 
    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entering Walk State");

        Collider[] enemiesInRange = Physics.OverlapSphere(player.transform.position, 6f);

        foreach (Collider enemyCollider in enemiesInRange)
        {
            AMONGUSENEMY enemy = enemyCollider.GetComponent<AMONGUSENEMY>();
            if (enemy != null)
            {
                enemy.SetTarget(player.gameObject);  
                enemy.SetState(AMONGUSENEMY.State.Follow);  
            }
        }
    }

    public override void UpdateState(PlayerStateManager player)
    {
        //What are you doing during this state?
        player.MovePlayer(player.default_speed);


        //On what condition do we leave this state
        if (player.movement.magnitude < 0.1)
        {
            player.SwitchState(player.idleState);
        } else if (player.IsSneaking)
        {
                player.SwitchState(player.sneakState);
        }
    }
}
