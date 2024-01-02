using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class O_Build_Deployers : O_Build
{
    [SerializeField] private UCanvasController canvasController;
    [SerializeField] private Transform websiteCanvasTransform;
    [SerializeField] private InputNode inputNode;
    [SerializeField] private Bindable<int> requiredRate;

    private float elapsedTime = 0f;
    public Bindable<int> acceptedPagesRate = new Bindable<int>(0);

    private WebPageSO.PageData currentPageData;

    public bool HasReachedRequiredRate => acceptedPagesRate.Value >= requiredRate.Value;

    protected override void Start()
    {
        base.Start();

        canvasController.OnWidgetAttached(this);
        canvasController.BindUI(ref acceptedPagesRate,"rate", value => $"{value} pages/min");
        canvasController.BindUI(ref requiredRate, "required", value => $"Min. {value}");
    }

    protected override void OnLevelStarted()
    {
        requiredRate.Value = UnityEngine.Random.Range(0, 0);

        int randomIndex = UnityEngine.Random.Range(0, levelManager.WebpageSO.WebPageDataSet.Count);
        currentPageData = levelManager.WebpageSO.WebPageDataSet[randomIndex];
        Instantiate(currentPageData.WebPage, websiteCanvasTransform);
    }

    protected override void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (inputNode.TryGetBuildItem(out O_BuildItem item))
        {
            BuildBehaviours.ConsumeItem(this, item);

            O_BuildPage page = item as O_BuildPage;

            //Validate
            if (levelManager.WebpageSO.IsComponentRequirementsMet(page, currentPageData))
            {
                acceptedPagesRate.Value++;
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
