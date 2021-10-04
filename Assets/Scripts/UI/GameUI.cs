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
    public ItemSlot[] InvSlots;
    public Inventory PlayerInventory;
    public GameObject ItemHolderPrefab;

    public Vector2 SelectedCellSize;
    public Vector2 defaultCellSize;

    public ItemInfo ItemInfoObj;

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


    //[Header("Pause")]
    //public GameObject pausePanel;
    //public GameObject pauseButton;
    private float inventoryPositionY;
    private float menuGruopPositionY;

    public Action<int> onCellClick = delegate { };
    public Action<int> onItemDrop = delegate { };
    public Action<int, int> onItemSwap = delegate { };

    void Start()
    {
        SetInitialState();
        PauseControl.Unpause();
        inventoryPositionY = inventoryGroup.transform.localPosition.y;
        //menuGruopPositionY = gameMenuGroup.transform.localPosition.y;
        InitHPBar();
    }

    private void SetInitialState()
    {
        setActiveCell(0);
        //pausePanel.GetComponent<Image>().DOColor(new Color32(0, 0, 0, 0), 0);
        //pausePanel.SetActive(false);
        //gameMenuGroup.transform.DOLocalMoveY(menuGruopPositionY + 1200, 0).SetUpdate(true);
    }

    private void InitHPBar()
    {

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

    public void NextScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    /// <summary>
    /// Делает выбранную клетку больше и возвращает осатальные к обычному размеру
    /// </summary>
    /// <param name="n"></param>
    public void setActiveCell(int n)
    {
        for (int i = 0; i < InvSlots.Length; i++)
        {
            if (i == n)
                InvSlots[n].GetComponent<RectTransform>().sizeDelta = SelectedCellSize;
            else
                InvSlots[i].GetComponent<RectTransform>().sizeDelta = defaultCellSize;
        }
    }

    /// <summary>
    /// Используется чтобы вызвать событие переключения слота
    /// </summary>
    /// <param name="n">Номер выбранного слота</param>
    public void SelectCell(int n)
    {
        onCellClick(n);
    }

    //public void SetPause() { // edited
    //    if (PauseControl.isPaused) {
    //        PauseControl.Unpause();
    //        HidePauseMenu();
    //    } else {
    //        PauseControl.Pause();
    //        ShowPauseMenu();
    //    }
    //}

    //public void ShowPauseMenu()
    //{
    //    pausePanel.SetActive(true);
    //    pausePanel.GetComponent<Image>().DOColor(new Color32(0, 0, 0, 100), alphaChannelTime).SetUpdate(true);
    //    inventoryGroup.transform.DOLocalMoveY(inventoryPositionY - 300, animTime).SetUpdate(true);
    //    gameMenuGroup.transform.DOLocalMoveY(menuGruopPositionY, animTime).SetEase(Ease.OutCubic).SetUpdate(true);
    //    pauseButton.SetActive(false);
    //}

    //public void HidePauseMenu()
    //{
    //    gameMenuGroup.transform.DOLocalMoveY(menuGruopPositionY + 1200, animTime * 0.5f /*или поставить 0?*/).SetUpdate(true);
    //    inventoryGroup.transform.DOLocalMoveY(inventoryPositionY, animTime).SetUpdate(true);
    //    pausePanel.GetComponent<Image>().DOColor(new Color32(0, 0, 0, 0), alphaChannelTime);
    //    pausePanel.SetActive(false);
    //    pauseButton.SetActive(true);

    //}


    float oldHP = 1f;
    public void UpdateHpBar(float percent)
    {
        var mat = hp_bar_img.material;
        mat.SetFloat("_level", percent);
        hp_bar_img.material = mat;
        if (percent < oldHP)
        {
            StartCoroutine(ChangeBackMatVal(oldHP, percent));
        }
        else
        {
            StartCoroutine(HealHPBarAnim(oldHP, percent));
        }
        oldHP = percent;
    }
    private IEnumerator ChangeBackMatVal(float oldVal, float newVal)
    {
        var mat = hp_bar_back_img.material;
        mat.SetFloat("_level", oldVal);
        hp_bar_back_img.material = mat;
        float startTime = Time.unscaledTime;

        while (startTime + hpTime > Time.unscaledTime)
        {
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            float dt = (Time.unscaledTime - startTime) / hpTime;
            float val = Mathf.Lerp(oldVal, newVal, dt * dt);
            mat.SetFloat("_level", val);
            hp_bar_back_img.material = mat;
        }

        mat.SetFloat("_level", newVal);
        hp_bar_back_img.material = mat;
    }
    private IEnumerator HealHPBarAnim(float botEdge, float topEdge)
    {
        var mat = hp_bar_heal_img.material;
        float startTime = Time.unscaledTime;
        mat.SetFloat("_topEdge", topEdge);
        //hp_bar_heal.SetActive(true);

        while (startTime + hpTime > Time.unscaledTime)
        {
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

    public void UpdateInventorySprite(int n, Sprite sprite, int itemAmount = 0, Transform data = null, Material material = null)
    {
        var cell = InvSlots[n];
        
        cell.Sprite.sprite = sprite;
        cell.SetActiveSprite(sprite != null);
        cell.Sprite.preserveAspect = true;
        cell.Amount = itemAmount;

        if (data != null)
        {
            var _t = cell.Sprite.transform;
            _t.rotation = data.localRotation;
            _t.localScale = data.localScale;
            _t.localPosition = data.localPosition;
        }

        if (material != null)
        {
            cell.Sprite.material = material;
        }
    }

    public void InitItemDrop(int n)
    {
        onItemDrop(n);
    }

    public void InitItemSwap(int i1, int i2)
    {
        onItemSwap(i1, i2);
    }
}
