using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text HeaderText = null;
    [SerializeField] private TMP_Text DescripitonText = null;
    [SerializeField] private RectTransform _rect = null;
    [SerializeField] private RectTransform _rectHeader = null;

    [SerializeField] private int _minWidth = 200;
    [SerializeField] private int _maxWidth = 350;
    [SerializeField] private int _minHeigth = 120;

    [SerializeField] private Camera _camera = null;

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

    public void FixedUpdate()
    {
        if (!_rect.gameObject.activeSelf) return;

        var pos = _camera.ScreenToWorldPoint(Input.mousePosition);
        pos.z = transform.position.z;

        transform.position = pos;
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
        var r = _rect.position;

        var width = Mathf.Clamp(
            Mathf.Max(
                HeaderText.renderedWidth,
                DescripitonText.renderedWidth),
            _minWidth,
            _maxWidth
        );

        r.x += width;

        _rect.right = r;

        var heigth = Mathf.Max(
            _minHeigth,
            _rectHeader.rect.height + DescripitonText.renderedHeight
        );

        r.y = heigth;

        //_rect.rect = r;
    }


}
