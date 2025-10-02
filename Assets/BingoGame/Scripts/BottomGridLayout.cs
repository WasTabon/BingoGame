using UnityEngine;
using UnityEngine.UI;

public class BottomGridLayout : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup gridLayout;
    [SerializeField] private RectTransform panel;
    [SerializeField] private int columns = 3;
    [SerializeField] private float spacing = 100f;
    [SerializeField] private float additionalPadding = 20f;

    void Start()
    {
        if (gridLayout == null) gridLayout = GetComponent<GridLayoutGroup>();
        if (panel == null) panel = GetComponent<RectTransform>();
        
        SetupGridLayout();
        UpdateCellSize();
    }

    void OnRectTransformDimensionsChange()
    {
        UpdateCellSize();
    }

    void SetupGridLayout()
    {
        if (gridLayout == null) return;

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;
        gridLayout.startCorner = GridLayoutGroup.Corner.LowerLeft;
        gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayout.childAlignment = TextAnchor.LowerCenter;
        gridLayout.padding = new RectOffset(0, 0, 0, 0);
    }

    void UpdateCellSize()
    {
        if (gridLayout == null || panel == null) return;

        gridLayout.spacing = new Vector2(spacing, spacing);
        
        float leftOffset = Screen.safeArea.x + additionalPadding;
        float rightOffset = (Screen.width - Screen.safeArea.xMax) + additionalPadding;
        
        float totalSpacingX = spacing * (columns - 1);
        float availableWidth = panel.rect.width - totalSpacingX - leftOffset - rightOffset;
        
        float cellSize = availableWidth / columns;
        
        gridLayout.cellSize = new Vector2(cellSize, cellSize);
        
        gridLayout.padding.left = (int)leftOffset;
        gridLayout.padding.right = (int)rightOffset;
    }
}