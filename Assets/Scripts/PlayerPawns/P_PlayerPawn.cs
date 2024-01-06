using DG.Tweening;
using System;
using UnityEngine;
using UnscriptedEngine;

public class P_PlayerPawn : URTSCamera
{
    [Header("Player Pawn Extension")]
    [SerializeField] private BuildListSO buildableDataSet;

    [SerializeField] private Vector2 panningDetectionThickness;

    private O_Build objectToBuild;

    public void StartBuildPreview(string buildID, Vector3 position)
    {
        EndBuildPreview();

        (int framework, int build) = buildableDataSet.GetBuildableFromID(buildID);
        objectToBuild = Instantiate(buildableDataSet.Frameworks[framework].DataSet[build].Build);
        objectToBuild.transform.position = position;

        objectToBuild.OnBeginPreview();
    }

    public void UpdateBuildPreview(Vector3 position, int rotationOffset)
    {
        objectToBuild.OnUpdatePreview(position, rotationOffset);
    }

    public void AttemptBuild(Vector3 position, int rotationOffset, bool keepBuilding)
    {
        if (objectToBuild.CanBeBuilt())
        {
            objectToBuild.Build(position, rotationOffset, keepBuilding);
        }
    }

    public void AttemptAlternateBuild(Vector3 position, int rotationOffset)
    {
        objectToBuild.AlternateBuild(position, rotationOffset);
    }

    public void EndBuildPreview()
    {
        if (objectToBuild == null) return;

        objectToBuild.OnEndPreview();
    }

    public override void MoveCamera(Direction direction)
    {
        Vector3 pos = anchor.position;

        switch (direction)
        {
            case Direction.Forward:
                pos += anchor.up * panSpeed * Time.unscaledDeltaTime;
                break;
            case Direction.Backward:
                pos += -anchor.up * panSpeed * Time.unscaledDeltaTime;
                break;
            case Direction.Left:
                pos += -anchor.right * panSpeed * Time.unscaledDeltaTime;
                break;
            case Direction.Right:
                pos += anchor.right * panSpeed * Time.unscaledDeltaTime;
                break;
            default:
                Debug.Log("Something went wrong here");
                break;
        }

        pos.x = Mathf.Clamp(pos.x, -bounds.x, bounds.x);
        pos.y = Mathf.Clamp(pos.y, -bounds.y, bounds.y);


        anchor.position = Vector3.Lerp(anchor.position, pos, smoothing);
    }

    public void MovePlayerCamera(Vector2 mousePos)
    {
        if (mousePos.x >= Screen.width - panningDetectionThickness.x)
        {
            MoveCamera(Direction.Right);
        }

        if (mousePos.x <= panningDetectionThickness.x)
        {
            MoveCamera(Direction.Left);
        }

        if (mousePos.y >= Screen.height - panningDetectionThickness.y)
        {
            MoveCamera(Direction.Forward);
        }

        if (mousePos.y <= panningDetectionThickness.y)
        {
            MoveCamera(Direction.Backward);
        }
    }

    public void MovePlayerCameraWASD(Vector2 direction)
    {
        Debug.Log(direction.magnitude);

        if (direction.magnitude <= 0f) return;

        //W/S -> y axis and A/D on the x axis
        if (direction.x > 0)
        {
            MoveCamera(Direction.Right);
        }
        else if (direction.x < 0)
        {
            MoveCamera(Direction.Left);
        }

        if (direction.y > 0)
        {
            MoveCamera(Direction.Forward);
        }
        else if (direction.y < 0)
        {
            MoveCamera(Direction.Backward);
        }
    }

    public void AttemptDelete(Vector3 mouseWorldPosition)
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(mouseWorldPosition, .25f);
        for (int i = 0; i < collider.Length; i++)
        {
            if (collider[i] == null) continue;

            O_Build build = collider[i].GetComponent<O_Build>();
            if (build == null)
            {
                build = collider[i].GetComponentInParent<O_Build>();
                if (build == null) continue;
            }

            build.DeleteSelf();

            break;
        }
    }

    public void MoveCameraToPosition(Vector3 position, Action OnComplete)
    {
        transform.DOMove(position, 1.5f).SetEase(Ease.InOutExpo).OnComplete(() => OnComplete());
    }
}