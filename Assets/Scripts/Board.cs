using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


// this is a generic GameObject that can be positioned at coordinate (x,y,z) when the game begins
[System.Serializable]
public class StartingObject
{
    public GameObject prefab;
    public int x;
    public int y;
    public int z;
}

[RequireComponent(typeof(BoardDeadlock))]
[RequireComponent(typeof(BoardShuffler))]
[RequireComponent(typeof(BoardInput))]
[RequireComponent(typeof(BoardQuery))]
[RequireComponent(typeof(BoardSetup))]
[RequireComponent(typeof(BoardTiles))]
[RequireComponent(typeof(BoardBomber))]
[RequireComponent(typeof(BoardFiller))]
[RequireComponent(typeof(BoardMatcher))]
[RequireComponent(typeof(BoardCollapser))]
[RequireComponent(typeof(BoardHighlighter))]

public class Board : MonoBehaviour
{

    // dimensions of board
    public int width;
    public int height;

    // margin outside Board for calculating camera field of view
    public int borderSize;

    // Prefab representing a single tile
    public GameObject tileNormalPrefab;

    // Prefab representing an empty, unoccupied tile
    public GameObject tileObstaclePrefab;

    // array of dot Prefabs
    public GameObject[] gamePiecePrefabs;

    // Prefab array for adjacent bombs
    public GameObject[] adjacentBombPrefabs;

    // Prefab array for column clearing bombs
    public GameObject[] columnBombPrefabs;

    // Prefab array for row clearing bombs
    public GameObject[] rowBombPrefabs;
	
    // Prefab bomb FX for clearing a single color from the Board
    public GameObject colorBombPrefab;

    // Prefab blocker, obscuring GamePieces
    public GameObject blockerPrefab;

    // the maximum number of Collectible game pieces allowed per Board
    public int maxCollectibles = 3;

    // the current number of Collectibles on the Board
    public int collectibleCount = 0;

    // this is the percentage for a top-row tile to get a collectible
    [Range(0, 1)]
    public float chanceForCollectible = 0.1f;

    // an array of our Collectible game objects
    public GameObject[] collectiblePrefabs;

    // reference to a Bomb created on the clicked Tile (first Tile clicked by mouse or finger)
    public GameObject clickedTileBomb;
	
    // reference to a Bomb created on the target Tile (Tile dragged into by mouse or finger)
    public GameObject targetTileBomb;

    // the time required to swap GamePieces between the Target and Clicked Tile
    public float swapTime = 0.5f;

    // array of all the Board's Tile pieces
    public Tile[,] allTiles;

    // array of all of the Board's GamePieces
    public GamePiece[,] allGamePieces;

    public Blocker[,] allBlockers;

    // Tile first clicked by mouse or finger
    public Tile clickedTile;

    // adjacent Tile dragged into by mouse or finger
    public Tile targetTile;

    // whether user input is currently allowed
    public bool playerInputEnabled = true;

    // manually positioned Tiles, placed before Board is filled
    public StartingObject[] startingTiles;

    // manually positioned GamePieces, placed before the Board is filled
    public StartingObject[] startingGamePieces;

    // manually positioned Blockers, placed before Board is filled
    public StartingObject[] startingBlockers;

    // manager class for particle effects
    public ParticleManager particleManager;

    // Y Offset used to make the pieces "fall" into place to fill the Board
    public int fillYOffset = 10;
	
    // time used to fill the Board
    public float fillMoveTime = 0.5f;

    // the current score multiplier, depending on how many chain reactions we have caused
    public int scoreMultiplier = 0;

    public bool isRefilling = false;

    public BoardDeadlock boardDeadlock;
    public BoardShuffler boardShuffler;
    public BoardSetup boardSetup;
    public BoardFiller boardFiller;
    public BoardHighlighter boardHighlighter;
    public BoardQuery boardQuery;
    public BoardInput boardInput;
    public BoardMatcher boardMatcher;
    public BoardCollapser boardCollapser;
    public BoardTiles boardTiles;
    public BoardBomber boardBomber;
    public BoardClearer boardClearer;

    public float delay = 0.2f;

