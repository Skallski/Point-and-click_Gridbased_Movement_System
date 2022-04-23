using SkalluUtils.PropertyAttributes.ReadOnlyInspectorPropertyAttribute;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static PlayerController _self;
    public static PlayerController self => _self;

    #region Inspector Fields
    [Tooltip("gridTile that the player character is currently standing on")]
    [ReadOnlyInspector] public GridTile activeTile;

    [Tooltip("the number of tiles (in a straight line) that the player moves for each turn")]
    public int moveRange = 2;
    
    [Tooltip("the speed with which the player character moves")]
    public float movementSpeed = 10f;
    #endregion
    
    [HideInInspector] public bool isMoving = false;
    private const float YPositionOffset = 0.0001f; // y offset to avoid clipping through certain objects
    
    private SpriteRenderer sr;

    private void Awake()
    {
        if (_self != null && _self != this)
            Destroy(gameObject);
        else
        {
            _self = this;
            
            sr = GetComponent<SpriteRenderer>();
        }
    }

    private void Start()
    {
        CursorTile.self.focusedGridTile = Gridmap.self.gridTileContainerObj.transform.GetChild(0).gameObject.GetComponent<GridTile>();
        PlaceOnTile(CursorTile.self.focusedGridTile);
        
        CursorTile.self.moveOverlay.ShowSurroundingTilesForMove(activeTile, moveRange);
    }

    /// <summary>
    /// Places player character on given grid tile
    /// </summary>
    /// <param name="gridTile"> Grid tile, on which player will be placed. </param>
    private void PlaceOnTile(GridTile gridTile)
    {
        var gridTilePos = gridTile.transform.position;
        transform.position = new Vector3(gridTilePos.x, gridTilePos.y + YPositionOffset, gridTilePos.z);
        activeTile = gridTile;
    }

    /// <summary>
    /// Moves player character along path
    /// </summary>
    public void Move()
    {
        isMoving = true;
        
        var path = CursorTile.self.pathfinding.path;
        
        transform.position = Vector2.MoveTowards(transform.position, path[0].transform.position, movementSpeed * Time.deltaTime);

        // flips player sprite in the proper direction
        if (path[0].transform.position.x - transform.position.x > 0.01f && sr.flipX)
            sr.flipX = false;
        else if (path[0].transform.position.x - transform.position.x < -0.01f && !sr.flipX)
            sr.flipX = true;

        if (Vector2.Distance(transform.position, path[0].transform.position) < 0.001f)
        {
            PlaceOnTile(path[0]);
            path.RemoveAt(0);
        }

        if (path.Count == 0)
        {
            CursorTile.self.moveOverlay.ShowSurroundingTilesForMove(activeTile, moveRange);
            isMoving = false;
        }
    }

}