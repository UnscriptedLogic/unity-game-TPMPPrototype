using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class GM_LevelManager : UGameModeBase
{
    [Header("Game Mode")]
    [SerializeField] private C_PlayerController playerController;
    [SerializeField] private P_PlayerPawn playerPawn;
    [SerializeField] private float nodeTickInterval = 0.1f;

    private C_PlayerController _playerController;
    private P_PlayerPawn _playerPawn;

    private TickSystem.Ticker ticker;

    public TickSystem.Ticker NodeTickSystem => ticker;

    protected override IEnumerator Start()
    {
        _playerController = Instantiate(playerController);
        _playerPawn = Instantiate(playerPawn);

        ticker = TickSystem.Create("Node Ticker", nodeTickInterval);

        return base.Start();
    }

    public P_PlayerPawn GetPlayerPawn()
    {
        return _playerPawn;
    }

    public C_PlayerController GetPlayerController()
    {
        return _playerController;
    }
}
