using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionGoalPanel : MonoBehaviour
{
	// collection goal 
    public CollectionGoal collectionGoal;

    // text indicating number to collect
    public Text numberLeftText;

    // icon for the GamePiece
    public Image prefabImage;

    void Start()
    {
        SetupPanel();
    }
	
	// setup the sprites and text
    public void SetupPanel()
    {
        if (collectionGoal != null && numberLeftText != null && prefabImage != null)
        {
            SpriteRenderer prefabSprite = collectionGoal.prefabToCollect.GetComponent<SpriteRenderer>(); 
            if (prefabSprite != null)
            {
                prefabImage.sprite = prefabSprite.sprite;
                prefabImage.color = prefabSprite.color;
            }

            numberLeftText.text = collectionGoal.numberToCollect.ToString();
        }
    }

    // update the text
    public void UpdatePanel()
    {
        if (collectionGoal != null && numberLeftText != null)
        {
            numberLeftText.text = collectionGoal.numberToCollect.ToString();
        }
    }

}
