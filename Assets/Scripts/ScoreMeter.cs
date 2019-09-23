using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ScoreMeter : MonoBehaviour
{
    // reference to slider component
    public Slider slider;

    // array of ScoreStar components (defaults to three stars)
    public ScoreStar[] scoreStars = new ScoreStar[3];
    
    // reference to LevelGoal component
    LevelGoal m_levelGoal;
    
    // reference to maximum score (largest scoring goal)
    int m_maxScore;

    void Awake()
    {
        // populate the Slider component
        slider = GetComponent<Slider>();
    }

    // position the ScoreStars automatically
    public void SetupStars(LevelGoal levelGoal)
    {
        // if levelGoal is invalid, return immediately
        if (levelGoal == null)
        {
            Debug.LogWarning("SCOREMETER Invalid level goal!");
            return;
        }

        // cache the LevelGoal component for later
        m_levelGoal = levelGoal;

        // set the maximum score goal
        m_maxScore = m_levelGoal.scoreGoals[m_levelGoal.scoreGoals.Length - 1];

        // get the slider's RectTransform width
        float sliderWidth = slider.GetComponent<RectTransform>().rect.width;

        // avoid divide by zero error
        if (m_maxScore > 0)
        {
            // loop through our scoring goals
            for (int i = 0; i < levelGoal.scoreGoals.Length; i++)
            {
                // if the corresponding ScoreStar exists...
                if (scoreStars[i] != null)
                {
                    // set the x value based on the ratio of the scoring goal over the maximum score
                    float newX = (sliderWidth * levelGoal.scoreGoals[i] / m_maxScore) - (sliderWidth * 0.5f);

                    // move the ScoreStar's RectTransform
                    RectTransform starRectXform = scoreStars[i].GetComponent<RectTransform>();

                    if (starRectXform != null)
                    {
                        starRectXform.anchoredPosition = new Vector2(newX, starRectXform.anchoredPosition.y);
                    }
                }
            }

        }

    }

    // Update the ScoreMeter 
    public void UpdateScoreMeter(int score, int starCount)
    {
        if (m_levelGoal != null)
        {
            // adjust the slider fill area (cast as floats, otherwise will become zero)
            slider.value = (float) score / (float) m_maxScore;
        }

        // activate each star based on current star count
        for (int i = 0; i < starCount; i++)
        {
            if (scoreStars[i] != null)
            {
                scoreStars[i].Activate();
            }
        }
    }
}
