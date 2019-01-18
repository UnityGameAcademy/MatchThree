using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoalCollected : LevelGoal
{

    public CollectionGoal[] collectionGoals;

    public void UpdateGoals(GamePiece pieceToCheck)
    {
        if (pieceToCheck != null)
        {
            foreach (CollectionGoal goal in collectionGoals)
            {
                if (goal != null)
                {
                    goal.CollectPiece(pieceToCheck);
                }
            }
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCollectionGoalLayout();
        }
    }

    bool AreGoalsComplete(CollectionGoal[] goals)
    {
        foreach (CollectionGoal g in goals)
        {
            if (g == null | goals == null)
            {
                return false;
            }

            if (goals.Length == 0)
            {
                return false;
            }

            if (g.numberToCollect != 0)
            {
                return false;
            }
        }
        return true;
    }

    public override bool IsGameOver()
    {
        if (AreGoalsComplete(collectionGoals) && ScoreManager.Instance != null)
        {
            int maxScore = scoreGoals[scoreGoals.Length - 1];
            if (ScoreManager.Instance.CurrentScore >= maxScore)
            {
                return true;
            }
        }
        if (levelCounter == LevelCounter.Timer)
        {
            return (timeLeft <= 0);
        }
        else
        {
            return (movesLeft <= 0);
        }
    }

    public override bool IsWinner()
    {
        if (ScoreManager.Instance != null)
        {
            return (ScoreManager.Instance.CurrentScore >= scoreGoals[0] && AreGoalsComplete(collectionGoals));
        }
        return false;
    }
}
