using System;
using System.Collections;
using UnityEngine;
using UnscriptedEngine;

public class GM_TutorialGameMode : UGameModeBase, IUsesPageObjects, IBuildSystem, IFactoryValidation
{
    [SerializeField] private WebPageSO webpage;
    [SerializeField] private float nodeTickInterval;
    [SerializeField] private Material globalBeltMaterial;

    private bool isProjectCompleted = false;
    private bool isSpeedingUpFactoryOverTime = false;
    private GI_CustomGameInstance customGameInstance;
    private TickSystem.Ticker ticker;

    public event EventHandler OnTestFactoryClicked;
    public event EventHandler OnProjectCompleted;
    public event EventHandler OnProjectEvaluationCompleted;

    public WebPageSO WebpageSO => webpage;
    public TickSystem.Ticker NodeTickSystem => ticker;
    public bool IsSpeedingUpFactoryOverTime => isSpeedingUpFactoryOverTime;
    public bool IsProjectCompleted => isProjectCompleted;

    public float GlobalBeltSpeed
    {
        get
        {
            return customGameInstance.playerData.Value.conveyorBeltSpeed.Value;
        }
    }

    protected override IEnumerator Start()
    {
        customGameInstance = GameInstance.CastTo<GI_CustomGameInstance>();

        ticker = TickSystem.Create("Node Ticker", nodeTickInterval);

        yield return base.Start();
    }

    protected override void Update()
    {
        globalBeltMaterial.AnimateConveyorMaterial(GlobalBeltSpeed);
    }
}