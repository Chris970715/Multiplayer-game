using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainGameUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI killText;

    private void Awake()
    {
        killText.enabled = false;
    }

    public void ShowKillTextPanel(int killNumbers)
    {
        killText.enabled = true;
        killText.text = "Kills " + killNumbers;
        StartCoroutine(DisableKillTextPanel());
    }

    private IEnumerator DisableKillTextPanel()
    {
        yield return new WaitForSeconds(2f);
        killText.enabled = false;
    }
}
