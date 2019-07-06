using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GamePieceBreakable : GamePiece
{

    public int breakableValue = 0;

    // array of Sprites used to show damage on Breakable Tile
    public Sprite[] breakableSprites;

    // the Sprite for this Tile
    private SpriteRenderer m_spriteRenderer;

    public float breakDelay = 0.25f;
    public bool isBroken = false;

    public override void Awake()
    {
        base.Awake();

        // initialize our SpriteRenderer
        m_spriteRenderer = GetComponent<SpriteRenderer>();

        if (breakableSprites[breakableValue] != null)
        {
            m_spriteRenderer.sprite = breakableSprites[breakableValue];
        }

    }

    public void BreakPiece()
    {
        if (isBroken)
        {
            return;
        }

        StartCoroutine(BreakPieceRoutine());
    }

    // decrement the breakable value, switch to the appropriate sprite
    // and conver the Tile to become normal once the breakableValue reaches 0
    IEnumerator BreakPieceRoutine()
    {
        if (!isBroken)
        {
            breakableValue = Mathf.Clamp(breakableValue--, 0, breakableValue);

            if (breakableSprites[breakableValue] != null)
            {
                m_spriteRenderer.sprite = breakableSprites[breakableValue];
            }
        }

        yield return new WaitForSeconds(breakDelay);

        // if we are broken already, just clear normally
        if (breakableValue == 0)
        {
            isBroken = true;

        }
    }

}
