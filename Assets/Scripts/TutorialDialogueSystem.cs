using System;
using System.Collections.Generic;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.UI;
using UnscriptedEngine;

public class TutorialDialogueSystem : ULevelObject
{
    [SerializeField] private UIC_DialogueUI dialogueUIPrefab;
    [SerializeField] private List<UIC_DialogueUI.Dialogue> dialogues;

    private GM_TutorialGameMode tutorialGameMode;
    private UIC_DialogueUI dialogueUI;

    protected override void OnLevelStarted()
    {
        base.OnLevelStarted();

        tutorialGameMode = GameMode.CastTo<GM_TutorialGameMode>();
        tutorialGameMode.OnSectionStarted += TutorialGameMode_OnSectionStarted;
        tutorialGameMode.OnLastSection += TutorialGameMode_OnLastSection;
    }

    private void TutorialGameMode_OnLastSection(object sender, EventArgs e)
    {
        dialogueUI = GameMode.GetPlayerController().AttachUIWidget(dialogueUIPrefab);
        dialogueUI.OnCharacterTyped += CheckDialogueBoxWidth;

        dialogueUI.StartDialogue(dialogues, dialogues.Count - 1, 0, OnDialogueCompleted);
    }

    private void CheckDialogueBoxWidth(object sender, UIC_DialogueUI.OnCharacterTypedEventArgs e)
    {
        if (e.typedOutText.Length >= 60)
        {
            dialogueUI.ContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
        else
        {
            dialogueUI.ContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }

    private void TutorialGameMode_OnSectionStarted(object sender, GM_TutorialGameMode.OnSectionCompeletedEventArgs e)
    {
        dialogueUI = GameMode.GetPlayerController().AttachUIWidget(dialogueUIPrefab);
        dialogueUI.OnCharacterTyped += CheckDialogueBoxWidth;

        dialogueUI.StartDialogue(dialogues, e.sectionIndex, 0, OnDialogueCompleted);
    }

    private void OnDialogueCompleted()
    {
        dialogueUI.OnCharacterTyped -= CheckDialogueBoxWidth;
        DettachUIWidget(dialogueUI.gameObject);
    }

    protected override void OnDestroy()
    {
        tutorialGameMode.OnSectionStarted -= TutorialGameMode_OnSectionStarted;
        tutorialGameMode.OnLastSection -= TutorialGameMode_OnLastSection;

        base.OnDestroy();
    }
}
