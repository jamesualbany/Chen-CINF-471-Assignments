using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        Debug.Log("Entering Run State");

        Collider[] enemiesInRange = Physics.OverlapSphere(player.transform.position, 100f);

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
        player.MovePlayer(player.run_speed);

        if (player.movement.magnitude < 0.1f)
        {
            player.SwitchState(player.idleState);
        }
        else if (!player.IsRunning)
        {
            player.SwitchState(player.walkState);
        }
    }
}
