using Mirror;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    private Targetable target;

    public Targetable GetTarget()
    {
        return this.target;
    }

    public override void OnStartServer()
    {
        GameOverHandler.GameOver += HandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.GameOver -= HandleGameOver;
    }

    [Command] // "command" attribute is used for a client to tell the server to do whatever is in the method
    public void CmdSetTarget(GameObject targetGameObject) // targeting things that are targetable
    {
         if (!targetGameObject.TryGetComponent<Targetable>(out Targetable target)) { return; }

         this.target = target;
    }
    
    public void ClearTarget() // clearing the already targeted
    {
        this.target = null;
    }

    private void HandleGameOver()
    {
        ClearTarget();
    }

}
