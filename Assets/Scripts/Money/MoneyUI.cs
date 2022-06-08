using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyText = null;

    private RTSPlayer player;

    private void Start()
    {
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

        HandleMoneyUpdated(player.GetMoney());    //
                                                  // if money is updated run a method accordingly
        player.moneyUpdated += HandleMoneyUpdated;//
    }

    private void OnDestroy()
    {
        player.moneyUpdated -= HandleMoneyUpdated; // stop listening to if money is updated
    }

    private void HandleMoneyUpdated(int money) // if money updated change the UI text
    {
        moneyText.text = $"Money: {money}";
    }
}
