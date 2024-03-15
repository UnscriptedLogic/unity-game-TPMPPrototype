using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public abstract class O_Build : ULevelObject
{
    [System.Serializable]
    public class InputNode
    {
        [SerializeField] private Transform transform;
        [Min(1)]
        [SerializeField] private int maxCap = 1;

        private bool isConnected;
        private O_Build_ConveyorBelt conveyorBelt;
        
        public List<O_BuildItem> Inventory;

        public Transform Transform => transform;
        public O_Build_ConveyorBelt ConveyorBelt => conveyorBelt;
        public bool IsConnected => isConnected;
        public bool HasSpace => Inventory.Count < maxCap;
        public bool isInventoryEmpty => Inventory.Count == 0;

        public void CheckConnection()
        {
            isConnected = false;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.25f);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].transform == transform) continue;

                if (!colliders[i].CompareTag(END_CONSTRUCT_POINT)) continue;

                O_Build_ConveyorBelt conveyorBelt = colliders[i].GetComponentInParent<O_Build_ConveyorBelt>();
                if (conveyorBelt == null) continue;

                isConnected = true;
                this.conveyorBelt = conveyorBelt;
                break;
            }
        }

        public bool TryGetBuildItem(out O_BuildItem buildItem)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
            for (int i = 0; i < colliders.Length; i++)
            {
                O_BuildItem item = colliders[i].GetComponent<O_BuildItem>();
                if (item == null) continue;

                buildItem = item;
                return true;
            }

            buildItem = null;
            return false;
        }

        public bool TryGetBuildComponent<T>(out T buildItem) where T : O_BuildItem
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
            for (int i = 0; i < colliders.Length; i++)
            {
                T item = colliders[i].GetComponent<T>();
                if (item == null) continue;

                buildItem = item;
                return true;
            }

            buildItem = null;
            return false;
        }

        public void ClearNulls()
        {
            for (int i = Inventory.Count - 1; i >= 0; i--)
            {
                if (Inventory[i] == null)
                {
                    Inventory.RemoveAt(i);
                }
            }
        }
    }

    [System.Serializable]
    public struct OutputNode
    {
        [SerializeField] private Transform transform;

        private O_Build_ConveyorBelt conveyorBelt;

        public Transform Transform => transform;
        public O_Build_ConveyorBelt ConveyorBelt => conveyorBelt;
        public bool IsConnected => HasConveyorBelt() || IsBuildingInfront;

        public bool IsSpawnAreaEmpty
        {
            get
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].GetComponent<O_BuildItem>() != null)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public bool IsBuildingInfront
        {
            get
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.2f);
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].CompareTag(END_CONSTRUCT_POINT))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool HasConveyorBelt()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
            for (int i = 0; i < colliders.Length; i++)
            {
                conveyorBelt = colliders[i].GetComponent<O_Build_ConveyorBelt>();
                if (conveyorBelt != null)
                {
                    return true;
                }
            }

            conveyorBelt = null;
            return false;
        }

        public bool HasConveyorBelt(out O_Build_ConveyorBelt conveyorBelt)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
            for (int i = 0; i < colliders.Length; i++)
            {
                conveyorBelt = colliders[i].GetComponent<O_Build_ConveyorBelt>();
                if (conveyorBelt != null)
                {
                    return true;
                }
            }

            conveyorBelt = null;
            return false;
        }

        public O_Build GetBuildInfront()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.2f);
            for (int i = 0; i < colliders.Length; i++)
            {
                O_Build build = colliders[i].GetComponentInParent<O_Build>();
                if (build != null && build.transform != transform.GetComponentInParent<O_Build>().transform)
                {
                    return build;
                }
            }

            return null;
        }
    }

    public const string START_CONSTRUCT_POINT = "StartConstructPoint";
    public const string END_CONSTRUCT_POINT = "EndConstructPoint";

    protected int ticksLeft;
    protected bool inPreview;

    [SerializeField] protected bool indestructible;
    [SerializeField] protected bool movable = true;
    [SerializeField] protected Vector2 cellSize = new Vector2(0.9f, 0.9f);
    [SerializeField] protected Vector2 offset;

    protected List<O_BuildItem> inventory;

    protected IBuildSystem levelBuildInterface;
    protected IPlayerState playerStateUser;

    protected C_PlayerController.PlayerState playerState;

    public static event EventHandler OnBuildCreated;
    public static event EventHandler OnBuildDestroyed;
    public static event EventHandler OnObjectBuilt;
    public static event EventHandler OnPreviewingBuild;

    public Vector2 CellSize => cellSize;
    public List<O_BuildItem> Inventory => inventory;
    public bool Indestructible => indestructible;
    public bool IsMovable => movable;

    protected virtual void Start()
    {
        inventory = new List<O_BuildItem>();

        levelBuildInterface = GameMode as IBuildSystem;
        if (levelBuildInterface != null)
        {
            levelBuildInterface.NodeTickSystem.OnTick += NodeTickSystem_OnTick;
        }

        playerStateUser = GameMode.GetPlayerController<C_PlayerController>();
        if (playerStateUser != null)
        {
            playerStateUser.CurrentPlayerState.OnValueChanged += OnPlayerStateChanged;
        }

        OnBuildCreated?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e) 
    {
        if (ticksLeft > 0)
        {
            ticksLeft--;
        }
    }

    public virtual void OnBeginPreview() 
    {
        inPreview = true;

        OnPreviewingBuild?.Invoke(this, EventArgs.Empty);
    }
    
    public virtual void OnUpdatePreview(Vector3 position, int rotationOffset) 
    {
        transform.position = Vector3.Lerp(transform.position, position + new Vector3(0.5f, 0.5f, 0f), 0.5f);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, rotationOffset), 0.5f);
    }

    public virtual void OnEndPreview() 
    {
        inPreview = false;

        Destroy(gameObject);
    }

    public virtual bool CanBeBuilt()
    {
        return IsAreaEmpty();
    }

    public virtual void Build(Vector3 position, int rotationOffset, bool keepBuilding)
    {
        O_Build build = Instantiate(gameObject).GetComponent<O_Build>();
        build.OnBuilt();
        build.FireBuiltEvent();

        if (!keepBuilding)
        {
            OnEndPreview();
        }
    }

    public void FireBuiltEvent()
    {
        OnObjectBuilt?.Invoke(this, EventArgs.Empty);
    }

    public virtual void OnBuilt()
    {
        transform.localScale *= 0.75f;
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);
    }

    public virtual void AlternateBuild(Vector3 position, int rotationOffset)
    {

    }

    protected virtual void OnPlayerStateChanged(C_PlayerController.PlayerState playerState)
    {
        this.playerState = playerState;
    }

    public virtual bool IsAreaEmpty()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position + (Vector3)offset, cellSize, 0);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<O_Build>() && colliders[i].gameObject != gameObject)
            {
                return false;
            }
        }

        return true;
    }

    protected override void OnDestroy()
    {
        levelBuildInterface.NodeTickSystem.OnTick -= NodeTickSystem_OnTick;

        if (playerStateUser != null)
        {
            playerStateUser.CurrentPlayerState.OnValueChanged -= OnPlayerStateChanged;
        }

        OnBuildDestroyed?.Invoke(this, EventArgs.Empty);

        base.OnDestroy();
    }

    public virtual void DeleteSelf()
    {
        if (indestructible) return;

        Destroy(gameObject);
    }
}
