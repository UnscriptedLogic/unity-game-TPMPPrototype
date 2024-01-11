using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnscriptedEngine;

public class P_PlayerPawn : URTSCamera
{
    [Header("Player Pawn Extension")]
    [SerializeField] private BuildListSO buildableDataSet;
    [SerializeField] private Vector2 panningDetectionThickness;

    [SerializeField] private GameObject selectionBoxPrefab;
    [SerializeField] private GameObject selectionObjectPrefab;

    private Vector3 startPosition;
    private Vector3 prevPosition;

    private O_Build objectToBuild;
    private BoxCollider2D selectionCollider;
    private bool isDraggingToSelect;

    private Dictionary<O_Build, GameObject> selectionDict = new Dictionary<O_Build, GameObject>();

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

    public void BeginDragToSelect(Vector3 position, bool keepSelection)
    {
        startPosition = position;

        if (!keepSelection)
        {
            for (int i = 0; i < selectionDict.Count; i++)
            {
                Destroy(selectionDict.ElementAt(i).Value);
            }

            selectionDict.Clear();
        }

        selectionCollider = Instantiate(selectionBoxPrefab, position, Quaternion.identity, transform).GetComponent<BoxCollider2D>();
        selectionCollider.transform.localScale = Vector3.zero;

        isDraggingToSelect = true;
    }

    public void UpdateDragToSelect(Vector3 position)
    {
        if (Vector3.Distance(position, prevPosition) <= 0.01f) return;

        selectionCollider.transform.position = CalculateMidPoint(startPosition, position);
        selectionCollider.transform.localScale = CalculateScale(startPosition, position);

        prevPosition = position;
    }

    private Vector3 CalculateScale(Vector3 startPosition, Vector3 position)
    {
        float x = position.x - startPosition.x;
        float y = position.y - startPosition.y;
        return new Vector3(x, y, 1f);
    }

    private Vector3 CalculateMidPoint(Vector3 startPosition, Vector3 position)
    {
        Vector3 dir = (position - startPosition).normalized;
        float distance = Vector3.Distance(position, startPosition);

        return startPosition + (dir * (distance * 0.5f));
    }

    public void EndDragToSelect(Vector3 position)
    {
        isDraggingToSelect = false;

        if (selectionCollider != null)
        {
            Destroy(selectionCollider.gameObject);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        O_Build build = collision.GetComponent<O_Build>();
        if (build != null)
        {
            if (selectionDict.ContainsKey(build)) return;

            GameObject selectionObject = Instantiate(selectionObjectPrefab, build.transform.position, Quaternion.identity, build.transform);
            selectionObject.transform.localScale = new Vector3(build.CellSize.x * 1.5f, build.CellSize.y * 1.5f);

            selectionDict.Add(build, selectionObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isDraggingToSelect) return;

        O_Build build = collision.GetComponent<O_Build>();
        if (build != null)
        {
            if (!selectionDict.ContainsKey(build)) return;

            Destroy(selectionDict[build]);

            selectionDict.Remove(build);
        }
    }

    public void DeleteAllSelected()
    {
        for (int i = 0; i < selectionDict.Count; i++)
        {
            selectionDict.ElementAt(i).Key.DeleteSelf();
        }

        selectionDict.Clear();
    }
}