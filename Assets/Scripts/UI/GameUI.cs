using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameUI : MonoBehaviour
{
    //[System.NonSerialized] Health health = null;

    [Header("Inventory")]
    public GameObject gameMenuGroup;
    public GameObject inventoryGroup;
    //public GameObject leftArmItem;
    public GameObject[] mainItem;

    public Vector2 SelectedCellSize;
    public Vector2 defaultCellSize;

    [Header("Animation")]
    public float alphaChannelTime = 1f;
    public float animTime = 1f;

    [Header("HP bar")]
    public GameObject hp_bar;
    public GameObject hp_bar_behind;
    public GameObject hp_bar_heal;
    private Image hp_bar_img;
    private Image hp_bar_back_img;
    private Image hp_bar_heal_img;
    public float hpTime;
    //public float perToHealthFactor; // множитель относительно 100 хп, нужен чтобы хп соответствовало размеру в пикселях (100hp на 200px это perToHealthFactor = 2)
    // оно тебе не надо, считай в процентах от Health.MaxHealth


    [Header("Pause")]
    public GameObject pausePanel;
    public GameObject pauseButton;
    private float inventoryPositionY;
    private float menuGruopPositionY;

    public Action<int> onCellClick = delegate { };
    public Action<int> onItemDrop = delegate { };
    public Action<int, int> onItemSwap = delegate { };

    void Start()
    {
        //FindObjects();
        SetInitialState();
        PauseControl.Unpause();
        inventoryPositionY = inventoryGroup.transform.localPosition.y;
        menuGruopPositionY = gameMenuGroup.transform.localPosition.y;

        hp_bar_img = hp_bar.GetComponent<Image>();
        hp_bar_back_img = hp_bar_behind.GetComponent<Image>();
        hp_bar_heal_img = hp_bar_heal.GetComponent<Image>();

        var mat1 = Instantiate(hp_bar_img.material);
        mat1.SetFloat("_level", 1);
        hp_bar_img.material = mat1;

        var mat2 = Instantiate(hp_bar_back_img.material);
        mat2.SetFloat("_level", 1);
        hp_bar_back_img.material = mat2;

        var mat3 = Instantiate(hp_bar_heal_img.material);
        mat3.SetFloat("_botEdge", 0);
        mat3.SetFloat("_topEdge", 0);
        hp_bar_heal_img.material = mat3;
        //hp_bar_heal.SetActive(false);
    }

  //  private void FindObjects() {
  //      // здесь была написана хуета поэтому я ее отключил
  //      pausePanel = GameObject.Find("PausePanel");
        //pauseControl = GameObject.Find("GameManager").GetComponent<PauseControl>();
        //pauseButton = GameObject.Find("PauseButton");
  //  }

    private void SetInitialState()
    {
        setActiveRightCell(0);
        pausePanel.GetComponent<Image>().DOColor(new Color32(0, 0, 0, 0), 0);
        pausePanel.SetActive(false);
        gameMenuGroup.transform.DOLocalMoveY(menuGruopPositionY + 1200, 0).SetUpdate(true);
    }
    public void NextScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    /// <summary>
    /// Делает выбранную клетку больше и возвращает осатальные к обычному размеру
    /// </summary>
    /// <param name="n"></param>
    public void setActiveRightCell(int n) {
        for (int i = 0; i < mainItem.Length; i++) {
            if (i == n)
                mainItem[n].GetComponent<RectTransform>().sizeDelta = SelectedCellSize;
            else
                mainItem[i].GetComponent<RectTransform>().sizeDelta = defaultCellSize;
        }
    }

    /// <summary>
    /// Используется чтобы вызвать событие переключения слота
    /// </summary>
    /// <param name="n">Номер выбранного слота</param>
    public void SelectCell (int n) {
        onCellClick(n);
    }

    public void SetPause() { // edited
        if (PauseControl.isPaused) {
            PauseControl.Unpause();
            HidePauseMenu();
        } else {
            PauseControl.Pause();
            ShowPauseMenu();
        }
    }

    public void ShowPauseMenu()
    {
        pausePanel.SetActive(true);
        pausePanel.GetComponent<Image>().DOColor(new Color32(0, 0, 0, 100), alphaChannelTime).SetUpdate(true);
        inventoryGroup.transform.DOLocalMoveY(inventoryPositionY - 300, animTime).SetUpdate(true);
        gameMenuGroup.transform.DOLocalMoveY(menuGruopPositionY, animTime).SetEase(Ease.OutCubic).SetUpdate(true);
        pauseButton.SetActive(false);
    }

    public void HidePauseMenu()
    {
        gameMenuGroup.transform.DOLocalMoveY(menuGruopPositionY + 1200, animTime * 0.5f /*или поставить 0?*/).SetUpdate(true);
        inventoryGroup.transform.DOLocalMoveY(inventoryPositionY, animTime).SetUpdate(true);
        pausePanel.GetComponent<Image>().DOColor(new Color32(0, 0, 0, 0), alphaChannelTime);
        pausePanel.SetActive(false);
        pauseButton.SetActive(true);
        
    }

    //public void HealthScale(float percent)
    //{
    //    //percent = -percent;
    //    if (percent > 100) percent = 100;
    //    if (percent < 0) percent = 0;
    //    percent -= 100;
    //    percent *= perToHealthFactor;
    //    hp_bar.GetComponent<RectTransform>().offsetMax = new Vector2(hp_bar.GetComponent<RectTransform>().offsetMax.x, percent);
    //    hp_bar_behind.GetComponent<RectTransform>().DOSizeDelta(new Vector2(hp_bar_behind.GetComponent<RectTransform>().offsetMax.x, percent), hpTime).SetEase(Ease.InQuart).SetUpdate(true);
    //}

    float oldHP = 1f;
    public void updateHpBar (float percent) { //то же самое, но по-человечески, универсальнее и понятнее
        var mat = hp_bar_img.material;
        mat.SetFloat("_level", percent);
        hp_bar_img.material = mat;
        if (percent < oldHP) {
            StartCoroutine(changeBackMatVal(oldHP, percent));
        } else {
            StartCoroutine(healHPBarAnim(oldHP, percent));
        }
        oldHP = percent;
    }
    private IEnumerator changeBackMatVal(float oldVal, float newVal) {
        var mat = hp_bar_back_img.material;
        mat.SetFloat("_level", oldVal);
        hp_bar_back_img.material = mat;
        float startTime = Time.unscaledTime;

        while (startTime + hpTime > Time.unscaledTime) {
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            float dt = (Time.unscaledTime - startTime) / hpTime;
            float val = Mathf.Lerp(oldVal, newVal, dt * dt);
            mat.SetFloat("_level", val);
            hp_bar_back_img.material = mat;
        }

        mat.SetFloat("_level", newVal);
        hp_bar_back_img.material = mat;
    }
    private IEnumerator healHPBarAnim(float botEdge, float topEdge) {
        var mat = hp_bar_heal_img.material;
        float startTime = Time.unscaledTime;
        mat.SetFloat("_topEdge", topEdge);
        //hp_bar_heal.SetActive(true);

        while (startTime + hpTime > Time.unscaledTime) {
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            float dt = (Time.unscaledTime - startTime) / hpTime;
            float val = Mathf.Lerp(botEdge, topEdge, dt * dt);
            mat.SetFloat("_botEdge", val);
            hp_bar_heal_img.material = mat;
        }

        mat.SetFloat("_botEdge", topEdge);
        hp_bar_heal_img.material = mat;
        //hp_bar_heal.SetActive(false);
    }
    
    public void updateInventorySprite (int n, Sprite sprite) {
        updateSpriteOnCell(mainItem[n], sprite);
    }

    //public void updateLeftHandSprite (Sprite sprite) {
    //    updateSpriteOnCell(leftArmItem, sprite);
    //}

    private void updateSpriteOnCell (GameObject cell, Sprite sprite) {
        var img = cell.transform.GetChild(0).GetComponent<Image>();
        if (sprite != null) {
            img.gameObject.SetActive(true);
        } else {
            img.gameObject.SetActive(false);
        }
        img.sprite = sprite;
    }

    public void initItemDrop (int n) {
        onItemDrop(n);
	}

    public void initItemSwap (int i1, int i2) {
        onItemSwap(i1, i2);
    }
}
