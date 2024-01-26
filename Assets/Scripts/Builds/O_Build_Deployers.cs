using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnscriptedEngine;

public class O_Build_Deployers : O_Build, IDeployer
{
    [SerializeField] private UCanvasController canvasController;
    [SerializeField] private Transform websiteCanvasTransform;
    [SerializeField] private InputNode inputNode;
    [SerializeField] private CanvasRecievedItemGlint canvasGlint;

    [Header("Misc")]
    [SerializeField] private GameObject onDeletedObject;
    [SerializeField] private TextMeshProUGUI onDeletedTMP;
    [SerializeField] private List<string> onDeletedTextList;

    private Bindable<int> requiredRate = new Bindable<int>(0);
    protected WebPageSO.PageData currentPageData;
    private IUsesPageObjects pageObjectInterface;

    private float elapsedTime = 0f;
    private int totalItemsReceived = 0;

    public Bindable<int> acceptedPagesRate = new Bindable<int>(0);

    public bool HasReachedRequiredRate => acceptedPagesRate.Value >= requiredRate.Value;
    public int AcceptedPageCount => totalItemsReceived;

    protected override void Start()
    {
        base.Start();

        pageObjectInterface = GameMode as IUsesPageObjects;
        if (pageObjectInterface == null)
        {
            Debug.Log("GameMode doesn't use IUsesPageObjects");
            return;
        }

        canvasController.OnWidgetAttached(this);
        canvasController.BindUI(ref acceptedPagesRate, "rate", value => $"{value} pages/min");
        canvasController.BindUI(ref requiredRate, "required", value => $"Min. {value}");

        requiredRate.Value = currentPageData.RequiredMinPage;
    }

    public virtual void InitializeDeployers(WebPageSO.PageData pageData)
    {
        currentPageData = pageData;
        Instantiate(currentPageData.WebPage, websiteCanvasTransform);
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
                totalItemsReceived++;
                canvasGlint.FlashSuccess();
            }
            else
            {
                //Preferrably we wanna show some form of feedback when the wrong page is given (and why?)
                canvasGlint.FlashError();
            }
        }
    }

    public override void DeleteSelf()
    {
        if (indestructible) return;

        StartCoroutine(OnDeletedCoroutine());

    }

    private IEnumerator OnDeletedCoroutine()
    {
        indestructible = true;

        int randomMessageIndex = UnityEngine.Random.Range(0, onDeletedTextList.Count - 1);
        if (UnityEngine.Random.Range(0, 100) <= 10f) randomMessageIndex = onDeletedTextList.Count - 1;
        onDeletedTMP.text = onDeletedTextList[randomMessageIndex];

        canvasController.gameObject.SetActive(false);
        onDeletedObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        canvasController.gameObject.SetActive(true);
        onDeletedObject.SetActive(false);

        indestructible = false;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        CalculateRate();
    }

    private void CalculateRate()
    {
        acceptedPagesRate.Value = (int)(totalItemsReceived / (elapsedTime / 60.0f));
    }
}
