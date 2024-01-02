using System;
using TMPro;
using UnityEngine;
using UnscriptedEngine;

public class ProjectUIBtn : UButtonComponent
{
    [SerializeField] private TextMeshProUGUI projectNameTMP;

    public TextMeshProUGUI ProjectNameTMP => projectNameTMP;

    public void Initialize(Project project, UCanvasController context)
    {
        projectNameTMP.text = project.Name;

        SetID(project.Name);
        InitializeUIComponent(context);
    }
}