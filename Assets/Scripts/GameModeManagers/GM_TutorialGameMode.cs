using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class GM_TutorialGameMode : UGameModeBase, IUsesPageObjects, IBuildSystem, IFactoryValidation
{
    [System.Serializable]
    public class OnSectionCompeletedEventArgs : EventArgs
    {
        public List<string> buildsToAdd;
    }

    [System.Serializable]
    public class Section
    {
        public List<string> buildsToAdd;
        public int pageIndex;
    }

    [Header("Tutorial Extensions")]
    [SerializeField] private WebPageSO webpage;
    [SerializeField] private float nodeTickInterval;
    [SerializeField] private Material globalBeltMaterial;
    [SerializeField] private O_Build_TutorialDeployer deployer;
    [SerializeField] private UIC_TutorialLevelHUD tutorialLevelHUDPrefab;
    [SerializeField] private List<Section> sections = new List<Section>();
    
    private bool isProjectCompleted = false;
    private bool isSpeedingUpFactoryOverTime = false;
    private GI_CustomGameInstance customGameInstance;
    private TickSystem.Ticker ticker;
    
    private int currentSectionIndex;

    public event EventHandler OnTestFactoryClicked;
    public event EventHandler OnProjectCompleted;
    public event EventHandler OnProjectEvaluationCompleted;

    public event EventHandler<OnSectionCompeletedEventArgs> OnSectionStarted;
    public event EventHandler OnClearAllObjects;

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

        deployer.OnDeployerRecievedValidItem += Deployer_OnDeployerRecievedValidItem;

        yield return base.Start();

        GetPlayerController().AttachUIWidget(tutorialLevelHUDPrefab);

        InitializeSection(currentSectionIndex);
    }

    private void Deployer_OnDeployerRecievedValidItem(object sender, EventArgs e)
    {
        currentSectionIndex++;

        GetPlayerPawn().CastTo<P_PlayerPawn>().MoveCameraToPosition(new Vector3(0f, 0f, -10f), () =>
        {
            if (currentSectionIndex >= sections.Count)
            {

                return;
            }

            InitializeSection(currentSectionIndex);
        });
    }

    private void InitializeSection(int currentSectionIndex)
    {
        deployer.InitializeDeployer(sections[currentSectionIndex].pageIndex);

        OnSectionStarted?.Invoke(this, new OnSectionCompeletedEventArgs()
        {
            buildsToAdd = sections[currentSectionIndex].buildsToAdd
        });
    }

    protected override void Update()
    {
        globalBeltMaterial.AnimateConveyorMaterial(GlobalBeltSpeed);
    }

    public void TestFactory()
    {
        OnTestFactoryClicked?.Invoke(this, EventArgs.Empty);
    }

    public void FireClearObjectsEvent()
    {
        OnClearAllObjects?.Invoke(this, EventArgs.Empty);
    }
}