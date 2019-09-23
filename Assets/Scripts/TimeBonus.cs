using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GamePiece))]
public class TimeBonus : MonoBehaviour
{
    [Range(0,5)]
    public int bonusValue = 5;

    [Range(0f,1f)]
    public float chanceForBonus = 0.1f;

    public GameObject bonusGlow;
    public Material[] bonusMaterials;

    public GameObject ringGlow;


    void Start()
    {
        // generate a random number to check against chance for bonus
        float random = Random.Range(0f, 1f);

        // disable the Time Bonus if we exceed chanceForBonus
        if (random > chanceForBonus)
        {
            bonusValue = 0;
        }

        // if we are not using a timed, level disable the TimeBonus
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.LevelGoal.levelCounter == LevelCounter.Moves)
            {
                bonusValue = 0;
            }
        }

        // activate/deactive Ring Glow and Bonus Glow
        SetActive(bonusValue != 0);

        // if TimeBonus is active, set the particle material based on the bonusValue
        if (bonusValue != 0)
        {
            SetupMaterial(bonusValue - 1, bonusGlow);
        }

    }

    // activate or deactive bonusGlow and ringGlow effects
    void SetActive(bool state)
    {
        if (bonusGlow != null)
        {
            bonusGlow.SetActive(state);
        }

        if (ringGlow != null)
        {
            ringGlow.SetActive(state);
        }
    }
     
    // set the material depending on the bonus value
    void SetupMaterial(int value, GameObject bonusGlow)
    {
        // avoids Out of Range error
        int clampedValue = Mathf.Clamp(value, 0, bonusMaterials.Length - 1);

        // set the BonusGlow renderer to use the proper material
        if (bonusMaterials[clampedValue] != null)
        {
            if (bonusGlow != null)
            {
                ParticleSystemRenderer bonusGlowRenderer = bonusGlow.GetComponent<ParticleSystemRenderer>();
                bonusGlowRenderer.material = bonusMaterials[clampedValue];
            }
        }

    }
}
