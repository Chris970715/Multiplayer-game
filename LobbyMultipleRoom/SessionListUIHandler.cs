using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

public class SessionListUIHandler : MonoBehaviour
{
    //public TextMeshProUGUI statusText;
    public GameObject sessionItemListPrefab;
    public VerticalLayoutGroup verticalLayoutGroup;

    private void Awake()
    {
        Clearlist();
    }

    public void Clearlist()
    {
        // Delete all children of the vertical layout group
        foreach (Transform chlid in verticalLayoutGroup.transform)
        {
            Destroy(chlid.gameObject);
        }
        
        //Hide the status message
        //statusText.gameObject.SetActive(false);
    }

    public void AddToList(SessionInfo sessionInfo)
    {
        // Add a new item to the list
        SessionInfoListUI addedSessionInfoListUI = Instantiate(sessionItemListPrefab, verticalLayoutGroup.transform)
            .GetComponent<SessionInfoListUI>();
        addedSessionInfoListUI.SetInformation(sessionInfo);
        
        //Hook up events
        //addedSessionInfoListUI.OnJoinSession += AddedSessionInfoListUIItem_OnJoinSession;

    }

    private void AddedSessionInfoListUIItem_OnJoinSession(SessionInfo obj)
    {
        //throw new System.NotImplementedException();
        
    }

    public void OnNoSessionFound()
    {
        //statusText.text = "No game session found";
        //statusText.gameObject.SetActive(true);
    }

    public void OnLookingForGameSession()
    {
        //statusText.text = "Looking for game sessions";
        //statusText.gameObject.SetActive(true);
    }
}
