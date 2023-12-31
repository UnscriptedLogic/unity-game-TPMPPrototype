using System;
using Unity.VisualScripting;
using UnityEngine;
using UnscriptedEngine;

public class P_PlayerPawn : URTSCamera
{
    [Header("Player Pawn Extension")]
    [SerializeField] private SO_Builds buildableDataSet;

    [SerializeField] private Vector2 panningDetectionThickness;

    private O_Build objectToBuild;

    public void StartBuildPreview(string buildID)
    {
        ClearPreview();
        objectToBuild = Instantiate(buildableDataSet.GetBuildableWithID(buildID).Build);

        objectToBuild.OnBeginPreview();
    }

    public void UpdateBuildPreview(Vector3 position, int rotationOffset)
    {
        objectToBuild.OnUpdatePreview(position, rotationOffset);
    }

    public void AttemptBuild(Vector3 position, int rotationOffset)
    {
        if (objectToBuild.CanBeBuilt())
        {
            objectToBuild.Build(position, rotationOffset);
        }
    }

    public void AttemptAlternateBuild(Vector3 position, int rotationOffset)
    {
        objectToBuild.AlternateBuild(position, rotationOffset);
    }

    public void ClearPreview()
    {
        if (objectToBuild != null)
        {
            Destroy(objectToBuild.gameObject);
        }
    }

    public void EndBuildPreview()
    {
        if (objectToBuild == null) return;

        ClearPreview();
        objectToBuild.OnEndPreview();
    }

    public override void MoveCamera(Direction direction)
    {
        Vector3 pos = anchor.position;

        switch (direction)
        {
            case Direction.Forward:
                pos += anchor.up * panSpeed * Time.deltaTime;
                break;
            case Direction.Backward:
                pos += -anchor.up * panSpeed * Time.deltaTime;
                break;
            case Direction.Left:
                pos += -anchor.right * panSpeed * Time.deltaTime;
                break;
            case Direction.Right:
                pos += anchor.right * panSpeed * Time.deltaTime;
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
}