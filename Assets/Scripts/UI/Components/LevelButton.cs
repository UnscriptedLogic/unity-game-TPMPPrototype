using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI levelTMP;
    [SerializeField] private Button levelBtn;
    [SerializeField] private CanvasGroup canvasGroup;

    public void SetButton(int level, Camera camera, bool isActive, Action<int> OnClicked)
    {
        canvas.worldCamera = camera;

        levelBtn.interactable = isActive;
        canvasGroup.alpha = isActive ? 1 : 0.25f;

        levelTMP.text = (level + 1).ToString();
        levelBtn.onClick.AddListener(() =>
        {
            OnClicked?.Invoke(level);
        });
    }

    private void OnDestroy()
    {
        levelBtn.onClick.RemoveAllListeners();
    }
}
