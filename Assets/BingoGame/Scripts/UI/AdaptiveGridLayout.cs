using UnityEngine;
using UnityEngine.UI;

public class AdaptiveGridLayout : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup gridLayout;
    [SerializeField] private RectTransform panel;
    [SerializeField] private int rows = 5;
    [SerializeField] private int columns = 5;
    [SerializeField] private float spacing = 30f;

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
        gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayout.childAlignment = TextAnchor.MiddleCenter;
        gridLayout.padding = new RectOffset(0, 0, 0, 0);
    }

    void UpdateCellSize()
    {
        if (gridLayout == null || panel == null) return;

        gridLayout.spacing = new Vector2(spacing, spacing);
        
        float totalSpacingX = spacing * (columns - 1);
        float totalSpacingY = spacing * (rows - 1);
        
        float availableWidth = panel.rect.width - totalSpacingX - 20f;
        float availableHeight = panel.rect.height - totalSpacingY - 20f;
        
        float cellWidth = availableWidth / columns;
        float cellHeight = availableHeight / rows;
        
        float cellSize = Mathf.Min(cellWidth, cellHeight);
        
        gridLayout.cellSize = new Vector2(cellSize, cellSize);
    }
}