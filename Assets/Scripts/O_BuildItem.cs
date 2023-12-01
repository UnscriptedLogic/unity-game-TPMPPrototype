using UnityEngine;
using UnityEngine.Splines;
using UnscriptedEngine;

[RequireComponent(typeof(SplineAnimate))]
public class O_BuildItem : ULevelObject
{
    [SerializeField] private FrameworkCategory category = FrameworkCategory.VANILLA;
    [SerializeField] private string id;

    private SplineAnimate splineAnimator;
    private O_Build_ConveyorBelt conveyorBelt;

    public string ID => id;
    public FrameworkCategory Category => category;

    protected override void Awake()
    {
        base.Awake();
    }

    public SplineAnimate SplineAnimator
    {
        get
        {
            if (splineAnimator == null)
            {
                splineAnimator = GetComponent<SplineAnimate>();
            }

            return splineAnimator;
        }
    }

    public O_Build_ConveyorBelt ConveyorBelt
    {
        get
        {
            if (conveyorBelt == null)
            {
                conveyorBelt = SplineAnimator.Container.GetComponent<O_Build_ConveyorBelt>();
            }

            return conveyorBelt;
        }
    }

    public void SetSpline(SplineContainer splineContainer)
    {
        SplineAnimator.Container = splineContainer;
        splineAnimator.Play();
    }
}