using UnityEngine;
using System.Collections;

// a Tile can be an empty space, an Obstacle preventing movement, or a removeable Breakable tile
public enum TileType 
{
	Normal,
	Obstacle,
	Breakable
}
// a Tile represents one space of the Board

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour {

    // x and y position in the array
	public int xIndex;
	public int yIndex;

    // reference to our Board
	Board m_board;

    // our current TileType
	public TileType tileType = TileType.Normal;

    // the Sprite for this Tile
	SpriteRenderer m_spriteRenderer;

    // current "health" of a Breakable tile before it is removed
	public int breakableValue = 0;

    // array of Sprites used to show damage on Breakable Tile
	public Sprite[] breakableSprites;

    // sets the color of Breakable Tile back to normal once it is removed
	public Color normalColor;

	// Use this for initialization
	void Awake () 
	{
        // initialize our SpriteRenderer
		m_spriteRenderer = GetComponent<SpriteRenderer>();
	}

    // initialze the Tile's array index and cache a reference to the Board
	public void Init(int x, int y, Board board)
	{
		xIndex = x;
		yIndex = y;
		m_board = board;

        // if the Tile is breakable, set its Sprite
		if (tileType == TileType.Breakable)
		{
			if (breakableSprites[breakableValue] !=null)
			{
				m_spriteRenderer.sprite = breakableSprites[breakableValue];
			}
		}
	}

    // if the mouse clicks the Collider on this Tile, run ClickTile on the Board
	void OnMouseDown()
	{
		if (m_board !=null)
		{
			m_board.ClickTile(this);
		}

	}

    // if the mousebutton is held and then the pointer is dragged into the Collider on this Tile...
    // run DragToTile on the Board, passing in this component 
	void OnMouseEnter()
	{
		if (m_board !=null)
		{
			m_board.DragToTile(this);
		}
	}

    // if we let go of the mouse button while on this Tile, run ReleaseTile on the Board
	void OnMouseUp()
	{
		if (m_board !=null)
		{
			m_board.ReleaseTile();
		}
	}

    // starts the coroutine to break a Breakable Tile
	public void BreakTile()
	{
		if (tileType != TileType.Breakable)
		{
			return;
		}

		StartCoroutine(BreakTileRoutine());
	}

    // decrement the breakable value, switch to the appropriate sprite
    // and conver the Tile to become normal once the breakableValue reaches 0
	IEnumerator BreakTileRoutine()
	{
		breakableValue = Mathf.Clamp(breakableValue--, 0, breakableValue);

		yield return new WaitForSeconds(0.25f);

		if (breakableSprites[breakableValue] !=null)
		{
			m_spriteRenderer.sprite = breakableSprites[breakableValue];
		}

		if (breakableValue == 0)
		{
			tileType = TileType.Normal;
			m_spriteRenderer.color = normalColor;

		}

	}

}
