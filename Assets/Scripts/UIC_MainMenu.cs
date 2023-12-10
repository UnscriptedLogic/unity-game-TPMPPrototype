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
    [SerializeField] private GameObject contentTab2;

    public event EventHandler OnQuitBtnClickedEvent;

    public override void OnWidgetAttached(ULevelObject context)
    {
        base.OnWidgetAttached(context);

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
        BindUI(ref GameMode.GameInstance.CastTo<GI_CustomGameInstance>().packets, "packets", (value) => $"Packets: {value}");

        overviewPage.SetActive(false);
        findProjectsSection.SetActive(false);
        shopSection.SetActive(false);

        contentTab2.SetActive(false);
    }

    private void OnLevelClicked(string id)
    {
        contentTab1.SetActive(false);
        contentTab2.SetActive(false);

        if (id == "level1")
        {
            contentTab1.SetActive(true);
        }
        else if (id == "level2")
        {
            contentTab2.SetActive(true);
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
        UnBindUI(ref GameMode.GameInstance.CastTo<GI_CustomGameInstance>().packets, "packets");

        base.OnDestroy();
    }
}