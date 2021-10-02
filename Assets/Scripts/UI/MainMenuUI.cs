using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenuUI : MonoBehaviour
{
    public float movementSpeed, alphaChannelSpeed = 1f;
    private Transform buttonGroup;
    private Transform backToMainMeneuButton;
    private GameObject exitPanel;
    private float groupPositionX, backButtonX;
    //private bool hideTrigger = true;



    public void HideButton()
    {
        //hideTrigger = false;
        buttonGroup.DOLocalMoveX(groupPositionX + 700, movementSpeed).SetEase(Ease.OutQuint).SetUpdate(true);
    }

    public void ShowButton()
    {
        //hideTrigger = true;
        buttonGroup.DOLocalMoveX(groupPositionX, movementSpeed).SetEase(Ease.OutQuint).SetUpdate(true);
        backToMainMeneuButton.DOLocalMoveX(backButtonX-700, movementSpeed).SetEase(Ease.OutQuint).SetUpdate(true);
    }

    //public void GroupMoveControl()
    //{
    //    //hideTrigger = !hideTrigger;
    //    if (hideTrigger == true) HideButton();
    //    else ShowButton();
    //}

    public void ShowLeftButtons()
    {
        backToMainMeneuButton.DOLocalMoveX(backButtonX, movementSpeed).SetEase(Ease.OutQuint).SetUpdate(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    public void NextScene (string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }


    public void ShowExitGroup()
    {   
        exitPanel.SetActive(true);
        exitPanel.GetComponent<Image>().DOColor(new Color32(0, 0, 0, 120), alphaChannelSpeed);
        exitPanel.transform.Find("ExitBackground").GetComponent<Image>().DOColor(new Color32(255, 255, 255, 255), alphaChannelSpeed);
        exitPanel.transform.Find("ExitBackground").Find("Text").GetComponent<Text>().DOColor(new Color32(0, 0, 0, 255), alphaChannelSpeed);
        exitPanel.transform.Find("ExitBackground").Find("YesButton").GetComponent<Image>().DOColor(new Color32(255, 255, 255, 255), alphaChannelSpeed);
        exitPanel.transform.Find("ExitBackground").Find("NoButton").GetComponent<Image>().DOColor(new Color32(255, 255, 255, 255), alphaChannelSpeed);
        exitPanel.transform.Find("ExitBackground").Find("NoButton").Find("Text").GetComponent<Text>().DOColor(new Color32(0, 0, 0, 255), alphaChannelSpeed);
        exitPanel.transform.Find("ExitBackground").Find("YesButton").Find("Text").GetComponent<Text>().DOColor(new Color32(0, 0, 0, 255), alphaChannelSpeed);
        
    }

    public void HideExitGroup()
    {
        exitPanel.GetComponent<Image>().DOColor(new Color32(0,0,0,0), 0);
        exitPanel.transform.Find("ExitBackground").GetComponent<Image>().DOColor(new Color32(255, 255, 255, 0), 0);
        exitPanel.transform.Find("ExitBackground").Find("Text").GetComponent<Text>().DOColor(new Color32(0, 0, 0, 0), 0);
        exitPanel.transform.Find("ExitBackground").Find("YesButton").GetComponent<Image>().DOColor(new Color32(255, 255, 255, 0), 0);
        exitPanel.transform.Find("ExitBackground").Find("NoButton").GetComponent<Image>().DOColor(new Color32(255, 255, 255, 0), 0);
        exitPanel.transform.Find("ExitBackground").Find("NoButton").Find("Text").GetComponent<Text>().DOColor(new Color32(0, 0, 0, 0), 0);
        exitPanel.transform.Find("ExitBackground").Find("YesButton").Find("Text").GetComponent<Text>().DOColor(new Color32(0, 0, 0, 0), 0);
        exitPanel.SetActive(false);
    }

    private void FindObjects()
    {
        exitPanel = GameObject.Find("ExitPanel");
        buttonGroup = GameObject.Find("ButtonGroup").transform;
        groupPositionX = buttonGroup.localPosition.x;
        backToMainMeneuButton = GameObject.Find("BackToMainMenuButton").transform;
        backButtonX = backToMainMeneuButton.localPosition.x;
        backToMainMeneuButton.DOLocalMoveX(backButtonX - 700, 0).SetUpdate(true);
    }

    void Start()
    {
        FindObjects();
        HideExitGroup();
        Time.timeScale = 1;
    }

    void Update()
    {
        
    }
}
