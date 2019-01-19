using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Board))]
public class BoardCollapser : MonoBehaviour
{
    public Board board;

    private void Awake()
    {
        board = GetComponent<Board>();
    }


    // compresses a given column to remove any empty Tile spaces
    public List<GamePiece> CollapseColumn(int column, float collapseTime = 0.1f)
    {
        // running list of GamePieces that we need to move
        List<GamePiece> movingPieces = new List<GamePiece>();

        // loop from the bottom of the column
        for (int i = 0; i < board.height - 1; i++)
        {
            // if the current space is empty and not occupied by an Obstacle Tile...
            if (board.allGamePieces[column, i] == null && board.allTiles[column, i].tileType != TileType.Obstacle
            && board.boardQuery.IsUnblocked(column,i))
            {
                // ...loop from the space above it to the top of the column, to search for the next GamePiece
                for (int j = i + 1; j < board.height; j++)
                {
                    // if we find a GamePiece...
                    if (board.allGamePieces[column, j] != null)
                    {
                        // move the GamePiece downward to fill in the space and update the GamePiece array
                        board.allGamePieces[column, j].Move(column, i, collapseTime * (j - i));
                        board.allGamePieces[column, i] = board.allGamePieces[column, j];
                        board.allGamePieces[column, i].SetCoord(column, i);

                        // add our piece to the list of pieces that we are moving
                        if (!movingPieces.Contains(board.allGamePieces[column, i]))
                        {
                            movingPieces.Add(board.allGamePieces[column, i]);
                        }

                        board.allGamePieces[column, j] = null;

                        // break out of the loop and stop searching 
                        break;
                    }
                }
            }
        }
        // return our list of GamePieces that are being moved
        return movingPieces;
    }

    // collapse all columns given a list of GamePieces
    public List<GamePiece> CollapseColumn(List<GamePiece> gamePieces)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();

        List<int> columnsToCollapse = board.boardQuery.GetColumns(gamePieces);

        foreach (int column in columnsToCollapse)
        {
            movingPieces = movingPieces.Union(CollapseColumn(column)).ToList();
        }

        return movingPieces;
    }

    public List<GamePiece> CollapseColumn(List<int> columnsToCollapse)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();
        foreach (int column in columnsToCollapse)
        {
            movingPieces = movingPieces.Union(CollapseColumn(column)).ToList();
        }
        return movingPieces;
    }



}
