using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemInfo : MonoBehaviour
{
    [SerializeField] TMP_Text HeaderText = null;
    [SerializeField] TMP_Text DescripitonText = null;

    public string Header { get => HeaderText.text; set => HeaderText.text = value; }
    public string Description { get => DescripitonText.text; set => DescripitonText.text = value; }

}
