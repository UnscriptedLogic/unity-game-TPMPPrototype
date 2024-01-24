using System;
using System.Collections;
using UnityEngine;
using UnscriptedEngine;

public class GM_EndGameMode : UGameModeBase, IBuildSystem, IFactoryValidation, IUsesPageObjects
{
    [SerializeField] private float nodeTickInterval = 0.1f;

    private GI_CustomGameInstance customGameInstance;
    private TickSystem.Ticker ticker;

    public float GlobalBeltSpeed => 1f;
    public bool IsProjectCompleted => true;
    public bool IsSpeedingUpFactoryOverTime => false;
    public WebPageSO WebpageSO => null;
    public TickSystem.Ticker NodeTickSystem => ticker;

    public event EventHandler OnTestFactoryClicked;
    public event EventHandler OnProjectCompleted;
    public event EventHandler OnProjectEvaluationCompleted;
    public event EventHandler OnClearAllObjects;

    public void FireClearObjectsEvent() { }
    public IDeployer[] GetDeployers() => null;

    protected override IEnumerator Start()
    {
        customGameInstance = GameInstance.CastTo<GI_CustomGameInstance>();

        OnTestFactoryClicked?.Invoke(this, EventArgs.Empty);
        OnProjectCompleted?.Invoke(this, EventArgs.Empty);
        OnProjectEvaluationCompleted?.Invoke(this, EventArgs.Empty);
        OnClearAllObjects?.Invoke(this, EventArgs.Empty);

        nodeTickInterval = customGameInstance.playerData.Value.tickSpeed.Value;
        ticker = TickSystem.Create("Node Ticker", nodeTickInterval);

        return base.Start();
    }

    protected override void Update()
    {
        customGameInstance.GlobalConveyorMaterial.AnimateConveyorMaterial(GlobalBeltSpeed);
    }
}