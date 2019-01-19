using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Board))]
public class BoardMatcher : MonoBehaviour
{
    public Board board;

    private void Awake()
    {
        board = GetComponent<Board>();
    }


    // general method to find matches, defaulting to a minimum of three-in-a-row, passing in an (x,y) position and direction

    public List<GamePiece> FindMatches(int startX, int startY, Vector2 searchDirection, int minLength = 3)
    {
        // keep a running list of GamePieces
        List<GamePiece> matches = new List<GamePiece>();

        GamePiece startPiece = null;

        // get a starting piece at an (x,y) position in the array of GamePieces
        if (board.boardQuery.IsWithinBounds(startX, startY) && board.boardQuery.IsUnblocked(startX,startY))
        {
            startPiece = board.allGamePieces[startX, startY];
        }

        if (startPiece != null)
        {
            matches.Add(startPiece);
        }
        else
        {
            return null;
        }

        // use the search direction to increment to the next space to look...
        int nextX;
        int nextY;

        int maxValue = (board.width > board.height) ? board.width : board.height;

        for (int i = 1; i < maxValue - 1; i++)
        {
            nextX = startX + (int)Mathf.Clamp(searchDirection.x, -1, 1) * i;
            nextY = startY + (int)Mathf.Clamp(searchDirection.y, -1, 1) * i;

            if (!board.boardQuery.IsWithinBounds(nextX, nextY))
            {
                break;
            }

            // ... find the adjacent GamePiece and check its MatchValue...
            GamePiece nextPiece = board.allGamePieces[nextX, nextY];

            if (nextPiece == null)
            {
                break;
            }

            // ... if it matches then add it our running list of GamePieces
            else
            {
                if (nextPiece.matchValue == startPiece.matchValue && !matches.Contains(nextPiece) && nextPiece.matchValue != MatchValue.None &&
                    board.boardQuery.IsUnblocked(nextPiece.xIndex, nextPiece.yIndex))
                {
                    matches.Add(nextPiece);
                }
                else
                {
                    break;
                }
            }
        }

        // if our list is greater than our minimum (usually 3), then return the list...
        if (matches.Count >= minLength)
        {
            return matches;
        }

        //...otherwise return nothing
        return null;

    }

    // find all vertical matches given a position (x,y) in the Board
    public List<GamePiece> FindVerticalMatches(int startX, int startY, int minLength = 3)
    {
        List<GamePiece> upwardMatches = FindMatches(startX, startY, new Vector2(0, 1), 2);
        List<GamePiece> downwardMatches = FindMatches(startX, startY, new Vector2(0, -1), 2);

        if (upwardMatches == null)
        {
            upwardMatches = new List<GamePiece>();
        }

        if (downwardMatches == null)
        {
            downwardMatches = new List<GamePiece>();
        }

        var combinedMatches = upwardMatches.Union(downwardMatches).ToList();

        return (combinedMatches.Count >= minLength) ? combinedMatches : null;

    }

    // find all horizontal matches given a position (x,y) in the Board
    public List<GamePiece> FindHorizontalMatches(int startX, int startY, int minLength = 3)
    {
        List<GamePiece> rightMatches = FindMatches(startX, startY, new Vector2(1, 0), 2);
        List<GamePiece> leftMatches = FindMatches(startX, startY, new Vector2(-1, 0), 2);

        if (rightMatches == null)
        {
            rightMatches = new List<GamePiece>();
        }

        if (leftMatches == null)
        {
            leftMatches = new List<GamePiece>();
        }

        var combinedMatches = rightMatches.Union(leftMatches).ToList();

        return (combinedMatches.Count >= minLength) ? combinedMatches : null;

    }

    // find horizontal and vertical matches at a position (x,y) in the Board
    public List<GamePiece> FindMatchesAt(int x, int y, int minLength = 3)
    {
        List<GamePiece> horizMatches = FindHorizontalMatches(x, y, minLength);
        List<GamePiece> vertMatches = FindVerticalMatches(x, y, minLength);

        if (horizMatches == null)
        {
            horizMatches = new List<GamePiece>();
        }

        if (vertMatches == null)
        {
            vertMatches = new List<GamePiece>();
        }
        var combinedMatches = horizMatches.Union(vertMatches).ToList();
        return combinedMatches;
    }

    // find all matches given a list of GamePieces
    public List<GamePiece> FindMatchesAt(List<GamePiece> gamePieces, int minLength = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();

        foreach (GamePiece piece in gamePieces)
        {
            matches = matches.Union(FindMatchesAt(piece.xIndex, piece.yIndex, minLength)).ToList();
        }
        return matches;

    }

    // find all matches in the game Board
    public List<GamePiece> FindAllMatches()
    {
        List<GamePiece> combinedMatches = new List<GamePiece>();

        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                var matches = FindMatchesAt(i, j);
                combinedMatches = combinedMatches.Union(matches).ToList();
            }
        }
        return combinedMatches;
    }




}
