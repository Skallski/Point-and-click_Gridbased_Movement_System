using System;
using SkalluUtils.PropertyAttributes.ReadOnlyInspectorPropertyAttribute;
using UnityEngine;

public class GridTile : MonoBehaviour
{
    [Serializable]
    public struct Node
    {
        [ReadOnlyInspector] public int gCost;
        [ReadOnlyInspector] public int hCost;
        [HideInInspector] public int fCost => gCost + hCost;
        
        [HideInInspector] public GridTile previousTile;
        [ReadOnlyInspector] public Vector3Int gridCoordinates;
    }

    public Node node; // pathfinding related struct
    
    private SpriteRenderer sr;
    
    // grid tile display related variables
    [ReadOnlyInspector] public bool isShown;
    [HideInInspector] public bool isFadingIn;
    [HideInInspector] public bool isFadingOut;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    
    private void OnEnable()
    {
        CursorTile.self.OnTileClicked += CursorTile_OnTileClicked;
    }

    private void Start()
    {
        isShown = false;

        isFadingIn = false;
        isFadingOut = false;
    }

    private void Update()
    {
        if (isFadingOut)
            Disappear();
        
        if (isFadingIn)
            Appear();
    }
    
    private void OnDisable()
    {
        CursorTile.self.OnTileClicked -= CursorTile_OnTileClicked;
    }
    
    /// <summary>
    /// Cursor tile's On Tile Clicked event subscriber method
    /// </summary>
    /// <param name="sender"> "On Tile Clicked" event sender object. </param>
    /// <param name="e"> "On Tile Clicked" event args. </param>
    private void CursorTile_OnTileClicked(object sender, EventArgs e)
    {
        if (isShown && CursorTile.self.focusedGridTile != null)
        {
            if (CursorTile.self.focusedGridTile != CursorTile.self.playerController.activeTile && CursorTile.self.moveOverlay.tilesInsideMoveRange.Contains(CursorTile.self.focusedGridTile))
            {
                isFadingOut = true;
            }
        }
    }
    
    /// <summary>
    /// Shows tile by slowly increasing alpha
    /// </summary>
    private void Appear()
    {
        Color newColor = sr.color;
        sr.color = new Color(1f, 1f, 1f, newColor.a += Time.deltaTime * 2f);

        if (sr.color.a >= 1f)
            Show();
    }

    /// <summary>
    /// Shows tile instantly
    /// </summary>
    public void Show()
    {
        isFadingIn = false;
        
        sr.color = new Color(1f, 1f, 1f, 1f);
        isShown = true;
    }

    /// <summary>
    /// Highlights tile with given color instantly
    /// </summary>
    /// <param name="highlightColor"> Color the object will be highlighted with. </param>
    public void Highlight(Color highlightColor)
    {
        if (sr.color != highlightColor)
            sr.color = highlightColor;
    }

    /// <summary>
    /// Hides tile by slowly decreasing alpha
    /// </summary>
    private void Disappear()
    {
        Color newColor = sr.color;
        sr.color = new Color(1f, 1f, 1f, newColor.a -= Time.deltaTime * 2f);

        if (sr.color.a <= 0f)
            Hide();
    }
    
    /// <summary>
    /// Hides tile instantly
    /// </summary>
    public void Hide()
    {
        isFadingOut = false;
        
        sr.color = new Color(1f, 1f, 1f, 0f);
        isShown = false;
    }
    
}