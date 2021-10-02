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
    [SerializeField] private int _minHeigth = 40;
    [SerializeField] private int _heigthAdding = 20;

    [SerializeField] private Camera _camera = null;

    public string Header
    {
        get => HeaderText.text;
        set
        {
            HeaderText.text = value;
            if (_rect.gameObject.activeSelf)
                CheckForFit();
        }
    }

    public string Description
    {
        get => DescripitonText.text;
        set
        {
            DescripitonText.text = value;
            if (_rect.gameObject.activeSelf)
                CheckForFit();
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
        CheckForFit();
        _rect.gameObject.SetActive(true);
    }
    public void ToggleOff()
    {
        _rect.gameObject.SetActive(false);
    }

    private void CheckForFit()
    {
        IEnumerator _coroutine()
        {
            var width = Mathf.Clamp(
                Mathf.Max(
                    HeaderText.renderedWidth,
                    DescripitonText.renderedWidth),
                _minWidth,
                _maxWidth
            );

            var r = new Vector2(width, _minHeigth);

            _rect.sizeDelta = r;

            yield return new WaitForEndOfFrame();

            var heigth = Mathf.Max(
                _rectHeader.rect.height + _minHeigth,
                _rectHeader.rect.height + DescripitonText.renderedHeight + _heigthAdding
            );

            r.y = heigth;

            _rect.sizeDelta = r;
        }
        StartCoroutine(_coroutine());
    }


}
