using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnscriptedEngine;

public class UIC_MainMenu : UCanvasController
{
    [System.Serializable]
    public class Milestone
    {
        [SerializeField] private List<MilestoneUpgrades> milestoneRewards;

        public List<MilestoneUpgrades> MilestoneRewards => milestoneRewards;

        public Milestone(List<MilestoneUpgrades> milestoneRewards)
        {
            this.milestoneRewards = milestoneRewards;
        }
    }

    [System.Serializable]
    public class MilestoneUpgrades
    {
        [SerializeField] private UpgradeSO upgradeSO;
        private Action onUpgrade;

        public UpgradeSO UpgradeSO => upgradeSO;
        public Action OnUpgrade => onUpgrade;

        public MilestoneUpgrades(Action onUpgrade)
        {
            this.onUpgrade = onUpgrade;
        }
    }

    [Header("Pages")]
    [SerializeField] private GameObject mainPage;
    [SerializeField] private GameObject overviewPage;

    [SerializeField] private GameObject yourProjectsSection;
    [SerializeField] private GameObject findProjectsSection;
    [SerializeField] private GameObject shopSection;

    [Header("Shop Page")]
    [SerializeField] private List<Milestone> milestones;
    [SerializeField] private Transform milestoneParent;
    [SerializeField] private MilestoneBtn milestoneButtonPrefab;
    [SerializeField] private GameObject shopContent;

    [SerializeField] private UpgradeDetails upgradeDetails;

    private UpgradeManager upgradeManager;

    [Header("Projects Page")]
    [SerializeField] private ProjectUIBtn projectPrefab;
    [SerializeField] private Transform projectsParent;

    private GI_CustomGameInstance gameInstance;

    public event EventHandler OnQuitBtnClickedEvent;

    public override void OnWidgetAttached(ULevelObject context)
    {
        base.OnWidgetAttached(context);

        gameInstance = GameMode.GameInstance.CastTo<GI_CustomGameInstance>();

        upgradeManager = GameMode.GetComponent<UpgradeManager>();

        Bind<UButtonComponent>("tutorialBtn", OnTutorialButtonClicked);
        Bind<UButtonComponent>("playBtn", OnPlayBtnClicked);
        Bind<UButtonComponent>("quitBtn", OnQuitBtnClicked);

        //overview page
        Bind<UButtonComponent>("yourprojects", OnYourProjectsBtnClicked);
        Bind<UButtonComponent>("findprojects", OnFindProjectsBtnClicked);
        Bind<UButtonComponent>("shopBtn", OnShopBtnClicked);
        Bind<UButtonComponent>("backtomainbtn", OnBackToMainBtnClicked);

        //Shop Page
        Bind<UButtonComponent>("level1", OnLevelClicked);
        Bind<UButtonComponent>("level2", OnLevelClicked);
        BindUI(ref gameInstance.playerData.Value.credits, "packets", (value) => $"Credits: {value}");
        Bind<UButtonComponent>("shopboundary", CloseMilestoneDetail);

        LoadMilestones(0);

        overviewPage.SetActive(false);
        findProjectsSection.SetActive(false);
        shopSection.SetActive(false);
        shopContent.SetActive(false);
        upgradeDetails.gameObject.SetActive(false);

        LoadProjects();
    }

    private void OnTutorialButtonClicked()
    {
        GameMode.LoadScene(2);
    }

    private void LoadMilestones(int index)
    {
        //Clear initial buttons
        for (int i = milestoneParent.childCount - 1; i >= 0; i--)
        {
            Destroy(milestoneParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < milestones[index].MilestoneRewards.Count; i++)
        {
            MilestoneBtn milestoneButton = Instantiate(milestoneButtonPrefab, milestoneParent);
            milestoneButton.Initialize(this, milestones[index].MilestoneRewards[i].UpgradeSO.UpgradeIcon, milestones[index].MilestoneRewards[i].UpgradeSO.UpgradeLabel);
        }
    }

    private void LoadProjects()
    {
        List<Project> projects = new List<Project>(gameInstance.playerData.Value.Projects);

        for (int i = 0; i < projects.Count; i++)
        {
            ProjectUIBtn projectUIBtn = Instantiate(projectPrefab, projectsParent);
            projectUIBtn.Initialize(projects[i], this);

            Bind<UButtonComponent>(projectUIBtn.ID, OnProjectClicked);
        }
    }

    public void OnBuyUpgrade(string id)
    {
        (int milestoneIndex, int rewardIndex) = GetMilestoneByID(id);

        UpgradeSO upgradeSO = milestones[milestoneIndex].MilestoneRewards[rewardIndex].UpgradeSO;

        if (gameInstance.playerData.Value.credits.Value < upgradeSO.Cost)
        {
            Debug.Log("Not enought credits");
            return;
        }

        gameInstance.playerData.Value.credits.Value -= upgradeSO.Cost;
        upgradeManager.Upgrade(milestoneIndex, rewardIndex);
    }

    public void OnMilestoneClicked(string id)
    {
        shopContent.SetActive(true);
        upgradeDetails.gameObject.SetActive(true);

        (int milestoneIndex, int rewardIndex) = GetMilestoneByID(id);

        upgradeDetails.Initialize(this, milestones[milestoneIndex].MilestoneRewards[rewardIndex].UpgradeSO);
    }

    private (int, int) GetMilestoneByID(string ID)
    {
        for (int i = 0; i < milestones.Count; i++)
        {
            for (int j = 0; j < milestones[i].MilestoneRewards.Count; j++)
            {
                if (milestones[i].MilestoneRewards[j].UpgradeSO.UpgradeLabel == ID)
                {
                    return (i, j);
                }
            }
        }

        Debug.Log("Couldn't find the milestone with the ID: " + ID);
        return (0, 0);
    }

    private void CloseMilestoneDetail()
    {
        shopContent.SetActive(false);
    }

    private void OnLevelClicked(string id)
    {

    }

    private void OnProjectClicked(string id)
    {
        if (!gameInstance.playerData.Value.Projects.DoesPlayerHaveProject(id))
        {
            Debug.Log("Something went wrong. You're opening a project that you don't have.");
            return;
        }

        gameInstance.Project = gameInstance.playerData.Value.Projects.GetPlayerProjectByName(id);
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
        OnQuitBtnClickedEvent?.Invoke(this, EventArgs.Empty);
    }
}