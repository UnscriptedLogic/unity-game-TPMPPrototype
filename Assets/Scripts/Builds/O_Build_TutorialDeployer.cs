using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnscriptedEngine;

public class O_Build_TutorialDeployer : O_Build, IDeployer
{
    [SerializeField] private UCanvasController canvasController;
    [SerializeField] private Transform websiteCanvasTransform;
    [SerializeField] private InputNode inputNode;
    [SerializeField] private Vector2Int requiredRateRange = new Vector2Int(10, 24);
    [SerializeField] private bool useRate;
    [SerializeField] private CanvasRecievedItemGlint canvasGlint;

    [Header("Deployer Changed Settings")]
    [SerializeField] private Image deployerChangedImage;
    [SerializeField] private CanvasGroup deployerChangedCanvasGroup;
    [SerializeField] private Color changedDeployerColor;

    [SerializeField] private Transform deployerNameTransform;
    [SerializeField] private GameObject labelTMP;
    [SerializeField] private GameObject requiredTMP;
    [SerializeField] private GameObject rateTMP;

    private Bindable<int> requiredRate = new Bindable<int>(0);
    private WebPageSO.PageData currentPageData;
    private IUsesPageObjects pageObjectInterface;
    private GameObject webPage;
    private float previousAcceptedTime = 0f;
    private int totalItemsReceived;

    public Bindable<int> acceptedPagesRate = new Bindable<int>(0);

    public bool HasReachedRequiredRate => acceptedPagesRate.Value >= requiredRate.Value;
    public int AcceptedPageCount => totalItemsReceived;

    public event EventHandler OnDeployerRecievedValidItem;

    protected override void Start()
    {
        base.Start();

        canvasController.OnWidgetAttached(this);
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
                previousAcceptedTime = Time.time;
                totalItemsReceived++;
                canvasGlint.FlashSuccess();

                OnDeployerRecievedValidItem?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                canvasGlint.FlashError();
            }
        }
    }

    //Object cannot be deleted
    public override void DeleteSelf() { }

    private void Update()
    {
        CalculateRate();
    }

    private void CalculateRate()
    {
        acceptedPagesRate.Value = (int)(60.0f / Time.time - previousAcceptedTime);
    }

    public void StartUsingRate()
    {
        deployerChangedImage.color = changedDeployerColor;
        deployerChangedCanvasGroup.alpha = 1;
        deployerChangedCanvasGroup.DOFade(0f, 0.5f).SetEase(Ease.InSine);

        labelTMP.transform.position = deployerNameTransform.position;
        requiredTMP.SetActive(true);
        rateTMP.SetActive(true);

        requiredRate = new Bindable<int>(0);

        canvasController.BindUI(ref acceptedPagesRate, "rate", value => $"{value} pages/min");
        canvasController.BindUI(ref requiredRate, "required", value => $"Min. {value}");

        requiredRate.Value = 17;
    }
}