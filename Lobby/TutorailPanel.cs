using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorailPanel : LobbyPanelBase
{
    // Buttons from UI 
    [SerializeField] private Button nextBtn;
    [SerializeField] private Button previousBtn;
    
    // Image
    [SerializeField] private Image image;
    
    // Addtional resources from Editer
    [SerializeField] private Sprite[] tutorialSprites;
    
    // Variable to count
    private int tutorialImgCount = 0;
    private int MAX_NUMBER_OF_COUNT = 5;
    private int MIN_NUMBER_OF_COUNT = 0;
     
    public override void InitPanel(LobbyUIManager uiManager)
    {
        base.InitPanel(uiManager);
        nextBtn.onClick.AddListener(() => OnNextPanel());
        previousBtn.onClick.AddListener(() => OnPreviousPanel());
    }

    private void OnNextPanel()
    {
        AudioManager.Instance.PlayButtonClip();
        tutorialImgCount++;

        if (tutorialImgCount == MAX_NUMBER_OF_COUNT)
        {
            // TutorialImgCount = MAX_NUMBER_OF_COUNT;
            base.ClosePanel();
            // Reset Tutorial back to 0
            tutorialImgCount = 0;
            image.sprite = tutorialSprites[tutorialImgCount];
        }
        else
        {
            LoadPage();
        }
    }

    private void OnPreviousPanel()
    {
        AudioManager.Instance.PlayButtonClip();
        Debug.Log($"Previous/{tutorialImgCount}");

        if (tutorialImgCount <= MIN_NUMBER_OF_COUNT)
        {
            tutorialImgCount = MIN_NUMBER_OF_COUNT;
        }
        else
        {
            tutorialImgCount--;
            LoadPage();
        }
    }

    private void LoadPage()
    {
        image.sprite = tutorialSprites[tutorialImgCount];
    }
}
