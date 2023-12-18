using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class GM_LevelManager : UGameModeBase
{
    [Header("Game Mode")]
    [SerializeField] private float nodeTickInterval = 0.1f;
    [SerializeField] private int resetEnergy = 3;

    private bool isProjectCompleted = false;

    public Bindable<int> energy = new Bindable<int>(3);
    public Bindable<int> daysLeft = new Bindable<int>(3);

    public event EventHandler OnTestFactoryClicked;
    public event EventHandler OnProjectCompleted;

    private TickSystem.Ticker ticker;

    public TickSystem.Ticker NodeTickSystem => ticker;
    public int ResetEnergyAmount => resetEnergy;
    public bool IsProjectCompleted => isProjectCompleted;

    protected override IEnumerator Start()
    {
        nodeTickInterval = GameInstance.CastTo<GI_CustomGameInstance>().tickSpeed.Value;

        ticker = TickSystem.Create("Node Ticker", nodeTickInterval);

        return base.Start();
    }

    public void TestFactory()
    {
        if (energy.Value <= 0) return;

        energy.Value--;

        OnTestFactoryClicked?.Invoke(this, EventArgs.Empty);
    }

    public void ClockIn()
    {
        daysLeft.Value--;

        energy.Value = resetEnergy;
    }

    public void FinishProject()
    {
        isProjectCompleted = true;

        energy.Value = resetEnergy;

        OnProjectCompleted?.Invoke(this, EventArgs.Empty);
    }
}
