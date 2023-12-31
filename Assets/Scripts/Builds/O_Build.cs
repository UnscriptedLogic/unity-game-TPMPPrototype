using System;
using UnityEngine;
using UnscriptedEngine;

public abstract class O_Build : ULevelObject
{
    [System.Serializable]
    public struct InputNode
    {
        [SerializeField] private Transform transform;

        private bool isConnected;
        private Collider2D collider;
        private O_Build_ConveyorBelt conveyorBelt;

        public Transform Transform => transform;
        public O_Build_ConveyorBelt ConveyorBelt => conveyorBelt;
        public bool IsConnected
        {
            get { return isConnected; }
        }

        public void Initialize()
        {
            collider = transform.GetComponent<Collider2D>();
        }

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

            collider.enabled = !isConnected;
        }

        public bool TryGetBuildItem(out O_BuildItem buildItem)
        {
            if (!isConnected)
            {
                buildItem = null;
                return false;
            }

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
            for (int i = 0; i < colliders.Length; i++)
            {
                O_BuildItem item = colliders[i].GetComponent<O_BuildItem>();
                if (item == null) continue;
                
                O_Build_ConveyorBelt itemConveyorBelt = item.SplineAnimator.Container.GetComponent<O_Build_ConveyorBelt>();
                if (itemConveyorBelt == null) continue;
                if (itemConveyorBelt != conveyorBelt) continue;

                buildItem = item;
                return true;
            }

            buildItem = null;
            return false;
        }

        public bool TryGetBuildComponent<T>(out T buildItem) where T : O_BuildItem
        {
            if (!isConnected)
            {
                buildItem = default;
                return false;
            }

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
            for (int i = 0; i < colliders.Length; i++)
            {
                T item = colliders[i].GetComponent<T>();
                if (item == null) continue;

                O_Build_ConveyorBelt itemConveyorBelt = item.SplineAnimator.Container.GetComponent<O_Build_ConveyorBelt>();
                if (itemConveyorBelt == null) continue;
                if (itemConveyorBelt != conveyorBelt) continue;

                buildItem = item;
                return true;
            }

            buildItem = null;
            return false;
        }
    }

    [System.Serializable]
    public struct OutputNode
    {
        [SerializeField] private Transform transform;

        private bool isConnected;
        private BoxCollider2D collider;
        private O_Build_ConveyorBelt conveyorBelt;

        public Transform Transform => transform;
        public O_Build_ConveyorBelt ConveyorBelt => conveyorBelt;
        public bool IsConnected
        {
            get { return isConnected; }
            set { isConnected = value; }
        }

        public void Initialize()
        {
            collider = transform.GetComponent<BoxCollider2D>();
        }

        public void CheckConnection()
        {
            isConnected = false;
            
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.25f);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (!colliders[i].CompareTag(START_CONSTRUCT_POINT)) continue;

                O_Build_ConveyorBelt conveyorBelt = colliders[i].GetComponentInParent<O_Build_ConveyorBelt>();
                if (!colliders[i].GetComponentInParent<O_Build_ConveyorBelt>()) continue;

                IsConnected = true;
                this.conveyorBelt = conveyorBelt;
                break;
            }

            collider.enabled = !isConnected;
        }
    }

    public const string START_CONSTRUCT_POINT = "StartConstructPoint";
    public const string END_CONSTRUCT_POINT = "EndConstructPoint";

    [SerializeField] protected Vector2 cellSize = new Vector2(0.9f, 0.9f);
    [SerializeField] protected Vector2 offset;

    protected GM_LevelManager levelManager;
    protected C_PlayerController.PlayerState playerState;

    public static event EventHandler OnBuildCreated;
    public static event EventHandler OnBuildDestroyed;

    protected virtual void Start()
    {
        levelManager = GameMode as GM_LevelManager;

        levelManager.GetPlayerController().CastTo<C_PlayerController>().playerState.OnValueChanged += OnPlayerStateChanged;

        OnBuildCreated?.Invoke(this, EventArgs.Empty);

        levelManager.NodeTickSystem.OnTick += NodeTickSystem_OnTick;
    }

    protected virtual void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e)
    {

    }

    public virtual void OnBeginPreview() 
    { 
    
    }
    
    public virtual void OnUpdatePreview(Vector3 position, int rotationOffset) 
    {
        transform.position = Vector3.Lerp(transform.position, position + new Vector3(0.5f, 0.5f, 0f), 0.5f);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, rotationOffset), 0.5f);
    }

    public virtual void OnEndPreview() 
    {  
    
    }

    public virtual bool CanBeBuilt()
    {
        if (!IsOverlapping())
        {
            return false;
        }

        return true;
    }

    public virtual void Build(Vector3 position, int rotationOffset)
    {
        O_Build build = Instantiate(gameObject).GetComponent<O_Build>();
        build.OnBuilt();
    }

    public virtual void OnBuilt()
    {

    }

    public virtual void AlternateBuild(Vector3 position, int rotationOffset)
    {

    }

    protected virtual void OnPlayerStateChanged(C_PlayerController.PlayerState playerState)
    {
        this.playerState = playerState;
    }

    public bool IsOverlapping()
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
        levelManager.NodeTickSystem.OnTick -= NodeTickSystem_OnTick;

        if (levelManager.GetPlayerController().CastTo<C_PlayerController>() != null)
        {
            levelManager.GetPlayerController().CastTo<C_PlayerController>().playerState.OnValueChanged -= OnPlayerStateChanged;
        }

        OnBuildDestroyed?.Invoke(this, EventArgs.Empty);

        base.OnDestroy();
    }

    public virtual void DeleteSelf()
    {
        Destroy(gameObject);
    }
}
