using JetBrains.Annotations;
using System;
using UnityEngine;
using UnscriptedEngine;

public class O_Build_TutorialDeployer : O_Build
{
    [SerializeField] private UCanvasController canvasController;
    [SerializeField] private Transform websiteCanvasTransform;
    [SerializeField] private InputNode inputNode;
    [SerializeField] private Vector2Int requiredRateRange = new Vector2Int(10, 24);
    
    [SerializeField] private bool useRate;

    private Bindable<int> requiredRate = new Bindable<int>(0);
    private WebPageSO.PageData currentPageData;
    private float elapsedTime = 0f;
    private IUsesPageObjects pageObjectInterface;
    private GameObject webPage;

    public Bindable<int> acceptedPagesRate = new Bindable<int>(0);

    public bool HasReachedRequiredRate => acceptedPagesRate.Value >= requiredRate.Value;

    public event EventHandler OnDeployerRecievedValidItem;

    protected override void Start()
    {
        base.Start();

        canvasController.OnWidgetAttached(this);

        if (useRate)
        {
            canvasController.BindUI(ref acceptedPagesRate, "rate", value => $"{value} pages/min");
            canvasController.BindUI(ref requiredRate, "required", value => $"Min. {value}");
        }
    }

    protected override void OnLevelStarted()
    {
        requiredRate.Value = UnityEngine.Random.Range(requiredRateRange.x, requiredRateRange.y);

        pageObjectInterface = GameMode as IUsesPageObjects;
        if (pageObjectInterface == null)
        {
            Debug.Log("GameMode doesn't use IUsesPageObjects");
            return;
        }
    }

    public void InitializeDeployer(int pageIndex)
    {        
        if (webPage != null)
        {
            Destroy(webPage);
            webPage = null;

            currentPageData = null;
        }

        currentPageData = pageObjectInterface.WebpageSO.WebPageDataSet[pageIndex];
        webPage = Instantiate(currentPageData.WebPage, websiteCanvasTransform);
    }

    protected override void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (inputNode.TryGetBuildItem(out O_BuildItem item))
        {
            BuildBehaviours.ConsumeItem(this, item);

            O_BuildPage page = item as O_BuildPage;

            //Validate
            if (pageObjectInterface.WebpageSO.IsComponentRequirementsMet(page, currentPageData))
            {
                acceptedPagesRate.Value++;

                OnDeployerRecievedValidItem?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                //Preferrably we wanna show some form of feedback when the wrong page is given (and why?)
            }
        }
    }

    //Object cannot be deleted
    public override void DeleteSelf() { }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= 60f)
        {
            elapsedTime = 0f;
            acceptedPagesRate.Value = 0;
        }
    }
}