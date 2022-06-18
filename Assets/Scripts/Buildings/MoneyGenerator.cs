using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyGenerator : NetworkBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private int moneyPerInterval = 10;
    [SerializeField] private float interval = 2f;

    private float timer;
    private RTSPlayer player;

    public override void OnStartServer() // when server starts listen to know if the player has died or if the game is over
    {
        timer = interval;
        player = connectionToClient.identity.GetComponent<RTSPlayer>();

        health.SomehtingDied += HandleSomethingDied;
        GameOverHandler.GameOver += HandleGameOver;
    }

    public override void OnStopServer() // when server stops stop listening to those things
    {
        health.SomehtingDied -= HandleSomethingDied;
        GameOverHandler.GameOver -= HandleGameOver;
    }

    private void Update() // is called every frame
    {
        if (player == null) { return; } // avoid unneccesary error

        timer -= Time.deltaTime; // deltaTime is used for it to work even for people with different fps

        if(timer <= 0) // when timer hits 0 give the player money and set the timer back
        {
            player.SetMoney(player.GetMoney() + moneyPerInterval);

            timer += interval;
        }
    }

    private void HandleSomethingDied() // if player has died destroy this object
    {
        NetworkServer.Destroy(gameObject);
    }

    private void HandleGameOver() // if game is over disable this script
    {
        enabled = false;
    }
}
