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

                if (conveyorBelt == null) return null;
            }

            return conveyorBelt;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        O_Build.OnBuildDestroyed += CheckConveyor;
    }

    private void CheckConveyor(object sender, System.EventArgs e)
    {
        O_Build_ConveyorBelt conveyorBelt = sender as O_Build_ConveyorBelt;
        if (conveyorBelt == null) return;

        if (ConveyorBelt == conveyorBelt)
        {
            Destroy(gameObject);
        }
    }

    public void SetSpline(SplineContainer splineContainer)
    {
        if (splineContainer == null) return;

        SplineAnimator.Container = splineContainer;
        splineAnimator.ElapsedTime = 0;
        splineAnimator.Play();
    }

    protected override void OnDestroy()
    {
        O_Build.OnBuildDestroyed -= CheckConveyor;

        base.OnDestroy();
    }
}