using System;
using System.Collections;
using UnityEngine;
using UnscriptedEngine;

public class GM_MainMenuMode : UGameModeBase, IBuildSystem, IFactoryValidation, IUsesPageObjects
{
    [SerializeField] private WebPageSO webPageSO;
    [SerializeField] private Material globalConveyorMaterial;

    private TickSystem.Ticker nodeTickSystem;
    private Bindable<float> beltSpeed;

    public TickSystem.Ticker NodeTickSystem => nodeTickSystem;
    public float GlobalBeltSpeed => beltSpeed.Value;
    public bool IsProjectCompleted => true;
    public bool IsSpeedingUpFactoryOverTime => false;
    public WebPageSO WebpageSO => webPageSO;

    public event EventHandler OnTestFactoryClicked;
    public event EventHandler OnProjectCompleted;
    public event EventHandler OnProjectEvaluationCompleted;
    public event EventHandler OnClearAllObjects;

    public void FireClearObjectsEvent() { }
    public IDeployer[] GetDeployers() => new IDeployer[0];

    protected override IEnumerator Start()
    {
        nodeTickSystem = TickSystem.Create("Start Tick System", 0.4f);
        beltSpeed = GameInstance.CastTo<GI_CustomGameInstance>().playerData.Value.conveyorBeltSpeed;

        yield return base.Start();
    }

    protected override void Update()
    {
        globalConveyorMaterial.AnimateConveyorMaterial(GlobalBeltSpeed);
    }
}