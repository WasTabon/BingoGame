using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TopPanelLayout : MonoBehaviour
{
    [SerializeField] private RectTransform topPanel;
    [SerializeField] private RectTransform chooseNumberPanel;
    [SerializeField] private TextMeshProUGUI chooseNumberText;
    [SerializeField] private RectTransform[] buttons;
    [SerializeField] private GridLayoutGroup buttonsGrid;
    
    [SerializeField] private float spacing = 100f;
    [SerializeField] private float buttonSizeMultiplier = 1.5f;
    [SerializeField] private float minFontSize = 30f;
    [SerializeField] private float maxFontSize = 60f;
    [SerializeField] private float verticalSpacing = 50f;
    [SerializeField] private float panelPaddingHorizontal = 50f;
    [SerializeField] private float panelPaddingVertical = 30f;

    void Start()
    {
        if (buttonsGrid == null && buttons.Length > 0)
        {
            buttonsGrid = buttons[0].parent.GetComponent<GridLayoutGroup>();
        }
        
        SetupButtonsGrid();
        UpdateLayout();
    }

    void OnRectTransformDimensionsChange()
    {
        UpdateLayout();
    }

    void SetupButtonsGrid()
    {
        if (buttonsGrid == null) return;

        buttonsGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        buttonsGrid.constraintCount = 3;
        buttonsGrid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        buttonsGrid.startAxis = GridLayoutGroup.Axis.Horizontal;
        buttonsGrid.childAlignment = TextAnchor.MiddleCenter;
        buttonsGrid.spacing = new Vector2(spacing, spacing);
    }

    void UpdateLayout()
    {
        if (topPanel == null || chooseNumberPanel == null || buttonsGrid == null || chooseNumberText == null) return;

        float leftOffset = Screen.safeArea.x + 20f;
        float rightOffset = (Screen.width - Screen.safeArea.xMax) + 20f;
        
        float totalSpacingX = spacing * 2;
        float availableWidth = topPanel.rect.width - totalSpacingX - leftOffset - rightOffset;
        
        float buttonSize = (availableWidth / 3) * buttonSizeMultiplier;
        buttonSize = Mathf.Min(buttonSize, topPanel.rect.height * 0.3f);
        
        buttonsGrid.cellSize = new Vector2(buttonSize, buttonSize);
        
        float buttonsRowWidth = buttonSize * 3 + spacing * 2;
        
        float fontSize = Mathf.Lerp(minFontSize, maxFontSize, buttonSize / 150f);
        chooseNumberText.fontSize = fontSize;
        chooseNumberText.ForceMeshUpdate();
        
        Vector2 textSize = chooseNumberText.GetPreferredValues();
        
        float chooseNumberWidth = textSize.x + panelPaddingHorizontal * 2;
        float chooseNumberHeight = textSize.y + panelPaddingVertical * 2;
        
        chooseNumberPanel.sizeDelta = new Vector2(chooseNumberWidth, chooseNumberHeight);
        
        float totalWidthIfSideBySide = buttonsRowWidth + spacing + chooseNumberWidth;
        float availableHeightForStacking = topPanel.rect.height - buttonSize - verticalSpacing * 2;
        
        if (availableHeightForStacking >= chooseNumberHeight + verticalSpacing)
        {
            chooseNumberPanel.anchoredPosition = new Vector2(0, buttonSize / 2 + verticalSpacing + chooseNumberHeight / 2);
            buttonsGrid.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -verticalSpacing);
        }
        else
        {
            chooseNumberPanel.anchoredPosition = new Vector2(-buttonsRowWidth / 2 - spacing / 2 - chooseNumberWidth / 2, 0);
            buttonsGrid.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(chooseNumberWidth / 2 + spacing / 2, 0);
        }
    }
}