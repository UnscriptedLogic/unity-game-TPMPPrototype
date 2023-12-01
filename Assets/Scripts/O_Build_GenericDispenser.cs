using System;
using UnityEngine;
using UnityEngine.Splines;
using UnscriptedEngine;

public class O_Build_GenericDispenser : O_Build
{
    [SerializeField] private GameObject depositOrientation;
    [SerializeField] private Transform depositTransform;
    [SerializeField] private O_BuildItem buildItemPrefab;

    [SerializeField] private float spawnInterval = 2f;
    private float _spawnInterval;

    private SplineContainer splineContainer;
    private bool isDepositting;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if (!isDepositting) return;

        if (_spawnInterval <= 0f)
        {
            _spawnInterval = spawnInterval;

            O_BuildItem buildItem = Instantiate(buildItemPrefab);
            buildItem.SetSpline(splineContainer);
        }
        else
        {
            _spawnInterval -= Time.deltaTime;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public override void OnBeginPreview()
    {
        depositOrientation.SetActive(true);
    }

    protected override void OnBuildModeChanged(C_PlayerController.PlayerState playerState)
    {
        base.OnBuildModeChanged(playerState);

        depositOrientation.SetActive(playerState == C_PlayerController.PlayerState.Building);

        if (playerState != C_PlayerController.PlayerState.Building)
        {
            if (!isDepositting)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(depositTransform.position, 0.25f);
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (!colliders[i].CompareTag(START_CONSTRUCT_POINT)) continue;

                    O_Build_ConveyorBelt conveyorBelt = colliders[i].GetComponentInParent<O_Build_ConveyorBelt>();
                    if (!colliders[i].GetComponentInParent<O_Build_ConveyorBelt>()) continue;

                    isDepositting = true;
                    splineContainer = conveyorBelt.ConveyorSplineContainer;
                    break;
                }
            }
        }
    }

    public override bool CanBeBuilt()
    {
        if (!IsOverlapping())
        {
            return false;
        }

        return true;
    }
}