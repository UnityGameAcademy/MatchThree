using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Board))]
public class BoardFiller : MonoBehaviour
{
    public Board board;

    private void Awake()
    {
        board = GetComponent<Board>();
    }

    // creates a random GamePiece at position (x,y)
    public GamePiece FillRandomGamePieceAt(int x, int y, int falseYOffset = 0, float moveTime = 0.1f)
    {
        if (board == null)
            return null;

        if (board.boardQuery.IsWithinBounds(x, y))
        {
            GameObject randomPiece = Instantiate(board.boardQuery.GetRandomGamePiece(), Vector3.zero, Quaternion.identity) as GameObject;
            MakeGamePiece(randomPiece, x, y, falseYOffset, moveTime);
            return randomPiece.GetComponent<GamePiece>();
        }
        return null;
    }


    // create a random Collectible at position (x,y) with optional Y Offset
    public GamePiece FillRandomCollectibleAt(int x, int y, int falseYOffset = 0, float moveTime = 0.1f)
    {
        if (board == null)
            return null;

        if (board.boardQuery.IsWithinBounds(x, y))
        {
            GameObject randomPiece = Instantiate(board.boardQuery.GetRandomCollectible(), Vector3.zero, Quaternion.identity) as GameObject;
            MakeGamePiece(randomPiece, x, y, falseYOffset, moveTime);
            return randomPiece.GetComponent<GamePiece>();
        }
        return null;
    }



