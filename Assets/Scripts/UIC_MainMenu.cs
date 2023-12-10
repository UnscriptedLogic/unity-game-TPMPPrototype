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

        overviewPage.SetActive(false);
        findProjectsSection.SetActive(false);
        shopSection.SetActive(false);
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
}