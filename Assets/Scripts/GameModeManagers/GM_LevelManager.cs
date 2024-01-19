using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class GM_LevelManager : UGameModeBase, IBuildSystem, IFactoryValidation, IUsesPageObjects
{
    [Header("Game Mode")]
    [SerializeField] private UIC_GameLevelHUD gameLevelHUD;
    [SerializeField] private UIC_ProjectCompletionHUD gameCompletedHUD;
    [SerializeField] private List<O_Build_Deployers> deployers;
    [SerializeField] private WebPageSetSO allWebPagesSO;
    [SerializeField] private float nodeTickInterval = 0.1f;
    [SerializeField] private int resetEnergy = 3;
    [SerializeField] private float evaluateTime = 10f;
    [SerializeField] private Material globalConveyorMaterial;

    private WebPageSO webpageData;
    private bool isSpeedingUpFactoryOverTime;
    private bool isProjectCompleted = false;
    private float lerp = 0;
    private float maxFactoryEvaluateSpeed = 8f;
    private float lerpToMaxSpeedTime = 0.1f;
    private float _evaluateTime;
    
    public Bindable<int> energy = new Bindable<int>(3);
    public Bindable<int> daysLeft = new Bindable<int>(3);

    private GI_CustomGameInstance customGameInstance;

    private float globalBeltSpeed;

    private event Action OnProjectSpeedingUpTime;
    private event Action OnSpeedUpTimeCompleted;

    public event EventHandler OnTestFactoryClicked;
    public event EventHandler OnProjectCompleted;
    public event EventHandler OnProjectEvaluationCompleted;
    public event EventHandler OnClearAllObjects;

    private TickSystem.Ticker ticker;

    public WebPageSO WebpageSO => webpageData;
    public TickSystem.Ticker NodeTickSystem => ticker;
    public int ResetEnergyAmount => resetEnergy;
    public bool IsProjectCompleted => isProjectCompleted;
    public float GlobalBeltSpeed => globalBeltSpeed;
    public List<O_Build_Deployers> Deployers => deployers;
    public bool IsSpeedingUpFactoryOverTime => isSpeedingUpFactoryOverTime;

    protected override IEnumerator Start()
    {
        customGameInstance = GameInstance.CastTo<GI_CustomGameInstance>();

        webpageData = allWebPagesSO.WebPageSOs[customGameInstance.LevelToLoad];

        globalBeltSpeed = customGameInstance.playerData.Value.conveyorBeltSpeed.Value;
        nodeTickInterval = customGameInstance.playerData.Value.tickSpeed.Value;
        ticker = TickSystem.Create("Node Ticker", nodeTickInterval);

        OnProjectSpeedingUpTime += GM_LevelManager_OnProjectSpeedingUpTime;
        OnSpeedUpTimeCompleted += GM_LevelManager_OnSpeedUpTimeCompleted;

        yield return base.Start();

        GetPlayerController().AttachUIWidget(gameLevelHUD);

        for (int i = 0; i < deployers.Count; i++)
        {
            deployers[i].InitializeDeployers(i);
        }
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

    protected override void Update()
    {
        globalConveyorMaterial.AnimateConveyorMaterial(globalBeltSpeed);

        if (isSpeedingUpFactoryOverTime)
        {
            SpeedUpFactoryOverTime();
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
        if (energy.Value <= 0) return;

        energy.Value--;

        OnTestFactoryClicked?.Invoke(this, EventArgs.Empty);
    }

    public void ClockIn()
    {
        if (IsSpeedingUpFactoryOverTime) return;

        if (daysLeft.Value <= 0)
        {
            //No more days left! Finish the project!
            return;
        }

        _evaluateTime = evaluateTime;
        isSpeedingUpFactoryOverTime = true;
    }

    public void FinishProject()
    {
        if (IsSpeedingUpFactoryOverTime) return;

        _evaluateTime = evaluateTime;
        isProjectCompleted = true;
        isSpeedingUpFactoryOverTime = true;

        OnProjectCompleted?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnDisable()
    {
        OnProjectSpeedingUpTime -= GM_LevelManager_OnProjectSpeedingUpTime;
        OnSpeedUpTimeCompleted -= GM_LevelManager_OnSpeedUpTimeCompleted;

        base.OnDisable();
    }

    public void FireClearObjectsEvent()
    {
        OnClearAllObjects?.Invoke(this, EventArgs.Empty);
    }

    public IDeployer[] GetDeployers()
    {
        IDeployer[] deployers = new IDeployer[this.deployers.Count];
        for (int i = 0; i < deployers.Length; i++)
        {
            deployers[i] = this.deployers[i] as IDeployer;
        }

        return deployers;
    }
}
