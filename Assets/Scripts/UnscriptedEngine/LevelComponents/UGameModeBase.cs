using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnscriptedEngine
{
    [DefaultExecutionOrder(-1)]
    public abstract class UGameModeBase : MonoBehaviour
    {
        public class LoadProcess
        {
            /// <summary>
            /// Name of the load process
            /// </summary>
            public string name;

            /// <summary>
            /// The progress of the load process
            /// </summary>
            public float progress;

            public IEnumerator process;

            /// <summary>
            /// Has the current progress reached 100? Used for detecting when to 
            /// proceed loading the next process in GameModeBase
            /// </summary>
            public bool IsDone => progress == 100f;

            /// <summary>
            /// Marks the current load process as completed and will continue if
            /// there are any other processes to load in the GameModeBase
            /// </summary>
            public void Completed() => progress = 100f;
        }

        [SerializeField] protected InputActionAsset inputContext;

        protected List<LoadProcess> loadProcesses;

        public event EventHandler OnPause;
        public event EventHandler OnResume;

        public event EventHandler OnLevelStarted;
        public event EventHandler OnLevelFinished;

        /// <summary>
        /// Event for when the game mode has been initialized and is ready to start loading the level
        /// </summary>
        public event EventHandler OnGameModeInitialized;

        public static UGameModeBase instance { get; private set; }

        public InputActionAsset InputContext => inputContext;

        protected void Awake()
        {
            instance = this;

            inputContext.Enable();

            loadProcesses = new List<LoadProcess>();
        }

        protected virtual IEnumerator Start()
        {
            OnGameModeInitialized?.Invoke(this, EventArgs.Empty);

            yield return StartCoroutine(LoadLevel());

            OnLevelStarted?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void Update() { }

        protected virtual void OnDisable()
        {
            OnLevelFinished?.Invoke(this, EventArgs.Empty);
        }

        internal virtual void PauseGame()
        {
            OnPause?.Invoke(this, EventArgs.Empty);
        }

        internal virtual void ResumeGame()
        {
            OnResume?.Invoke(this, EventArgs.Empty);
        }

        private void OnDestroy()
        {
            inputContext.Disable();
        }

        public void AddLoadingProcess(LoadProcess loadProcess)
        {
            if (loadProcess.process == null) return;

            loadProcesses.Add(loadProcess);
        }

        private IEnumerator LoadLevel()
        {
            for (int i = 0; i < loadProcesses.Count; i++)
            {
                LoadProcess loadprocess = loadProcesses[i];
                StartCoroutine(loadprocess.process);

                while (!loadprocess.IsDone)
                {
                    yield return null;
                }
            }
        }
    }
}