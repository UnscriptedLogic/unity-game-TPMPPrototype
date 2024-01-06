using UnityEngine;
using UnityEngine.Video;
using UnscriptedEngine;

public class UIC_BuildingDetailsModal : UCanvasController
{
    [SerializeField] private BuildListSO builds;
    [SerializeField] private VideoPlayer functionalityVideoPlayer;

    private UTextComponent buildName;
    private UTextComponent buildDesc;

    public override void OnWidgetAttached(ULevelObject context)
    {
        base.OnWidgetAttached(context);

        buildName = GetUIComponent<UTextComponent>("name");
        buildDesc = GetUIComponent<UTextComponent>("desc");
    }

    public void InitializeModal(string id)
    {
        (int frameworkIndex, int buildIndex) = builds.GetBuildableFromID(id);
        SO_Builds.BuildableData build = builds.Frameworks[frameworkIndex].DataSet[buildIndex];

        buildName.TMP.text = build.DisplayName;
        buildDesc.TMP.text = build.Description;
        functionalityVideoPlayer.clip = build.FunctionalityVideo;
    }
}