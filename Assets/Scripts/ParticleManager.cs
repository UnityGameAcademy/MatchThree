using UnityEngine;
using System.Collections;

// this manager class handles particle effects
public class ParticleManager : MonoBehaviour
{
    // prefab GameObject for clearing a GamePiece
	public GameObject clearFXPrefab;

    // prefab GameObject for breaking a Tile 
	public GameObject breakFXPrefab;

    // prefab GameObject for breaking a Doublebreak Tile effect
	public GameObject doubleBreakFXPrefab;

    // prefab GameObject for the bomb explosion effect
	public GameObject bombFXPrefab;

    // play the clear GamePiece effect
	public void ClearPieceFXAt(int x, int y, int z = 0)
	{
		if (clearFXPrefab != null)
		{
			GameObject clearFX = Instantiate(clearFXPrefab, new Vector3(x,y,z), Quaternion.identity) as GameObject;

			ParticlePlayer particlePlayer = clearFX.GetComponent<ParticlePlayer>();

			if (particlePlayer !=null)
			{
				particlePlayer.Play();
			}
		}
	}

    // play the Break Tile effect (either single or double effect depending on the Tile)
	public void BreakTileFXAt(int breakableValue, int x, int y, int z = 0)
	{
		GameObject breakFX = null;
		ParticlePlayer particlePlayer = null;

		if (breakableValue > 1)
		{
			if (doubleBreakFXPrefab !=null)
			{
				breakFX = Instantiate(doubleBreakFXPrefab, new Vector3(x,y,z), Quaternion.identity) as GameObject;
			}
		}
		else
		{
			if (breakFXPrefab !=null)
			{
				breakFX = Instantiate(breakFXPrefab, new Vector3(x,y,z), Quaternion.identity) as GameObject;
			}
		}

		if (breakFX !=null)
		{
			particlePlayer = breakFX.GetComponent<ParticlePlayer>();

			if (particlePlayer !=null)
			{
				particlePlayer.Play();
			}
		}
	}

    // play the bomb effect
	public void BombFXAt(int x, int y, int z = 0)
	{
		if (bombFXPrefab !=null)
		{
			GameObject bombFX = Instantiate(bombFXPrefab, new Vector3(x, y, z), Quaternion.identity) as GameObject;
			ParticlePlayer particlePlayer = bombFX.GetComponent<ParticlePlayer>();

			if (particlePlayer !=null)
			{
				particlePlayer.Play();
			}
		}
	}

}
