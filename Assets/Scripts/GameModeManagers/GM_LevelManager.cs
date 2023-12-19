using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnscriptedEngine;

public class GM_LevelManager : UGameModeBase
{
    [Header("Game Mode")]
    [SerializeField] private WebPageSO webpageData;
    [SerializeField] private float nodeTickInterval = 0.1f;
    [SerializeField] private int resetEnergy = 3;
    [SerializeField] private Material globalConveyorMaterial;

    private bool isProjectCompleted = false;

    public Bindable<int> energy = new Bindable<int>(3);
    public Bindable<int> daysLeft = new Bindable<int>(3);

    [SerializeField] private float globalBeltSpeed = 1f;

    public event EventHandler OnTestFactoryClicked;
    public event EventHandler OnProjectCompleted;

    private TickSystem.Ticker ticker;

    public WebPageSO WebpageSO => webpageData;
    public TickSystem.Ticker NodeTickSystem => ticker;
    public int ResetEnergyAmount => resetEnergy;
    public bool IsProjectCompleted => isProjectCompleted;
    public float GlobalBeltSpeed => globalBeltSpeed;

    protected override IEnumerator Start()
    {
        nodeTickInterval = GameInstance.CastTo<GI_CustomGameInstance>().tickSpeed.Value;

        ticker = TickSystem.Create("Node Ticker", nodeTickInterval);

        return base.Start();
    }

    protected override void Update()
    {
        globalConveyorMaterial.mainTextureOffset -= new Vector2(globalBeltSpeed * 4.25f * Time.deltaTime, 0);

        if (globalConveyorMaterial.mainTextureOffset.x <= -10)
        {
            globalConveyorMaterial.mainTextureOffset = Vector2.zero;
        }
    }

    public void TestFactory()
    {
        if (energy.Value <= 0) return;

        energy.Value--;

        OnTestFactoryClicked?.Invoke(this, EventArgs.Empty);
    }

    public void ClockIn()
    {
        if (daysLeft.Value <= 0)
        {
            //No more days left! Finish the project!
            return;
        }

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
