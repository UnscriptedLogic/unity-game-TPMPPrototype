using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class GM_LevelManager : UGameModeBase
{
    [Header("Game Mode")]
    [SerializeField] private List<O_Build_Deployers> deployers;
    [SerializeField] private WebPageSO webpageData;
    [SerializeField] private float nodeTickInterval = 0.1f;
    [SerializeField] private int resetEnergy = 3;
    [SerializeField] private float evaluateTime = 10f;
    [SerializeField] private Material globalConveyorMaterial;

    private Project project;
    private bool isProjectCompleted = false;
    private bool isProjectEvaluated = false;
    private float lerp = 0;
    private float maxFactoryEvaluateSpeed = 8f;
    private float lerpToMaxSpeedTime = 0.1f;
    private float _evaluateTime;
    
    public Bindable<int> energy = new Bindable<int>(3);
    public Bindable<int> daysLeft = new Bindable<int>(3);

    [SerializeField] private float globalBeltSpeed = 1f;

    public event EventHandler OnTestFactoryClicked;
    public event EventHandler OnProjectCompleted;
    public event EventHandler OnProjectEvaluationCompleted;

    private TickSystem.Ticker ticker;

    public WebPageSO WebpageSO => webpageData;
    public TickSystem.Ticker NodeTickSystem => ticker;
    public int ResetEnergyAmount => resetEnergy;
    public bool IsProjectCompleted => isProjectCompleted;
    public float GlobalBeltSpeed => globalBeltSpeed;
    public List<O_Build_Deployers> Deployers => deployers;

    protected override IEnumerator Start()
    {
        nodeTickInterval = GameInstance.CastTo<GI_CustomGameInstance>().tickSpeed.Value;

        ticker = TickSystem.Create("Node Ticker", nodeTickInterval);

        return base.Start();
    }

    protected override void Update()
    {
        AnimateConveyorBeltMaterial();

        if (!isProjectCompleted) return;

        if (!isProjectEvaluated)
        {
            lerp += Time.unscaledDeltaTime * lerpToMaxSpeedTime;
            Time.timeScale = Mathf.Lerp(1f, maxFactoryEvaluateSpeed, lerp);

            _evaluateTime -= Time.unscaledDeltaTime;
        }

        if (isProjectEvaluated) return;

        if (_evaluateTime <= 0f)
        {
            Time.timeScale = 1f;
            isProjectEvaluated = true;

            OnProjectEvaluationCompleted?.Invoke(this, EventArgs.Empty);
        }
    }

    private void AnimateConveyorBeltMaterial()
    {
        globalConveyorMaterial.mainTextureOffset -= new Vector2(globalBeltSpeed * 2.115f * Time.deltaTime, 0);

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
        _evaluateTime = evaluateTime;

        isProjectCompleted = true;
        OnProjectCompleted?.Invoke(this, EventArgs.Empty);
    }
}
