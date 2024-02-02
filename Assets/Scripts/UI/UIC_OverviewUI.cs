using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnscriptedEngine;

public class UIC_OverviewUI : UCanvasController
{
    [SerializeField] private GameObject lessonBtnPrefab;
    [SerializeField] private Transform lessonBtnParent;
    [SerializeField] private Image thumbnailImg;

    private int selectedLevelIndex = 0;
    private List<Project> levels;

    private UTextComponent title;
    private UTextComponent description;
    private GI_CustomGameInstance customGameInstance;

    public override void OnWidgetAttached(ULevelObject context)
    {
        base.OnWidgetAttached(context);

        customGameInstance = GameMode.GameInstance.CastTo<GI_CustomGameInstance>();

        levels = new List<Project>(customGameInstance.Levels);

        InstantiateLessonBtns();

        title = GetUIComponent<UTextComponent>("title");
        description = GetUIComponent<UTextComponent>("desc");

        Bind<UButtonComponent>("play", OnPlayPressed);
        Bind<UButtonComponent>("back", OnBackPressed);

        OnLevelPressed(customGameInstance.LevelToLoad.ToString());
    }

    private void InstantiateLessonBtns()
    {
        for (int i = 0; i < levels.Count; i++)
        {
            GameObject levelButton = Instantiate(lessonBtnPrefab, lessonBtnParent);
            UButtonComponent uButtonComponent = levelButton.GetComponent<UButtonComponent>();
            uButtonComponent.SetID(i.ToString());
            uButtonComponent.InitializeUIComponent(this);

            if (i > 0)
            {
                uButtonComponent.TMPButton.interactable = levels[i].IsCompleted || levels[i - 1].IsCompleted;
            }

            UTextComponent uTextComponent = levelButton.GetComponentInChildren<UTextComponent>();
            uTextComponent.InitializeUIComponent(this);
            uTextComponent.TMP.text = $"Level {i + 1}";

            Bind<UButtonComponent>(i.ToString(), OnLevelPressed);
        }
    }

    private void OnPlayPressed()
    {
        customGameInstance.SetProjectToLoad(selectedLevelIndex);

        string levelName = $"Level{selectedLevelIndex + 1}";
        if (SceneUtility.GetBuildIndexByScenePath($"Scenes/{levelName}") != -1)
        {
            SceneManager.LoadScene(levelName);
        }
        else
        {
            //Fallback to a generic level if no level specific scene is found
            GameMode.LoadScene(1);
        }
    }

    private void OnLevelPressed(string id)
    {
        int index = 0;
        int.TryParse(id, out index);

        selectedLevelIndex = index;

        Project selectedLevel = levels[index];
        title.TMP.text = selectedLevel.Name;
        description.TMP.text = selectedLevel.Description;
        thumbnailImg.sprite = selectedLevel.Thumbnail;
    }

    private void OnBackPressed()
    {
        DettachUIWidget(gameObject);
    }
}