    private void Awake()
    {

        boardDeadlock = GetComponent<BoardDeadlock>();
        boardShuffler = GetComponent<BoardShuffler>();
        boardSetup = GetComponent<BoardSetup>();
        boardFiller = GetComponent<BoardFiller>();
        boardHighlighter = GetComponent<BoardHighlighter>();
        boardQuery = GetComponent<BoardQuery>();
        boardInput = GetComponent<BoardInput>();
        boardMatcher = GetComponent<BoardMatcher>();
        boardCollapser = GetComponent<BoardCollapser>();
        boardTiles = GetComponent<BoardTiles>();
        boardBomber = GetComponent<BoardBomber>();
        boardClearer = GetComponent<BoardClearer>();
    }

    // invoked when we start the level
    void Start()
    {
        // initialize array of Tiles
        allTiles = new Tile[width, height];

        // initial array of GamePieces
        allGamePieces = new GamePiece[width, height];

        // initial array of Blockers
        allBlockers = new Blocker[width, height];

        // find the ParticleManager by Tag
        particleManager = GameObject.FindWithTag("ParticleManager").GetComponent<ParticleManager>();
    }



    // test if the Board is deadlocked
    public void TestDeadlock()
    {
        boardDeadlock.IsDeadlocked(allGamePieces, 3);
    }

    // invoke the ShuffleBoardRoutine (called by a button for testing)
    public void ShuffleBoard()
    {
        // only shuffle if the Board permits user input
        if (playerInputEnabled)
        {
            StartCoroutine(boardShuffler.ShuffleBoardRoutine(this));
        }
    }

    // swap two tiles
    public void SwitchTiles(Tile clickedTile, Tile targetTile)
    {
        StartCoroutine(SwitchTilesRoutine(clickedTile, targetTile));
    }

