using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class GM_LevelManager : UGameModeBase, IBuildSystem, IFactoryValidation, IUsesPageObjects
{
    [System.Serializable]
    public class MiscActiveObjects
    {
        public int levelIndex;
        public List<GameObject> objectsToActivate;
    }

    [Header("Game Mode")]
    [SerializeField] private UIC_GameLevelHUD gameLevelHUD;
    [SerializeField] private UIC_ProjectCompletionHUD gameCompletedHUD;
    [SerializeField] private float nodeTickInterval = 0.1f;
    [SerializeField] private float evaluateTime = 10f;
    [SerializeField] private Transform deployerParent;
    [SerializeField] private O_Build_Deployers deployerPrefab;

    [Header("Misc")]
    [SerializeField] private List<MiscActiveObjects> miscActiveObjects;

    private List<O_Build_Deployers> deployers;
    private WebPageSO webpageData;
    private bool isSpeedingUpFactoryOverTime;
    private float lerp = 0;
    private float maxFactoryEvaluateSpeed = 8f;
    private float lerpToMaxSpeedTime = 0.1f;
    private float _evaluateTime;

    private GI_CustomGameInstance customGameInstance;

    private float globalBeltSpeed;

    private event Action OnProjectSpeedingUpTime;
    private event Action OnSpeedUpTimeCompleted;

    public event EventHandler OnProjectCompleted;
    public event EventHandler OnProjectEvaluationCompleted;
    public event EventHandler OnClearAllObjects;

    private TickSystem.Ticker ticker;

    public WebPageSO WebpageSO => webpageData;
    public TickSystem.Ticker NodeTickSystem => ticker;
    public float GlobalBeltSpeed => globalBeltSpeed;
    public List<O_Build_Deployers> Deployers => deployers;
    public bool IsSpeedingUpFactoryOverTime => isSpeedingUpFactoryOverTime;

    public bool IsProjectCompleted
    {
        get
        {
            return customGameInstance.Project.IsCompleted;
        }

        set
        {
            if (value)
            {
               customGameInstance.Project.Complete();
            }
        }
    }

    protected override IEnumerator Start()
    {
        customGameInstance = GameInstance.CastTo<GI_CustomGameInstance>();

        webpageData = customGameInstance.Project.WebPageSO;

        globalBeltSpeed = customGameInstance.playerData.Value.conveyorBeltSpeed.Value;
        nodeTickInterval = customGameInstance.playerData.Value.tickSpeed.Value;
        ticker = TickSystem.Create("Node Ticker", nodeTickInterval);

        OnProjectSpeedingUpTime += GM_LevelManager_OnProjectSpeedingUpTime;
        OnSpeedUpTimeCompleted += GM_LevelManager_OnSpeedUpTimeCompleted;

        customGameInstance.Project.SetRequirements(new List<Requirement>()
        {
            new DeployersRecieveItem(),
            new DeployersMeetRateRequirement(),
        });

        for (int i = 0; i < miscActiveObjects.Count; i++)
        {
            if (miscActiveObjects[i].levelIndex != customGameInstance.LevelToLoad) continue;

            for (int j = 0; j < miscActiveObjects[i].objectsToActivate.Count; j++)
            {
                miscActiveObjects[i].objectsToActivate[j].SetActive(true);
            }
        }

        yield return base.Start();

        GetPlayerController().AttachUIWidget(gameLevelHUD);

        float currentOffset = 0;
        float offset = 2.5f;
        float side = 1f;
        bool timeToFlip = false;
        deployers = new List<O_Build_Deployers>();
        for (int i = 0; i < webpageData.WebPageDataSet.Count; i++)
        {
            O_Build_Deployers deployer = Instantiate(deployerPrefab, deployerParent);
            deployers.Add(deployer);

            if (timeToFlip)
            {
                currentOffset += offset;
                timeToFlip = false;
            }

            if ((i % 2) == 0)
            {
                side = -1f;
                timeToFlip = true;
            }
            else
            {
                side = 1f;
            }

            deployer.transform.position = new Vector3(side * currentOffset, 0, 0);

            deployer.InitializeDeployers(webpageData.WebPageDataSet[i]);
        }
    }

    private void GM_LevelManager_OnSpeedUpTimeCompleted()
    {
        if (IsProjectCompleted)
        {
            OnProjectEvaluationCompleted?.Invoke(this, EventArgs.Empty);
            customGameInstance.Project.Complete();

            GetPlayerController().AttachUIWidget(gameCompletedHUD);
        }
    }

    private void GM_LevelManager_OnProjectSpeedingUpTime()
    {
        if (IsProjectCompleted)
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
        customGameInstance.GlobalConveyorMaterial.AnimateConveyorMaterial(globalBeltSpeed);

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

    public void FinishProject()
    {
        if (IsSpeedingUpFactoryOverTime) return;

        _evaluateTime = evaluateTime;
        IsProjectCompleted = true;
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
