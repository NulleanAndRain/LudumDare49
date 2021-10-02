using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemInfo : MonoBehaviour
{
    [SerializeField] TMP_Text HeaderText = null;
    [SerializeField] TMP_Text DescripitonText = null;
    [SerializeField] RectTransform _rect = null;
    [SerializeField] RectTransform _rectHeader = null;

    [SerializeField] int _minWidth = 200;
    [SerializeField] int _maxWidth = 350;
    [SerializeField] int _minHeigth = 120;


    public string Header
    {
        get => HeaderText.text;
        set
        {
            //CheckForFit();
            HeaderText.text = value;
        }
    }

    public string Description
    {
        get => DescripitonText.text;
        set
        {
            //CheckForFit();
            DescripitonText.text = value;
        }

    }

    private void Start()
    {
        ToggleOff();
    }

    public void ToggleOn()
    {
        _rect.gameObject.SetActive(true);
    }
    public void ToggleOff()
    {
        _rect.gameObject.SetActive(false);
    }

    private void CheckForFit()
    { // todo: fit on screen (?)
        var r = _rect.rect;

        var width = Mathf.Clamp(
            Mathf.Max(
                HeaderText.renderedWidth,
                DescripitonText.renderedWidth),
            _minWidth,
            _maxWidth
        );

        r.width = width;

        var heigth = Mathf.Max(
            _minHeigth,
            _rectHeader.rect.height + DescripitonText.renderedHeight
        );

        r.height = heigth;

        //_rect.rect = r;
    }


}
