using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Singleton manager class to keep track of our score
public class ScoreManager : Singleton<ScoreManager> 
{
    // our current score
	int m_currentScore = 0;

    // read-only Property to refer to our current score publicly
    public int CurrentScore
    {
        get
        {
            return m_currentScore;
        }
    }

    // used to hold a "counter" show the score increment upward to current score
	int m_counterValue = 0;

    // amount to increment the counter
	int m_increment = 5;

    // UI.Text that shows the score
	public Text scoreText;


	public float countTime = 1f;

	// Use this for initialization
	void Start () 
	{
		UpdateScoreText (m_currentScore);
	}

    // update the UI score Text
	public void UpdateScoreText(int scoreValue)
	{
		if (scoreText != null) 
		{
			scoreText.text = scoreValue.ToString ();
		}
	}

    // add a value to the current score
	public void AddScore(int value)
	{
		m_currentScore += value;
		StartCoroutine (CountScoreRoutine ());
	}

    // coroutine shows the score counting up the currentScore value
	IEnumerator CountScoreRoutine()
	{
		int iterations = 0;

        // if we are less than the current score (and we haven't taken too long to get there)...
		while (m_counterValue < m_currentScore && iterations < 100000) 
		{
			m_counterValue += m_increment;
			UpdateScoreText (m_counterValue);
			iterations++;
			yield return null;
		}

        //... set the counter equal to the currentScore and update the score Text
		m_counterValue = m_currentScore;
		UpdateScoreText (m_currentScore);

	}

}
