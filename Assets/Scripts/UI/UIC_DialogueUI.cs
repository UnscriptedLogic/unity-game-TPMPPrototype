using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnscriptedEngine;

public class UIC_DialogueUI : UCanvasController
{
    public class OnCharacterTypedEventArgs : EventArgs
    {
        public TextDialogue textDialogue;
        public string typedOutText;
    }

    [System.Serializable]
    public class TextDialogue
    {
        [TextArea(3, 5)] public string text;
        public UnityEvent OnEntered;
        public UnityEvent<TextDialogue, string> OnCharacter;
        public UnityEvent OnExited;
    }

    [System.Serializable]
    public class Dialogue
    {
        [SerializeField] private List<TextDialogue> texts;

        public List<TextDialogue> Texts => texts;
    }

    [SerializeField] private ContentSizeFitter contentSizeFitter;
    [SerializeField] private CanvasGroup canvasGroup;

    private List<Dialogue> dialogues;
    private int dialogueIndex;
    private int textIndex;
    private Action OnComplete;

    private UTextComponent dialogueText;
    private bool isNextPressed;

    public event EventHandler<OnCharacterTypedEventArgs> OnCharacterTyped;

    public UTextComponent DialogueText => dialogueText;
    public ContentSizeFitter ContentSizeFitter => contentSizeFitter;

    public override void OnWidgetAttached(ULevelObject context)
    {
        base.OnWidgetAttached(context);

        dialogueText = GetUIComponent<UTextComponent>("dialogue");

        Bind<UButtonComponent>("next", OnNextButtonPressed);

        canvasGroup.DOFade(1f, 0.5f).SetEase(Ease.OutSine);
    }

    private void OnNextButtonPressed()
    {
        isNextPressed = true;
    }

    public void StartDialogue(List<Dialogue> dialogues, int dialogueIndex, int textIndex = 0, Action OnComplete = null)
    {
        this.dialogues = dialogues;
        this.dialogueIndex = dialogueIndex;
        this.textIndex = textIndex;
        this.OnComplete = OnComplete;

        if (dialogueIndex >= dialogues.Count)
        {
            Debug.LogWarning($"Dialogue index: {dialogueIndex} is out of the range of dialoge list: {dialogues.Count}");
            return;
        }

        if (textIndex >= dialogues[dialogueIndex].Texts.Count)
        {
            Debug.LogWarning($"Text index: {textIndex} is out of the range of text list: {dialogues[dialogueIndex].Texts.Count}");
            return;
        }

        dialogues[dialogueIndex].Texts[textIndex].OnEntered?.Invoke();
        StartCoroutine(TypeOutEffect(dialogues[dialogueIndex].Texts[textIndex], dialogueText.TMP, () =>
        {
            dialogues[dialogueIndex].Texts[textIndex].OnExited?.Invoke();
        }));
    }

    public void TryShowNextText()
    {
        if (textIndex + 1 >= dialogues[dialogueIndex].Texts.Count)
        {
            this.OnComplete();
            return;
        }

        textIndex++;
        StartDialogue(dialogues, dialogueIndex, textIndex, OnComplete);
    }

    private IEnumerator TypeOutEffect(TextDialogue textDialogue, TextMeshProUGUI tmp, Action OnFinishedTyping)
    {
        string typedOutText = "";
        float delay = 0.025f;
        for (int i = 0; i < textDialogue.text.Length; i++)
        {
            if (isNextPressed)
            {
                if (typedOutText.Length < textDialogue.text.Length && i > 10)
                {
                    delay = 0.0005f;
                }

                isNextPressed = false;
            }

            typedOutText += textDialogue.text[i];
            tmp.text = typedOutText;

            textDialogue.OnCharacter?.Invoke(textDialogue, tmp.text);
            OnCharacterTyped?.Invoke(this, new OnCharacterTypedEventArgs()
            {
                textDialogue = textDialogue,
                typedOutText = typedOutText,
            });

            yield return new WaitForSeconds(delay);
        }

        OnFinishedTyping();

        yield return new WaitUntil(() => isNextPressed == true);

        isNextPressed = false;

        TryShowNextText();
    }
}