    // fills the empty spaces in the Board with an optional Y offset to make the pieces drop into place
    public void FillBoard(int falseYOffset = 0, float moveTime = 0.1f)
    {

        int maxInterations = 100;
        int iterations = 0;

        // loop through all spaces of the board
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {

                // if the space is unoccupied and does not contain an Obstacle tile             
                if (board.allGamePieces[i, j] == null && board.allTiles[i, j].tileType != TileType.Obstacle)
                {
                    //GamePiece piece = null;

                    // if we are at the top row, check if we can drop a collectible...
                    if (j == board.height - 1 && board.boardQuery.CanAddCollectible())
                    {

                        // add a random collectible prefab
                        FillRandomCollectibleAt(i, j, falseYOffset, moveTime);
                        board.collectibleCount++;
                    }

                    // ...otherwise, fill in a game piece prefab
                    else
                    {
                        FillRandomGamePieceAt(i, j, falseYOffset, moveTime);
                        iterations = 0;

                        // if we form a match while filling in the new piece...
                        while (board.boardQuery.HasMatchOnFill(i, j))
                        {
                            // remove the piece and try again
                            board.boardClearer.ClearPieceAt(i, j);
                            FillRandomGamePieceAt(i, j, falseYOffset, moveTime);

                            // check to prevent infinite loop
                            iterations++;

                            if (iterations >= maxInterations)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
    }




    // fill the Board using a known list of GamePieces instead of Instantiating new prefabs
    public void FillBoardFromList(List<GamePiece> gamePieces)
    {
        // create a first in-first out Queue to store the GamePieces in a pre-set order
        Queue<GamePiece> unusedPieces = new Queue<GamePiece>(gamePieces);

        // iterations to prevent infinite loop
        int maxIterations = 100;
        int iterations = 0;

        // loop through each position on the Board
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                // only fill in a GamePiece if 
                if (board.allGamePieces[i, j] == null && board.allTiles[i, j].tileType != TileType.Obstacle)
                {
                    // grab a new GamePiece from the Queue
                    board.allGamePieces[i, j] = unusedPieces.Dequeue();

                    // reset iteration count
                    iterations = 0;

                    // while a match forms when filling in a GamePiece...
                    while (board.boardQuery.HasMatchOnFill(i, j))
                    {
                        // put the GamePiece back into the Queue at the end of the line
                        unusedPieces.Enqueue(board.allGamePieces[i, j]);

                        // grab a new GamePiece from the Queue
                        board.allGamePieces[i, j] = unusedPieces.Dequeue();

                        // increment iterations each time we try to place a piece
                        iterations++;

                        // if our iterations exceeds limit, break out of the loop and move to next position
                        if (iterations >= maxIterations)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }



    // create a Bomb prefab at location (x,y)
    public GameObject MakeBomb(GameObject prefab, int x, int y)
    {
        if (board == null)
            return null;

        // only run the logic on valid GameObject and if we are within the boundaries of the Board
        if (prefab != null && board.boardQuery.IsWithinBounds(x, y))
        {
            // create a Bomb and initialize it; parent it to the Board

            GameObject bomb = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
            bomb.GetComponent<Bomb>().Init(board);
            bomb.GetComponent<Bomb>().SetCoord(x, y);
            bomb.transform.parent = transform;
            return bomb;
        }
        return null;
    }


    public void MakeColorBombBooster(int x, int y)
    {
        if (board == null)
            return;

        if (board.boardQuery.IsWithinBounds(x, y))
        {
            GamePiece pieceToReplace = board.allGamePieces[x, y];

            if (pieceToReplace != null)
            {
                board.boardClearer.ClearPieceAt(x, y);
                GameObject bombObject = MakeBomb(board.colorBombPrefab, x, y);
                board.boardBomber.InitBomb(bombObject);
            }
        }
    }


    // Creates a GameObject prefab at certain (x,y,z) coordinate
    public void MakeTile(GameObject prefab, int x, int y, int z = 0)
    {
        if (board == null)
            return;

        // only run the logic on valid GameObject and if we are within the boundaries of the Board
        if (prefab != null && board.boardQuery.IsWithinBounds(x, y))
        {
            // create a Tile at position (x,y,z) with no rotations; rename the Tile and parent it 

            // to the Board, then initialize the Tile into the m_allTiles array

            GameObject tile = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity) as GameObject;
            tile.name = "Tile (" + x + "," + y + ")";
            board.allTiles[x, y] = tile.GetComponent<Tile>();
            tile.transform.parent = transform;
            board.allTiles[x, y].Init(x, y, board);
        }
    }

    // Creates a GamePiece prefab at a certain (x,y,z) coordinate
    public void MakeGamePiece(GameObject prefab, int x, int y, int falseYOffset = 0, float moveTime = 0.1f)
    {
        if (board == null)
            return;

        // only run the logic on valid GameObject and if we are within the boundaries of the Board
        if (prefab != null && board.boardQuery.IsWithinBounds(x, y))
        {
            prefab.GetComponent<GamePiece>().Init(board);
            PlaceGamePiece(prefab.GetComponent<GamePiece>(), x, y);

            // allows the GamePiece to be placed higher than the Board, so it can be moved into place


            if (falseYOffset != 0)
            {
                prefab.transform.position = new Vector3(x, y + falseYOffset, 0);
                prefab.GetComponent<GamePiece>().Move(x, y, moveTime);
            }

            // parent the GamePiece to the Board
            prefab.transform.parent = transform;
        }
    }


    // place a GamePiece onto the Board at position (x,y)
    public void PlaceGamePiece(GamePiece gamePiece, int x, int y)
    {
        if (board == null)
            return;

        if (gamePiece == null)
        {
            Debug.LogWarning("BOARD:  Invalid GamePiece!");
            return;
        }

        gamePiece.transform.position = new Vector3(x, y, 0);
        gamePiece.transform.rotation = Quaternion.identity;

        if (board.boardQuery.IsWithinBounds(x, y))
        {
            board.allGamePieces[x, y] = gamePiece;
        }

        gamePiece.SetCoord(x, y);
    }

    // coroutine to refill the Board
    public IEnumerator RefillRoutine()
    {
        board.boardFiller.FillBoard(board.fillYOffset, board.fillMoveTime);

        yield return null;
    }



}
