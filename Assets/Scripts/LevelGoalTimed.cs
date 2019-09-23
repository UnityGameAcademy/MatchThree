using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoalTimed : LevelGoal
{
    public override void Start()
    {
        levelCounter = LevelCounter.Timer;
        base.Start();
    }

    // did we win?
    public override bool IsWinner()
    {
        // we scored higher than the lowest score goal, we win
        if (ScoreManager.Instance != null)
        {
            return (ScoreManager.Instance.CurrentScore >= scoreGoals[0]);
        }
        return false;
    }

    // is the game over?
    public override bool IsGameOver()
    {
        int maxScore = scoreGoals[scoreGoals.Length - 1];

        // if we score higher than the last score goal, end the game
        if (ScoreManager.Instance.CurrentScore >= maxScore)
        {
            return true;
        }

        // end the game if we have no moves left
        return (timeLeft <= 0);
    }


}
