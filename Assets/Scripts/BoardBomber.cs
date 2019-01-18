using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Board))]

public class BoardBomber : MonoBehaviour
{
    public Board board;

    private void Awake()
    {
        board = GetComponent<Board>();
    }


    // generates bombs over two swiped tiles
    public void ProcessBombs(Tile tileA, Tile tileB, GamePiece clickedPiece, GamePiece targetPiece, List<GamePiece> tileAPieces, List<GamePiece> tileBPieces)
    {
        // record the general vector of our swipe
        Vector2 swipeDirection = new Vector2(tileB.xIndex - tileA.xIndex, tileB.yIndex - tileA.yIndex);

        // convert the clicked GamePiece or target GamePiece to a bomb depending on matches and swipe direction
        board.clickedTileBomb = DropBomb(tileA.xIndex, tileA.yIndex, swipeDirection, tileAPieces);
        board.targetTileBomb = DropBomb(tileB.xIndex, tileB.yIndex, swipeDirection, tileBPieces);

        // if the clicked GamePiece is a non-color Bomb, then change its color to the correct target color
        if (board.clickedTileBomb != null && targetPiece != null)
        {
            GamePiece clickedBombPiece = board.clickedTileBomb.GetComponent<GamePiece>();
            if (!board.boardQuery.IsColorBomb(clickedBombPiece))
            {
                clickedBombPiece.ChangeColor(targetPiece);
            }
        }

        // if the target GamePiece is a non-color Bomb, then change its color to the correct clicked color
        if (board.targetTileBomb != null && clickedPiece != null)
        {
            GamePiece targetBombPiece = board.targetTileBomb.GetComponent<GamePiece>();

            if (!board.boardQuery.IsColorBomb(targetBombPiece))
            {
                targetBombPiece.ChangeColor(clickedPiece);
            }
        }
    }



    // returns matching pieces from a ColorBomb
    public List<GamePiece> ProcessColorBombs(GamePiece clickedPiece, GamePiece targetPiece,
                                             bool clearNonBlockers = false)
    {
        // create a new List to hold potential color matches
        List<GamePiece> colorMatches = new List<GamePiece>();

        // reference to our two swapped GamePieces
        GamePiece colorBombPiece = null;
        GamePiece otherPiece = null;

        // if the clicked GamePiece is a Color Bomb and the target GamePiece is not
        if (board.boardQuery.IsColorBomb(clickedPiece) && !board.boardQuery.IsColorBomb(targetPiece))
        {
            colorBombPiece = clickedPiece;
            otherPiece = targetPiece;
        }
        //... if the target GamePiece is a Color Bomb, and the clicked GamePiece is not
        else if (!board.boardQuery.IsColorBomb(clickedPiece) && board.boardQuery.IsColorBomb(targetPiece))
        {
            colorBombPiece = targetPiece;
            otherPiece = clickedPiece;
        }
        //... if they are both Color bombs
        else if (board.boardQuery.IsColorBomb(clickedPiece) && board.boardQuery.IsColorBomb(targetPiece))
        {
            foreach (GamePiece piece in board.allGamePieces)
            {
                if (!colorMatches.Contains(piece))
                {
                    colorMatches.Add(piece);
                }
            }
        }

        // if nether GamePiece is a color bomb, we do nothing and return an empty list of GamePieces

        // if one GamePiece is a color bomb...
        if (colorBombPiece != null)
        {
            // set the color bomb's matchValue
            colorBombPiece.matchValue = otherPiece.matchValue;

            // store a list of all GamePieces with that matchValue
            colorMatches = board.boardQuery.FindAllMatchValue(otherPiece.matchValue);
        }

        // if you are only clearing Blockers (not Collectibles)...
        if (!clearNonBlockers)
        {
            // get a list of Collectibles that are cleared at the bottom of the Board
            List<GamePiece> collectedAtBottom = board.boardQuery.FindAllCollectibles(true);

            // if the other GamePiece in the swap is a Collectible, return nothing
            if (collectedAtBottom.Contains(otherPiece))
            {
                return new List<GamePiece>();
            }
            else
            {
                // otherwise, just remove all of the Collectibles from our list
                foreach (GamePiece piece in collectedAtBottom)
                {
                    colorMatches.Remove(piece);
                }
            }
        }
        // return list of GamePieces with the correct matchValue
        return colorMatches;
    }


    // drops a Bomb at a position (x,y) in the Board, given a list of matching GamePieces
    public GameObject DropBomb(int x, int y, Vector2 swapDirection, List<GamePiece> gamePieces)
    {

        GameObject bomb = null;
        MatchValue matchValue = MatchValue.None;

        if (gamePieces != null)
        {
            matchValue = board.boardQuery.FindMatchValue(gamePieces);
        }

        // check if the GamePieces are four or more in a row
        if (gamePieces.Count >= 5 && matchValue != MatchValue.None)
        {
            // check if we form a corner match and create an adjacent bomb
            if (board.boardQuery.IsCornerMatch(gamePieces))
            {
                GameObject adjacentBomb = board.boardQuery.FindGamePieceByMatchValue(board.adjacentBombPrefabs, matchValue);

                if (adjacentBomb != null)
                {
                    bomb = board.boardFiller.MakeBomb(adjacentBomb, x, y);
                }
            }
            else
            {
                // if have five or more in a row, form a color bomb - note we probably should swap this upward to 
                // give it priority over an adjacent bomb

                if (board.colorBombPrefab != null)
                {
                    bomb = board.boardFiller.MakeBomb(board.colorBombPrefab, x, y);

                }
            }
        }
        else if (gamePieces.Count == 4 && matchValue != MatchValue.None)
        {
            // otherwise, drop a row bomb if we are swiping sideways
            if (Mathf.Abs(swapDirection.x) > 0.01f)
            {
                GameObject rowBomb = board.boardQuery.FindGamePieceByMatchValue(board.rowBombPrefabs, matchValue);
                if (rowBomb != null)
                {
                    bomb = board.boardFiller.MakeBomb(rowBomb, x, y);
                }
            }
            else
            {
                GameObject columnBomb = board.boardQuery.FindGamePieceByMatchValue(board.columnBombPrefabs, matchValue);
                // or drop a vertical bomb if we are swiping upwards
                if (columnBomb != null)
                {
                    bomb = board.boardFiller.MakeBomb(columnBomb, x, y);
                }
            }
        }
        // return the Bomb object
        return bomb;
    }

    // puts the bomb into the game Board and treats it as a normal GamePiece
    public void InitBomb(GameObject bomb)
    {
        int x = (int)bomb.transform.position.x;
        int y = (int)bomb.transform.position.y;


        if (board.boardQuery.IsWithinBounds(x, y))
        {
            board.allGamePieces[x, y] = bomb.GetComponent<GamePiece>();
        }
    }

    // initializes any bombs created on the clicked or target tile
    public void InitAllBombs()
    {
        if (board.clickedTileBomb != null)
        {
            board.boardBomber.InitBomb(board.clickedTileBomb);
            board.clickedTileBomb = null;
        }

        if (board.targetTileBomb != null)
        {
            board.boardBomber.InitBomb(board.targetTileBomb);
            board.targetTileBomb = null;
        }
    }


}
