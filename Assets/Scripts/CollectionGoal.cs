using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionGoal : MonoBehaviour
{
    public GamePiece prefabToCollect;

    [Range(1,50)]
    public int numberToCollect = 5;

    SpriteRenderer m_spriteRenderer;

    // Use this for initialization
    void Start()
    {
        if (prefabToCollect != null)
        {
            m_spriteRenderer = prefabToCollect.GetComponent<SpriteRenderer>();
        }
    }

    public void CollectPiece(GamePiece piece)
    {
        if (piece == null)
        {
            Debug.LogError("COLLECTIONGOAL CollectPiece: missing piece");
            return;
        }

        if (prefabToCollect == null)
        {
            Debug.LogError("COLLECTIONGOAL CollectPiece: missing prefab specified...");
            return;
        }

        if (m_spriteRenderer == null)
        {
            m_spriteRenderer = prefabToCollect.GetComponent<SpriteRenderer>();
        }

        if (m_spriteRenderer == null)
        {
            Debug.LogError("COLLECTIONGOAL CollectPiece: prefab missing SpriteRenderer...");
            return;
        }

        if (piece != null)
        {

            SpriteRenderer spriteRenderer = piece.GetComponent<SpriteRenderer>();
            
            if (m_spriteRenderer.sprite == spriteRenderer.sprite && prefabToCollect.matchValue == piece.matchValue)
            {
                numberToCollect--;
                numberToCollect = Mathf.Clamp(numberToCollect, 0, numberToCollect);
            }
        }
    }
}
