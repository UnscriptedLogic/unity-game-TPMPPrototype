using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnscriptedEngine;

public class UIC_TestBuildHUD : UIC_BuildHUD
{
    protected override void ShowFrameworkButtons()
    {
        for (int i = 0; i < customGameInstance.AllFrameworks.Count; i++)
        {
            Framework framework = customGameInstance.AllFrameworks[i];

            //Framework button
            UButtonComponent frameworkBtn = Instantiate(frameworkBtnPrefab, buildBtnsParent).GetComponent<UButtonComponent>();
            frameworkBtn.GetComponentsInChildren<Image>()[1].sprite = framework.Details.FrameworkIcon;
            frameworkBtn.SetID(framework.Details.FrameworkName);
            frameworkBtn.GetComponentInChildren<TextMeshProUGUI>().text = framework.Details.FrameworkName;
            frameworkBtn.InitializeUIComponent(this);

            Bind<UButtonComponent>(framework.Details.FrameworkName, ShowFrameworkBuilds);
        }
    }

    protected override Framework GetFrameworkByID(string frameworkName)
    {
        for (int i = 0; i < customGameInstance.AllFrameworks.Count; i++)
        {
            if (customGameInstance.AllFrameworks[i].Details.FrameworkName != frameworkName) continue;

            return customGameInstance.AllFrameworks[i];
        }

        Debug.LogWarning($"Something hella sus went wrong here. Tried to get id of {frameworkName}");
        return null;
    }
}