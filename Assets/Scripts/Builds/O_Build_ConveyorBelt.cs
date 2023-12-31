using System.Linq;
using UnityEngine;
using UnityEngine.Splines;
using UnscriptedEngine;

public class O_Build_ConveyorBelt : O_Build
{
    [SerializeField] private SplineInstantiate splineInstantiate;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private GameObject ui_hud;
    [SerializeField] private Transform endPoint;

    [SerializeField] private Transform startPointerAnchor;
    [SerializeField] private Transform endPointerAnchor;

    private UIC_ConveyorBeltHUD hud;
    private bool isBuildingStart;

    public SplineContainer ConveyorSplineContainer => splineContainer;

    public override void OnBeginPreview()
    {
        isBuildingStart = true;

        hud = AttachUIWidget(ui_hud) as UIC_ConveyorBeltHUD;
        hud.transform.SetParent(null);

        endPointerAnchor.gameObject.SetActive(false);
    }

    public override void OnUpdatePreview(Vector3 position, int rotationOffset)
    {
        if (isBuildingStart)
        {

            transform.position = position + new Vector3(0.5f, 0.5f, 0f);
            startPointerAnchor.rotation = Quaternion.Euler(Vector3.forward * (rotationOffset - 45));
        }
        else
        {
            endPoint.position = position + new Vector3(0.5f, 0.5f, 0);
            splineContainer.Spline.SetKnot(splineContainer.Spline.Knots.ToList().Count - 1, CreatePoint(position - transform.position + new Vector3(0.5f, 0.5f, 0), rotationOffset));

            endPointerAnchor.rotation = Quaternion.Euler(Vector3.forward * (rotationOffset - 45));
        }
    }

    public override void OnEndPreview()
    {
        if (hud != null)
        {
            DettachUIWidget(hud.gameObject);
        }
    }

    private static BezierKnot CreatePoint(Vector3 position, int rotation)
    {
        BezierKnot bezierKnot = new BezierKnot(position, .5f, .5f, Quaternion.Euler(0f, 0f, rotation));

        return bezierKnot;
    }

    public override bool CanBeBuilt()
    {
        return true;
    }

    protected override void OnPlayerStateChanged(C_PlayerController.PlayerState isBuildMode)
    {
        startPointerAnchor.gameObject.SetActive(isBuildMode == C_PlayerController.PlayerState.Building);
        endPointerAnchor.gameObject.SetActive(isBuildMode == C_PlayerController.PlayerState.Building);
    }

    public override void AlternateBuild(Vector3 position, int rotationOffset)
    {
        if (!isBuildingStart)
        {
            splineContainer.Spline.Add(CreatePoint(position - transform.position + new Vector3(0.5f, 0.5f, 0), rotationOffset), TangentMode.Linear, 2);
        }
    }

    public override void Build(Vector3 position, int rotationOffset)
    {
        if (isBuildingStart)
        {
            splineInstantiate.gameObject.SetActive(true);

            isBuildingStart = false;

            splineContainer.Spline.Add(CreatePoint(Vector3.zero, rotationOffset), TangentMode.Linear, 2);

            splineContainer.Spline.Add(CreatePoint(position - transform.position + new Vector3(0.5f, 0.5f, 0), rotationOffset), TangentMode.Linear, 2);

            endPointerAnchor.gameObject.SetActive(true);
        }
        else
        {
            isBuildingStart = true;

            Instantiate(gameObject);

            splineContainer.Spline.Clear();

            endPoint.localPosition = Vector3.zero;

            endPointerAnchor.gameObject.SetActive(false);

            splineInstantiate.Clear();
            splineInstantiate.gameObject.SetActive(false);
        }
    }
}