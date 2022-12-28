using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour
{
    private Image img;
    [SerializeField] private Color defaultColor;

    [SerializeField] private Color hoverColor = new Color(192, 222, 255,1);
    void Start()
    {
        img = GetComponent<Image>();
        defaultColor = img.color;
    }

    public void OnHoverEnter()
    {
        img.color = hoverColor;
    }

    public void OnHoverExit()
    {
        img.color = defaultColor;
    }
}
