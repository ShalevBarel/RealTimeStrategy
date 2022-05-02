using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitPrefabSpawner = null;
    [SerializeField] private GameOverHandler gameOverHandlerPrefab = null;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn) // overriding an already built method and adding some custom logic
    {
        base.OnServerAddPlayer(conn);

        RTSPlayer player =  conn.identity.GetComponent<RTSPlayer>();
        player.SetTeamColor(new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)));

        GameObject unitSpawnerInstance = Instantiate(unitPrefabSpawner,  //
            conn.identity.transform.position,                            //spawning the unit spawner
            conn.identity.transform.rotation);                           //
                                                                         //
        NetworkServer.Spawn(unitSpawnerInstance, conn);                  //
    }

    public override void OnServerSceneChanged(string sceneName) // being called when a scene has changed
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map")) // if the scene is a map - spawn the game over handler
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);

            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
        }
    }
}
