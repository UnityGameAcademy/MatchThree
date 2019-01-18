using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// use these methods from the Board class to shuffle your GamePieces
public class BoardShuffler : MonoBehaviour
{

    // removes non-bomb and collectible pieces from your GamePiece array and returns them as a List
    public List<GamePiece> RemoveNormalPieces(GamePiece[,] allPieces)
    {
        // list of pieces to return
        List<GamePiece> normalPieces = new List<GamePiece>();

        // get width and height from array
        int width = allPieces.GetLength(0);
        int height = allPieces.GetLength(1);

        // foreach position in the array...
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // ... if it's not a null object (i.e. a hole caused by Obstacle)
                if (allPieces[i, j] != null)
                {
                    // check for bomb and collectible components
                    Bomb bomb = allPieces[i, j].GetComponent<Bomb>();
                    Collectible collectible = allPieces[i, j].GetComponent<Collectible>();

                    // add to normalPieces list if gamePiece is not Bomb or Collectible
                    if (bomb == null && collectible == null)
                    {
                        normalPieces.Add(allPieces[i, j]);

                        // and clear position from original array
                        allPieces[i, j] = null;
                    }
                }
            }
        }

        // returns list of non-bomb and non-collectible pieces
        return normalPieces;
    }

    // shuffles a list of GamePieces in place using Fisher-Yates shuffle
    public void ShuffleList(List<GamePiece> piecesToShuffle)
    {
        // number of GamePieces to shuffle
        int maxCount = piecesToShuffle.Count;

        // count up to maxCount minus 1 (last item has no other GamePieces to swap with)
        for (int i = 0; i < maxCount - 1; i++)
        {
            // generate a random number from current item to end of the list (note: maxCount is exclusive for integers)
            int r = Random.Range(i, maxCount);

            // if we have selected the current GamePiece, skip to next count
            if (r == i)
            {
                continue;
            }

            // swap the current items with the randomly selected item
            GamePiece temp = piecesToShuffle[r];

            piecesToShuffle[r] = piecesToShuffle[i];

            piecesToShuffle[i] = temp;

        }
    }

    // moves GamePieces into onscreen positions after being shuffled in the array
    public void MovePieces(GamePiece[,] allPieces, float swapTime = 0.5f)
    {

        // get width and height from array
        int width = allPieces.GetLength(0);
        int height = allPieces.GetLength(1);

        // run Move method for each GamePiece to move to correct (x,y) position onscreen
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allPieces[i, j] != null)
                {
                    allPieces[i, j].Move(i, j, swapTime);
                }
            }
        }

    }




}
