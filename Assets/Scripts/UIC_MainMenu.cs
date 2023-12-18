using System;
using UnityEngine;
using UnscriptedEngine;

public class UIC_MainMenu : UCanvasController
{
    [Header("Pages")]
    [SerializeField] private GameObject mainPage;
    [SerializeField] private GameObject overviewPage;

    [SerializeField] private GameObject yourProjectsSection;
    [SerializeField] private GameObject findProjectsSection;
    [SerializeField] private GameObject shopSection;

    [Header("Shop Page")]
    [SerializeField] private GameObject contentTab1;
    [SerializeField] private GameObject milestoneDetails;

    [SerializeField] private GameObject designmk1Details;

    private GI_CustomGameInstance gameInstance;

    public event EventHandler OnQuitBtnClickedEvent;

    public override void OnWidgetAttached(ULevelObject context)
    {
        base.OnWidgetAttached(context);

        gameInstance = GameMode.GameInstance.CastTo<GI_CustomGameInstance>();

        Bind<UButtonComponent>("playBtn", OnPlayBtnClicked);
        Bind<UButtonComponent>("quitBtn", OnQuitBtnClicked);

        //overview page
        Bind<UButtonComponent>("yourprojects", OnYourProjectsBtnClicked);
        Bind<UButtonComponent>("findprojects", OnFindProjectsBtnClicked);
        Bind<UButtonComponent>("shopBtn", OnShopBtnClicked);
        Bind<UButtonComponent>("testproject", OnProjectClicked);
        Bind<UButtonComponent>("backtomainbtn", OnBackToMainBtnClicked);

        //Shop Page
        Bind<UButtonComponent>("level1", OnLevelClicked);
        Bind<UButtonComponent>("level2", OnLevelClicked);
        BindUI(ref gameInstance.credits, "packets", (value) => $"Credits: {value}");
        Bind<UButtonComponent>("shopboundary", CloseMilestoneDetail);

        Bind<UButtonComponent>("designmk1", OnMilestoneClicked);

        Bind<UButtonComponent>("buydesignmk1", OnBuyMilestone);

        overviewPage.SetActive(false);
        findProjectsSection.SetActive(false);
        shopSection.SetActive(false);

        milestoneDetails.SetActive(false);

        designmk1Details.SetActive(false);
    }

    private void OnBuyMilestone(string id)
    {
        if (gameInstance.credits.Value >= 100)
        {
            switch (id)
            {
                case "designmk1":
                    break;
                case "logicmk1":
                    break;
                case "utilitymk1":
                    break;
                case "designmk2":
                    break;
                case "logicmk2":
                    break;
                case "utilitymk2":
                    break;
                default:
                    break;
            }
        }
    }

    private void OnMilestoneClicked(string id)
    {
        milestoneDetails.SetActive(true);

        designmk1Details.SetActive(false);

        switch (id)
        {
            case "designmk1":
                designmk1Details.SetActive(true);
                break;
            default:
                break;
        }
    }

    private void CloseMilestoneDetail()
    {
        milestoneDetails.SetActive(false);
    }

    private void OnLevelClicked(string id)
    {
        contentTab1.SetActive(false);

        if (id == "level1")
        {
            contentTab1.SetActive(true);
        }
    }

    private void OnProjectClicked()
    {
        GameMode.LoadScene(1);
    }

    private void OnYourProjectsBtnClicked()
    {
        yourProjectsSection.SetActive(true);
        findProjectsSection.SetActive(false);
        shopSection.SetActive(false);
    }

    private void OnFindProjectsBtnClicked()
    {
        yourProjectsSection.SetActive(false);
        findProjectsSection.SetActive(true);
        shopSection.SetActive(false);
    }

    private void OnShopBtnClicked()
    {
        yourProjectsSection.SetActive(false);
        findProjectsSection.SetActive(false);
        shopSection.SetActive(true);
    }

    private void OnPlayBtnClicked()
    {
        mainPage.SetActive(false);
        overviewPage.SetActive(true);
    }

    private void OnBackToMainBtnClicked()
    {
        mainPage.SetActive(true);
        overviewPage.SetActive(false);
    }

    private void OnQuitBtnClicked()
    {
        OnQuitBtnClicked();
    }

    protected override void OnDestroy()
    {
        UnBindUI(ref GameMode.GameInstance.CastTo<GI_CustomGameInstance>().credits, "packets");

        base.OnDestroy();
    }
}