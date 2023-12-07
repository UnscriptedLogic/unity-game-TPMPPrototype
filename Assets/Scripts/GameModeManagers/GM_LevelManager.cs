using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class GM_LevelManager : UGameModeBase
{
    [Header("Game Mode")]
    [SerializeField] private float nodeTickInterval = 0.1f;

    private TickSystem.Ticker ticker;

    public TickSystem.Ticker NodeTickSystem => ticker;

    protected override IEnumerator Start()
    {
        ticker = TickSystem.Create("Node Ticker", nodeTickInterval);

        return base.Start();
    }
}