    // coroutine for swapping two Tiles
    public IEnumerator SwitchTilesRoutine(Tile tileA, Tile tileB)
    { 
        // if the player input is enabled...
        if (playerInputEnabled && !GameManager.Instance.IsGameOver)
        {
            // set the corresponding GamePieces to the clicked Tile and target Tile
            GamePiece clickedPiece = allGamePieces[tileA.xIndex, tileA.yIndex];
            GamePiece targetPiece = allGamePieces[tileB.xIndex, tileB.yIndex];

            if (targetPiece != null && clickedPiece != null)
            {
                // move the clicked GamePiece to the target GamePiece and vice versa
                clickedPiece.Move(tileB.xIndex, tileB.yIndex, swapTime);
                targetPiece.Move(tileA.xIndex, tileA.yIndex, swapTime);

                // wait for the swap time
                yield return new WaitForSeconds(swapTime);

                // find all matches for each GamePiece after the swap
                List<GamePiece> tileAMatches = boardMatcher.FindMatchesAt(tileA.xIndex, tileA.yIndex);
                List<GamePiece> tileBMatches = boardMatcher.FindMatchesAt(tileB.xIndex, tileB.yIndex);
                List<GamePiece> colorMatches = boardBomber.ProcessColorBombs(clickedPiece, targetPiece);


                // if we don't make any matches, then swap the pieces back
                if (tileBMatches.Count == 0 && tileAMatches.Count == 0 && colorMatches.Count == 0)
                {
                    clickedPiece.Move(tileA.xIndex, tileA.yIndex, swapTime);
                    targetPiece.Move(tileB.xIndex, tileB.yIndex, swapTime);
                }
                else
                {
                    // wait for our swap time
                    yield return new WaitForSeconds(swapTime);

                    // record the general vector of our swipe
                    Vector2 swipeDirection = new Vector2(tileB.xIndex - tileA.xIndex, tileB.yIndex - tileA.yIndex);

                    // drop bombs on either tile if necessary
                    boardBomber.ProcessBombs(tileA, tileB, clickedPiece, targetPiece, tileAMatches, tileBMatches);

                    // clear matches and refill the Board
                    List<GamePiece> piecesToClear = tileAMatches.Union(tileBMatches).ToList().Union(colorMatches).ToList();

                    // clear any blockers adjacent to matching pieces
                    boardClearer.ClearAdjacentBlockers(piecesToClear);

                    yield return StartCoroutine(ClearAndRefillBoardRoutine(piecesToClear));


                    // otherwise, we decrement our moves left
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.UpdateMoves();
                    }
                }
            }
        }
    }


    // clear a list of GamePieces and refill the Board
    public void ClearAndRefillBoard(List<GamePiece> gamePieces)
    {
        StartCoroutine(ClearAndRefillBoardRoutine(gamePieces));
    }

    // coroutine to clear GamePieces and collapse empty spaces, then refill the Board
    public IEnumerator ClearAndRefillBoardRoutine(List<GamePiece> gamePieces)
    {

        // disable player input so we cannot swap pieces while the Board is collapsing/refilling
        playerInputEnabled = false;
        isRefilling = true;

        // create a new List of GamePieces, using our initial list as a starting point
        List<GamePiece> matches = gamePieces;




        // store a score multiplier for chain reactions
        scoreMultiplier = 0;
        do
        {
            //  increment our score multiplier by 1 for each subsequent recursive call of ClearAndCollapseRoutine
            scoreMultiplier++;



            // run the coroutine to clear the Board and collapse any columns to fill in the spaces
            yield return StartCoroutine(ClearAndCollapseRoutine(matches));

            // pause one frame
            yield return null;

            // run the coroutine to refill the Board
            yield return StartCoroutine(boardFiller.RefillRoutine());

            // find any subsequent matches and repeat the process...
            matches = boardMatcher.FindAllMatches();

            yield return new WaitForSeconds(delay);

        }
        // .. while our list of matches still has GamePieces in it
		while (matches.Count != 0);

        // deadlock check
        if (boardDeadlock.IsDeadlocked(allGamePieces, 3))
        {
            yield return new WaitForSeconds(delay*5f);

            // shuffle the Board's normal pieces instead of Clearing out the whole Board
            yield return StartCoroutine(boardShuffler.ShuffleBoardRoutine(this));
           
            yield return new WaitForSeconds(delay*5f);

            yield return StartCoroutine(boardFiller.RefillRoutine());
        }


        // re-enable player input
        playerInputEnabled = true;
        isRefilling = false;
    }

    // coroutine to clear GamePieces from the Board and collapse any empty spaces
    IEnumerator ClearAndCollapseRoutine(List<GamePiece> gamePieces)
    {



        // list of GamePieces that will be moved
        List<GamePiece> movingPieces = new List<GamePiece>();

        // list of GamePieces that form matches
        List<GamePiece> matches = new List<GamePiece>();

        // slight delay before clearing anything
        yield return new WaitForSeconds(delay);

        bool isFinished = false;

        while (!isFinished)
        {



            // check the original list for bombs and append any pieces affected by these bombs
            List<GamePiece> bombedPieces = boardQuery.GetBombedPieces(gamePieces);

            // combine that with our original list
            gamePieces = gamePieces.Union(bombedPieces).ToList();

            // repeat this check once to see if we hit any more bombs 
            bombedPieces = boardQuery.GetBombedPieces(gamePieces);
            gamePieces = gamePieces.Union(bombedPieces).ToList();

            // store any collectibles that need to be cleared
            List<GamePiece> collectedPieces = boardQuery.GetCollectedPieces(gamePieces);

            // store what columns need to be collapsed
            List<int> columnsToCollapse = boardQuery.GetColumns(gamePieces);

            // clear the GamePieces, pass in the list of GamePieces affected by bombs as a separate list
            boardClearer.ClearPieceAt(gamePieces, bombedPieces);



            // clear any blockers directly underneath bombed pieces
            boardClearer.ClearBlockers(bombedPieces);


            // break any tiles under the cleared GamePieces
            boardTiles.BreakTileAt(gamePieces);

            // if we create a bomb on the clicked or target tiles, add it to our active GamePieces
            boardBomber.InitAllBombs();

            // short delay
            yield return new WaitForSeconds(delay);

            // collapse any columns with empty spaces and keep track of what pieces moved as a result
            movingPieces = boardCollapser.CollapseColumn(columnsToCollapse);

            // wait while these pieces fill in the gaps
            while (!boardQuery.IsCollapsed(movingPieces))
            {
                yield return null;
            }
            // extra delay after collapsing is finished
            yield return new WaitForSeconds(delay);

            // find any matches that form from collapsing...
            matches = boardMatcher.FindMatchesAt(movingPieces);

            //...and any collectibles that hit the bottom row...
            collectedPieces = boardQuery.FindCollectiblesAt(0, true);

            //... and add them to our list of GamePieces to clear
            matches = matches.Union(collectedPieces).ToList();


            // if we didn't make any matches from the collapse, then we're done
            if (matches.Count == 0)
            {
                isFinished = true;
                break;
            }
            // otherwise, increase our score multiplier for the chair reaction... 
            else
            {
                scoreMultiplier++;

                // ...play a bonus sound for making a chain reaction...
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayBonusSound();
                }

                // ...and run ClearAndCollapse again
                yield return StartCoroutine(ClearAndCollapseRoutine(matches));
            }
        }
        yield return null;
    }



}
