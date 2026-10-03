using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindOption : MonoBehaviour
{
    private Button button;
    private GameObject option;
    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => UIManager.Instance.PopupPanel(UIManager.Instance.option));
    }
}
