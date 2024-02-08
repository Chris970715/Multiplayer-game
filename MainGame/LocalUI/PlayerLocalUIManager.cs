using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLocalUIManager : MonoBehaviour
{
    [SerializeField] private GameObject killPopUp;
    [SerializeField] private TextMeshProUGUI killPopUpText;

    private void Awake()
    {
        CloseKillPopUp();
    }
    
    

    public void OpenKillPopUp()
    {
        Utils.PlayOneShot(Utils.SfxTypes.GotAKill);
        killPopUp.SetActive(true);
        killPopUpText.text = $"Kills (+1)";
        StartCoroutine(FadeAwayPanel());
    }

    public void CloseKillPopUp()
    {
        killPopUp.SetActive(false);
    }

    private IEnumerator FadeAwayPanel()
    {
        yield return new WaitForSeconds(2f);
        CloseKillPopUp();
    }
}
