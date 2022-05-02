using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private GameObject gameOverDisplayParent = null;
    [SerializeField] private TMP_Text winnerNameText = null;

    private void Start() // called when this script is first opened
    {
        GameOverHandler.ClientGameOver += ClientGameOverHandler; // listens to know if game is over
    }

    private void OnDestroy() // called when script no longer in use
    {
        GameOverHandler.ClientGameOver -= ClientGameOverHandler; // stops listening
    }

    public void LeaveGame() // methind is being used to finish the game
    {
        if(NetworkServer.active && NetworkClient.isConnected) // if host send everyone to home screen
        {
            NetworkManager.singleton.StopHost();
        }
        else // if client send just him
        {
            NetworkManager.singleton.StopClient();
        }
    }

    private void ClientGameOverHandler(string winnerName) // displaying winner's name
    {
        winnerNameText.text = $"{winnerName} Has Won!";


        gameOverDisplayParent.SetActive(true);
    }
}
