using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CursorTile : MonoBehaviour
{
    private static CursorTile _self;
    public static CursorTile self => _self;
    
    [HideInInspector] public GridTile focusedGridTile; // grid tile that is triggered by mouse cursor
    public event EventHandler OnTileClicked;

    // player character related fields
    [SerializeField] private GameObject playerPrefab;
    [HideInInspector] public PlayerController playerController;
    
    // pulse effect related fields
    [Range(0.05f, 1)] [SerializeField] private float pulseEffectSpeed = 0.25f;
    private bool sizeDecreasing;

    // other scripts references
    [HideInInspector] public Pathfinding pathfinding;
    [HideInInspector] public MoveOverlay moveOverlay;

    private void Awake()
    {
        if (_self != null && _self != this)
            Destroy(gameObject);
        else
        {
            _self = this;
            
            pathfinding = GetComponent<Pathfinding>();
            moveOverlay = GetComponent<MoveOverlay>();
        }
    }

    private void Start()
    {
        sizeDecreasing = false;

        if (playerController == null)
            playerController = Instantiate(playerPrefab).GetComponent<PlayerController>();
    }
    
    private void Update()
    {
        Pulse(pulseEffectSpeed); // pulsing animation
        
        RaycastHit2D? focusedTileHit = FocusOnTile();
        if (focusedTileHit != null)
        {
            focusedGridTile = focusedTileHit.Value.collider.gameObject.GetComponent<GridTile>();
            transform.position = focusedGridTile.gameObject.transform.position;

            if (Input.GetMouseButtonDown(0) && !playerController.isMoving) // Left for debug purposes: && (!focusedGridTile.isShown || focusedGridTile.isFadingOut)
            {
                OnTileClicked?.Invoke(this, EventArgs.Empty); // invoke "On Tile Clicked" event

                // calculate path only when clicked tile is inside move range
                if (moveOverlay.tilesInsideMoveRange.Contains(focusedGridTile)) 
                {
                    pathfinding.path = pathfinding.FindPath(playerController.activeTile, focusedGridTile);
                    focusedGridTile.Show();
                }
            }
        }
        else
        {
            focusedGridTile = null;
        }

        if (pathfinding.path.Count > 0)
            playerController.Move();
    }

    /// <summary>
    /// Focuses on hit collider during ray cast
    /// </summary>
    /// <returns> Hit collider or null when nothing was hit. </returns>
    private RaycastHit2D? FocusOnTile()
    {
        List<RaycastHit2D> hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero).ToList();

        if (hits.Any())
            return hits.OrderByDescending(p => p.collider.transform.position.z).First();
        else
            return null;
    }

    /// <summary>
    /// Pulse effect animation by shrinking and stretching object's scale in time
    /// </summary>
    private void Pulse(float speed)
    {
        Vector2 newScale = transform.localScale;
        
        if (newScale.x <= 0.75f && newScale.y <= 0.75f)
            sizeDecreasing = false;
        if (newScale.x >= 1f && newScale.y >= 1f)
            sizeDecreasing = true;

        newScale = sizeDecreasing ? new Vector2(newScale.x -= Time.deltaTime * speed, newScale.y -= Time.deltaTime * speed) : new Vector2(newScale.x += Time.deltaTime * speed, newScale.y += Time.deltaTime * speed);

        transform.localScale = newScale;
    }

}