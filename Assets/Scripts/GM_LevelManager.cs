using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class GM_LevelManager : UGameModeBase
{
    [Header("Game Mode")]
    [SerializeField] private C_PlayerController playerController;
    [SerializeField] private P_PlayerPawn playerPawn;

    private C_PlayerController _playerController;
    private P_PlayerPawn _playerPawn;

    protected override IEnumerator Start()
    {
        _playerController = Instantiate(playerController);
        _playerPawn = Instantiate(playerPawn);

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
