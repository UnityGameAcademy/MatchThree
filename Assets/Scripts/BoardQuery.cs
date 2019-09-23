using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Board))]
public class BoardQuery : MonoBehaviour
{
    public Board board;

    private void Awake()
    {
        board = GetComponent<Board>();
    }

    // return a random object from an array of GameObjects
    public GameObject GetRandomObject(GameObject[] objectArray)
    {
        if (board == null)
            return null;

        int randomIdx = Random.Range(0, objectArray.Length);
        if (objectArray[randomIdx] == null)
        {
            Debug.LogWarning("ERROR:  BOARD.GetRandomObject at index " + randomIdx + "does not contain a valid GameObject!");
        }
        return objectArray[randomIdx];
    }

    // return a random GamePiece
    public GameObject GetRandomGamePiece()
    {
        if (board == null)
            return null;

        return GetRandomObject(board.gamePiecePrefabs);
    }

    // return a random Collectible
    public GameObject GetRandomCollectible()
    {
        if (board == null)
            return null;

        return GetRandomObject(board.collectiblePrefabs);
    }

    // given a List of GamePieces, return a list of columns by index number
    public List<int> GetColumns(List<GamePiece> gamePieces)
    {
        List<int> columns = new List<int>();

        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                if (!columns.Contains(piece.xIndex))
                {
                    columns.Add(piece.xIndex);
                }
            }
        }
        return columns;
    }

    // gets a list of GamePieces in a specified row
    public List<GamePiece> GetRowPieces(int row)
    {
        if (board == null)
            return null;

        List<GamePiece> gamePieces = new List<GamePiece>();

        for (int i = 0; i < board.width; i++)
        {
            if (board.allGamePieces[i, row] != null)
            {
                gamePieces.Add(board.allGamePieces[i, row]);
            }
        }
        return gamePieces;
    }

    // gets a list of GamePieces in a specified column
   public List<GamePiece> GetColumnPieces(int column)
    {
        if (board == null)
            return null;

        List<GamePiece> gamePieces = new List<GamePiece>();

        for (int i = 0; i < board.height; i++)
        {
            if (board.allGamePieces[column, i] != null)
            {
                gamePieces.Add(board.allGamePieces[column, i]);
            }
        }
        return gamePieces;
    }

     //get all GamePieces adjacent to a position (x,y) for adjacent 3 x3 bomb (include diagonals)
    public List<GamePiece> GetAdjacentPieces(int x, int y, int offset = 1)
    {
        if (board == null)
            return null;

        List<GamePiece> gamePieces = new List<GamePiece>();

        for (int i = x - offset; i <= x + offset; i++)
        {
            for (int j = y - offset; j <= y + offset; j++)
            {
                if (board.boardQuery.IsWithinBounds(i, j))
                {
                    gamePieces.Add(board.allGamePieces[i, j]);
                }

            }
        }

        return gamePieces;
    }

    // get blocker adjacent to postion (x,y) (does not include diagonals)
    public List<Blocker> GetAdjacentBlockers(int x, int y)
    {
        if (board == null)
            return new List<Blocker>(); ;

        List<Blocker> blockers = new List<Blocker>();

        Vector2Int[] directions = {new Vector2Int(-1,0), new Vector2Int(1,0), new Vector2Int(0,1), new Vector2Int(0,-1) };

        foreach (Vector2Int d in directions)
        {
            int i = x + d.x;
            int j = y + d.y;
            if (board.boardQuery.IsWithinBounds(i,j) && board.allBlockers[i,j] != null)
            {
                blockers.Add(board.allBlockers[i, j]);
            }

        }
        return blockers;

    }



    // given a list of GamePieces, returns a new List of GamePieces that would be destroyed by bombs from the original list
    public List<GamePiece> GetBombedPieces(List<GamePiece> gamePieces)
    {

        if (board == null)
            return null;

        // list of GamePieces to clear
        List<GamePiece> allPiecesToClear = new List<GamePiece>();

        // loop through the original list of GamePieces
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                // list of GamePieces to be cleared by bombs
                List<GamePiece> piecesToClear = new List<GamePiece>();

                // check each GamePiece if it has a Bomb
                Bomb bomb = piece.GetComponent<Bomb>();

                // if so, get a list of GamePieces affected
                if (bomb != null)
                {
                    switch (bomb.bombType)
                    {
                        case BombType.Column:
                            piecesToClear = GetColumnPieces(bomb.xIndex);
                            break;
                        case BombType.Row:
                            piecesToClear = GetRowPieces(bomb.yIndex);
                            break;
                        case BombType.Adjacent:
                            piecesToClear = GetAdjacentPieces(bomb.xIndex, bomb.yIndex, 1);
                            break;
                        case BombType.Color:

                            break;
                    }

                    // keep a running list of all GamePieces affected by bombs
                    allPiecesToClear = allPiecesToClear.Union(piecesToClear).ToList();

                    // remove any collectibles from our list
                    allPiecesToClear = board.boardQuery.RemoveCollectiblesFromList(allPiecesToClear);

                }
            }
        }
        // return a list of all GamePieces that would be affected by any bombs from the original list
        return allPiecesToClear;
    }

    // given a list of GamePieces, return a subset of Collectibles plus any Collectibles that have reached the bottom
    public List<GamePiece> GetCollectedPieces(List<GamePiece> gamePieces)
    {
        // find any collectibles that have reached the bottom and decrement the number of collectibles needed
        List<GamePiece> collectedPieces = FindCollectiblesAt(0, true);

        // find blockers destroyed by bombs
        List<GamePiece> allCollectibles = FindAllCollectibles();
        List<GamePiece> blockers = gamePieces.Intersect(allCollectibles).ToList();

        // add blockers to list of collected pieces
        collectedPieces = collectedPieces.Union(blockers).ToList();

        // decrement cleared collectibles/blockers
        board.collectibleCount -= collectedPieces.Count;

        // add these collectibles to the list of GamePieces to clear
        gamePieces = gamePieces.Union(collectedPieces).ToList();
        return collectedPieces;
    }



    // given a list of GamePieces, return a list without the Collectibles 
    public List<GamePiece> RemoveCollectiblesFromList(List<GamePiece> gamePieces)
    {

        List<GamePiece> collectiblePieces = board.boardQuery.FindAllCollectibles();
        List<GamePiece> piecesToRemove = new List<GamePiece>();

        foreach (GamePiece piece in collectiblePieces)
        {
            Collectible collectibleComponent = piece.GetComponent<Collectible>();
            if (collectibleComponent != null)
            {

                if (!collectibleComponent.clearedByBomb)
                {
                    piecesToRemove.Add(piece);
                }
            }
        }
        return gamePieces.Except(piecesToRemove).ToList();
    }

    // returns true if within the boundaries of the Board, otherwise returns false
    public bool IsWithinBounds(int x, int y)
    {
        if (board == null)
            return false;

        return (x >= 0 && x < board.width && y >= 0 && y < board.height);
    }

    // is the Tile blocked by a blocker?
    public bool IsUnblocked(int x, int y)
    {
        if (board == null)
            return false;

        return (board.allBlockers[x, y] == null);
    }

    // return if the Bomb is a Color Bomb
    public bool IsColorBomb(GamePiece gamePiece)
    {
        Bomb bomb = gamePiece.GetComponent<Bomb>();

        if (bomb != null)
        {
            return (bomb.bombType == BombType.Color);
        }
        return false;
    }

    // check if List of matching GamePieces forms an L shaped match
    public bool IsCornerMatch(List<GamePiece> gamePieces)
    {
        bool vertical = false;
        bool horizontal = false;
        int xStart = -1;
        int yStart = -1;

        // loop through all of the pieces
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                // if this is the very first piece we are checking, save its x and y index
                if (xStart == -1 || yStart == -1)
                {
                    xStart = piece.xIndex;
                    yStart = piece.yIndex;
                    continue;
                }

                // otherwise, see if GamePiece is in line horizontally with the first piece
                if (piece.xIndex != xStart && piece.yIndex == yStart)
                {
                    horizontal = true;
                }

                // check if are in line vertically with the first piece
                if (piece.xIndex == xStart && piece.yIndex != yStart)
                {
                    vertical = true;
                }
            }
        }

        // return true only if pieces align both horizontally and vertically with first piece
        return (horizontal && vertical);

    }

    // return true if one Tile is adjacent to another, otherwise returns false
    public bool IsNextTo(Tile start, Tile end)
    {

        return (Mathf.Abs(start.xIndex - end.xIndex) + Mathf.Abs(start.yIndex - end.yIndex) == 1);

    }

    // checks if the GamePieces have reached their destination positions on collapse
    public bool IsCollapsed(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                if (piece.transform.position.y - (float)piece.yIndex > 0.001f)
                {
                    return false;
                }

                if (piece.transform.position.x - (float)piece.xIndex > 0.001f)
                {
                    return false;
                }
            }
        }
        return true;
    }

    // check if we form a match down or to the left when filling the Board
    // note: this does not take into account StartingGamePieces
    public bool HasMatchOnFill(int x, int y, int minLength = 3)
    {
        // find matches to the left
        List<GamePiece> leftMatches = board.boardMatcher.FindMatches(x, y, new Vector2(-1, 0), minLength);

        // find matches downward
        List<GamePiece> downwardMatches = board.boardMatcher.FindMatches(x, y, new Vector2(0, -1), minLength);

        if (leftMatches == null)
        {
            leftMatches = new List<GamePiece>();
        }

        if (downwardMatches == null)
        {
            downwardMatches = new List<GamePiece>();
        }

        // return whether matches were found
        return (leftMatches.Count > 0 || downwardMatches.Count > 0);

    }
    // determines if we can add a Collectible based on probability
    public bool CanAddCollectible()
    {

        if (board == null)
            return false;

        return (Random.Range(0f, 1f) <= board.chanceForCollectible && board.collectiblePrefabs.Length > 0 && board.collectibleCount < board.maxCollectibles);
    }

    // find all Collectibles at a certain row
    public List<GamePiece> FindCollectiblesAt(int row, bool clearedAtBottomOnly = false)
    {
        if (board == null)
            return null;

        List<GamePiece> foundCollectibles = new List<GamePiece>();

        for (int i = 0; i < board.width; i++)
        {
            if (board.allGamePieces[i, row] != null)
            {
                Collectible collectibleComponent = board.allGamePieces[i, row].GetComponent<Collectible>();

                if (collectibleComponent != null)
                {
                    // only return the Collectible if it can be cleared by Bomb OR it can be cleared at the bottom of the Board
                    // and has reached the bottom
                    if (!clearedAtBottomOnly || (clearedAtBottomOnly && collectibleComponent.clearedAtBottom))
                    {
                        foundCollectibles.Add(board.allGamePieces[i, row]);
                    }
                }
            }
        }
        return foundCollectibles;
    }

    // find all Collectibles in the Board
    public List<GamePiece> FindAllCollectibles(bool clearedAtBottomOnly = false)
    {
        List<GamePiece> foundCollectibles = new List<GamePiece>();

        for (int i = 0; i < board.height; i++)
        {
            List<GamePiece> collectibleRow = FindCollectiblesAt(i, clearedAtBottomOnly);
            foundCollectibles = foundCollectibles.Union(collectibleRow).ToList();
        }

        return foundCollectibles;
    }

    // find all GamePieces on the Board with a certain MatchValue
    public List<GamePiece> FindAllMatchValue(MatchValue mValue)
    {
        if (board == null)
            return null;

        List<GamePiece> foundPieces = new List<GamePiece>();

        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allGamePieces[i, j] != null)
                {
                    if (board.allGamePieces[i, j].matchValue == mValue)
                    {
                        foundPieces.Add(board.allGamePieces[i, j]);
                    }
                }
            }
        }
        return foundPieces;
    }

    // given a list of GamePieces, return the first valid MatchValue found
    public MatchValue FindMatchValue(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                return piece.matchValue;
            }
        }
        return MatchValue.None;
    }

    // given an array of prefabs, return a GameObject whose GamePiece component has a given matchValue
    public GameObject FindGamePieceByMatchValue(GameObject[] prefabs, MatchValue matchValue)
    {
        if (matchValue == MatchValue.None)
        {
            return null;
        }

        foreach (GameObject go in prefabs)
        {
            GamePiece piece = go.GetComponent<GamePiece>();

            if (piece != null)
            {
                if (piece.matchValue == matchValue)
                {
                    return go;
                }
            }
        }

        return null;

    }

}
