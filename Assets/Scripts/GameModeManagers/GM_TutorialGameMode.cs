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
        public int sectionIndex;
        public List<string> buildsToAdd;
    }

    [System.Serializable]
    public class Section
    {
        public List<string> buildsToAdd;
        public int pageIndex;
    }

    [Header("Tutorial Extensions")]
    [SerializeField] private UIC_ProjectCompletionHUD gameCompletedHUD;
    [SerializeField] private float evaluateTime = 10f;
    [SerializeField] private WebPageSO webpage;
    [SerializeField] private float nodeTickInterval;
    [SerializeField] private Material globalBeltMaterial;
    [SerializeField] private O_Build_TutorialDeployer deployer;
    [SerializeField] private List<Section> sections = new List<Section>();
    [SerializeField] private int resetEnergy = 3;

    [SerializeField] private UIC_TutorialLevelHUD tutorialLevelHUDPrefab;

    public Bindable<int> energy = new Bindable<int>(0);
    public Bindable<int> daysLeft = new Bindable<int>(3);

    private bool isProjectCompleted = false;
    private float lerp = 0;
    private float maxFactoryEvaluateSpeed = 8f;
    private float lerpToMaxSpeedTime = 0.1f;
    private float _evaluateTime;
    private bool isSpeedingUpFactoryOverTime = false;
    private GI_CustomGameInstance customGameInstance;
    private TickSystem.Ticker ticker;

    private UIC_TutorialLevelHUD playerTutorialLevelHUD;
    
    private int currentSectionIndex;

    private bool isUsingRate;
    private bool isRatePassed;

    public event EventHandler OnTestFactoryClicked;
    public event EventHandler OnProjectCompleted;
    public event EventHandler OnProjectEvaluationCompleted;

    public event EventHandler<OnSectionCompeletedEventArgs> OnSectionStarted;
    public event EventHandler OnClearAllObjects;
    public event EventHandler OnLastSection;

    private event Action OnProjectSpeedingUpTime;
    private event Action OnSpeedUpTimeCompleted;

    public WebPageSO WebpageSO => webpage;
    public TickSystem.Ticker NodeTickSystem => ticker;
    public bool IsSpeedingUpFactoryOverTime => isSpeedingUpFactoryOverTime;
    public bool IsProjectCompleted => isProjectCompleted;
    public int CurrentSectionIndex => currentSectionIndex;

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

        OnProjectSpeedingUpTime += GM_LevelManager_OnProjectSpeedingUpTime;
        OnSpeedUpTimeCompleted += GM_LevelManager_OnSpeedUpTimeCompleted;

        yield return base.Start();

        playerTutorialLevelHUD = GetPlayerController().AttachUIWidget(tutorialLevelHUDPrefab);

        InitializeSection(currentSectionIndex);
    }

    private void GM_LevelManager_OnSpeedUpTimeCompleted()
    {
        if (isProjectCompleted)
        {
            OnProjectEvaluationCompleted?.Invoke(this, EventArgs.Empty);
            customGameInstance.Project.Complete();

            GetPlayerController().AttachUIWidget(gameCompletedHUD);
        }
        else
        {
            daysLeft.Value--;
            energy.Value = resetEnergy;
        }
    }

    private void GM_LevelManager_OnProjectSpeedingUpTime()
    {
        if (isProjectCompleted)
        {
            customGameInstance.Project.EvaluateRequirements(this);

            //Buffer of 3 seconds so in the rare case of instant approval, it doesn't look janky
            if (_evaluateTime <= evaluateTime - 3f && customGameInstance.Project.AreAllRequirementsMet)
            {
                //All conditions are met. No need to evaluate further
                _evaluateTime = 0f;
            }
        }
    }

    private void Deployer_OnDeployerRecievedValidItem(object sender, EventArgs e)
    {
        currentSectionIndex++;

        if (isUsingRate) return;

        GetPlayerPawn().CastTo<P_PlayerPawn>().MoveCameraToPosition(new Vector3(0f, 0f, -10f), () =>
        {
            if (currentSectionIndex >= sections.Count)
            {
                //Completed Project
                return;
            }

            if (currentSectionIndex == 4)
            {
                SwitchToUsingRate();
            }

            InitializeSection(currentSectionIndex);
        });
    }

    private void SwitchToUsingRate()
    {
        isUsingRate = true;
        deployer.StartUsingRate();
        playerTutorialLevelHUD.ShowClockIn();
        playerTutorialLevelHUD.UseRateRequirement();
    }

    private void InitializeSection(int currentSectionIndex)
    {
        deployer.InitializeDeployer(sections[currentSectionIndex].pageIndex);

        OnSectionStarted?.Invoke(this, new OnSectionCompeletedEventArgs()
        {
            buildsToAdd = sections[currentSectionIndex].buildsToAdd,
            sectionIndex = currentSectionIndex,
        });
    }

    protected override void Update()
    {
        globalBeltMaterial.AnimateConveyorMaterial(GlobalBeltSpeed);

        if (isSpeedingUpFactoryOverTime)
        {
            SpeedUpFactoryOverTime();
        }

        if (isUsingRate && !isRatePassed)
        {
            if (deployer.HasReachedRequiredRate)
            {
                isRatePassed = true;
                playerTutorialLevelHUD.ShowFinishProject();
                OnLastSection?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private void SpeedUpFactoryOverTime()
    {
        if (_evaluateTime > 0f)
        {
            lerp += Time.unscaledDeltaTime * lerpToMaxSpeedTime;
            Time.timeScale = Mathf.Lerp(1f, maxFactoryEvaluateSpeed, lerp);

            _evaluateTime -= Time.unscaledDeltaTime;

            OnProjectSpeedingUpTime?.Invoke();
        }
        else
        {
            Time.timeScale = 1f;
            OnSpeedUpTimeCompleted?.Invoke();

            isSpeedingUpFactoryOverTime = false;
        }
    }

    public void TestFactory()
    {
        OnTestFactoryClicked?.Invoke(this, EventArgs.Empty);
    }

    public void ClockIn()
    {
        lerp = 0f;
        if (IsSpeedingUpFactoryOverTime) return;

        _evaluateTime = evaluateTime;
        isSpeedingUpFactoryOverTime = true;
    }

    public void FinishProject()
    {
        lerp = 0f;
        if (IsSpeedingUpFactoryOverTime) return;

        _evaluateTime = evaluateTime;
        isProjectCompleted = true;
        isSpeedingUpFactoryOverTime = true;

        OnProjectCompleted?.Invoke(this, EventArgs.Empty);
    }

    public void FireClearObjectsEvent()
    {
        OnClearAllObjects?.Invoke(this, EventArgs.Empty);
    }

    public IDeployer[] GetDeployers()
    {
        IDeployer[] deployers = new IDeployer[1];
        deployers[0] = deployer;

        return deployers;
    }
}