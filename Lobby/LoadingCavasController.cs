using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCavasController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Button cancelBtn;

    private NetworkRunnerController networkRunnerController;

    private void Start()
    {
        networkRunnerController = GlobalManagers.Instance.networkRunnerController;
        networkRunnerController.OnStartedRunnerConnection += OnStartedRunnerConnection;
        networkRunnerController.OnFinishRunnerConnection += OnFinishRunnerConnection;
        networkRunnerController.OnPlayerJoinedSucessfully += OnPlayerJoinedSucessfully;
        
        cancelBtn.onClick.AddListener(networkRunnerController.ShutDownRunner);
        
        this.gameObject.SetActive(false);
    }

    private void OnPlayerJoinedSucessfully()
    {
        this.gameObject.SetActive(true);
        const string CLIP_NAME = "Out";
        StartCoroutine(Utils.PlayAnimationAndSetStateWhenFinished(gameObject, animator, CLIP_NAME, false));
    }

    private void OnStartedRunnerConnection()
    {
        this.gameObject.SetActive(true);
    }

    private void OnFinishRunnerConnection()
    {
        networkRunnerController.OnStartedRunnerConnection -= OnStartedRunnerConnection;
        networkRunnerController.OnFinishRunnerConnection -= OnFinishRunnerConnection;
        this.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        networkRunnerController.OnStartedRunnerConnection -= OnStartedRunnerConnection;
        networkRunnerController.OnPlayerJoinedSucessfully -= OnPlayerJoinedSucessfully;
    }

    // Need to instantiate the LoadingCanvas
    
    
}
