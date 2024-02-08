using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RefreshButton : MonoBehaviour
{
    private Button refreshButton;

    private void Awake()
    {
        if(refreshButton == null)
        {
            refreshButton = GetComponent<Button>();
        }
        refreshButton.onClick.AddListener(Refresh);
        GlobalManagers.Instance.networkRunnerController.RefreshSessionListUI();
    }

    private void Refresh()
    {
        StartCoroutine(RefreshWait());
    }

    private IEnumerator RefreshWait()
    {
        refreshButton.interactable = false;
        GlobalManagers.Instance.networkRunnerController.RefreshSessionListUI();

        yield return new WaitForSeconds(3f);
        refreshButton.interactable = true;
    }
}
