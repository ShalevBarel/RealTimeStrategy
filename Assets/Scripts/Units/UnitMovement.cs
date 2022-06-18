using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float chaseRange = 10f;


    #region Server

    public override void OnStartServer()
    {
        GameOverHandler.GameOver += HandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.GameOver -= HandleGameOver;
    }

    private void Update() // "update" method is being called every frame. this is where we do our constant checking if the player has already arrived and doesnt need to keep trying to move
    {
        Targetable target = targeter.GetTarget();

        if (target != null)
        {
            if (Vector3.Distance(transform.position, target.transform.position) > chaseRange)//
            {                                                                                //
                agent.SetDestination(target.transform.position);                             //
            }                                                                                //
            else if (agent.hasPath)                                                          //cheking if a target needs to be chased
            {                                                                                //
                agent.ResetPath();                                                           //
            }                                                                                //
                                                                                             //
            return;                                                                          //
        }

        if (!agent.hasPath) { return; }

        if(agent.remainingDistance > agent.stoppingDistance) { return; }

        agent.ResetPath();
    }

    [Command] // "command" attribute is used to make a client 'ask' a server to do something. In this case, to move.
    public void CmdMove(Vector3 position)
    {
        ServerMove(position);
    }

    public void ServerMove(Vector3 position)
    {
        targeter.ClearTarget();

        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }  //
                                                                                                      //checking if possible to move where you clicked, if it is - we move it
        agent.SetDestination(hit.position);                                                           //
    }

    private void HandleGameOver()
    {
        agent.ResetPath();
    }

    #endregion


}
