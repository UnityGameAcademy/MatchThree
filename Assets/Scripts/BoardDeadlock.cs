using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoardDeadlock : MonoBehaviour
{
    // given an (x,y) coordinate return a List of GamePieces (either a row or column) 
    List<GamePiece> GetRowOrColumnList(GamePiece[,] allPieces, int x, int y, int listLength = 3, bool checkRow = true)
    {
        // get the Board dimensions
        int width = allPieces.GetLength(0);
        int height = allPieces.GetLength(1);

        // empty list to store GamePieces
        List<GamePiece> piecesList = new List<GamePiece>();

        // loop through listLength number of pieces
        for (int i = 0; i < listLength; i++)
        {
            // for getting a row...
            if (checkRow)
            {
                // the coordinate is within the Board dimensions, add to our running list
                if (x + i < width && y < height && allPieces[x + i, y] != null)
                {
                    piecesList.Add(allPieces[x + i, y]);
                }
            }
            // for getting a column...
            else
            {
                // the coordinate is within the Board dimensions, add to our running list
                if (x < width && y + i < height && allPieces[x, y + i] != null)
                {
                    piecesList.Add(allPieces[x, y + i]);
                }
            }
        }
        return piecesList;
    }

    // given a list of GamePieces, return a sub-list of matching GamePieces
    List<GamePiece> GetMinimumMatches(List<GamePiece> gamePieces, int minForMatch = 2)
    {
        // empty running list of GamePieces
        List<GamePiece> matches = new List<GamePiece>();

        // use the Linq.GroupBy method to break the list into smaller groups
        var groups = gamePieces.GroupBy(n => n.matchValue);

        // check each group for minimum length and valid MatchValue
        foreach (var grp in groups)
        {
            // convert the group to our list of matches if it meets the requirements
            if (grp.Count() >= minForMatch && grp.Key != MatchValue.None)
            {
                matches = grp.ToList();
            }
        }
        // return the list of matching GamePieces (can be an empty list if we don't have valid matches)
        return matches;
    }

    // given a coordinate (x,y), locate the four adjacent GamePieces in the array
    List<GamePiece> GetNeighbors(GamePiece[,] allPieces, int x, int y)
    {
        // get the Board dimensions
        int width = allPieces.GetLength(0);
        int height = allPieces.GetLength(1);

        // empty List of GamePieces to return
        List<GamePiece> neighbors = new List<GamePiece>();

        // four compass directions represented as Vector2's
        Vector2[] searchDirections = new Vector2[4]
        {
            new Vector2(-1f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(0f, -1f)
        };

        // foreach compass direction, check if the neighboring space has a valid GamePiece
        foreach (Vector2 dir in searchDirections)
        {
            // verify the neighboring position is within the Board
            if (x + (int)dir.x >= 0 && x + (int)dir.x < width && y + (int)dir.y >= 0 && y + (int)dir.y < height)
            {
                // add each valid neighboring GamePiece to our running list
                if (allPieces[x + (int)dir.x, y + (int)dir.y] != null)
                {
                    if (!neighbors.Contains(allPieces[x + (int)dir.x, y + (int)dir.y]))
                    {
                        neighbors.Add(allPieces[x + (int)dir.x, y + (int)dir.y]);
                    }
                }
            }
        }
        // return the list of GamePieces (can be empty List if we don't have valid neighbors)
        return neighbors;
    }

    // do we have a possible move at postion (x,y) in the Board?
    bool HasMoveAt(GamePiece[,] allPieces, int x, int y, int listLength = 3, bool checkRow = true)
    {
        // list of GamePieces starting at coordinate (x,y)
        List<GamePiece> pieces = GetRowOrColumnList(allPieces, x, y, listLength, checkRow);

        // find what GamePieces in the list already match (usually we are looking for two out of three)
        List<GamePiece> matches = GetMinimumMatches(pieces, listLength - 1);

        // if we have matching GamePieces, find the single unmatched piece
        GamePiece unmatchedPiece = null;
        if (pieces != null && matches != null)
        {
            if (pieces.Count == listLength && matches.Count == listLength - 1)
            {
                unmatchedPiece = pieces.Except(matches).FirstOrDefault();
            }

            // if we have an unmatched GamePiece, check its neighboring GamePieces
            if (unmatchedPiece != null)
            {
                List<GamePiece> neighbors = GetNeighbors(allPieces, unmatchedPiece.xIndex, unmatchedPiece.yIndex);
                neighbors = neighbors.Except(matches).ToList();
                neighbors = neighbors.FindAll(n => n.matchValue == matches[0].matchValue);
                matches = matches.Union(neighbors).ToList();
            }

            // if the neighbor of the unmatched Piece matches our original list of matches, return true; we can make a move here
            if (matches.Count >= listLength)
            {
//                string rowColStr = (checkRow) ? " row " : " column ";
//                Debug.Log("======= AVAILABLE MOVE ================================");
//                Debug.Log("Move " + matches[0].matchValue + " piece to " + unmatchedPiece.xIndex + "," + 
//                    unmatchedPiece.yIndex + " to form matching " + rowColStr);
                return true;
            }
        }

        // otherwise, return false
        return false;
    }

    // does the Board have any more moves available?
    public bool IsDeadlocked(GamePiece[,] allPieces, int listLength = 3)
    {
        // get the Board dimensions
        int width = allPieces.GetLength(0);
        int height = allPieces.GetLength(1);

        // set our default return value to true
        bool isDeadlocked = true;

        // check each position if there is an available move 
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // if the row or column of GamePieces has an available move, return false; board is not deadlocked
                if (HasMoveAt(allPieces, i, j, listLength, true) || HasMoveAt(allPieces, i, j, listLength, false))
                {
                    isDeadlocked = false;

                }
            }
        }

        // // console notification 
        // if (isDeadlocked)
        // {
        //     Debug.Log(" ===========  BOARD DEADLOCKED ================= ");
        // }

        // otherwise, return true; board is deadlocked
        return isDeadlocked;

    }
}
