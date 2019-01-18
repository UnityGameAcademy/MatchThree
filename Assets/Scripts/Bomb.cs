using UnityEngine;
using System.Collections;

// the various bombs available in the Game
public enum BombType
{
	None,
	Column,
	Row,
	Adjacent,
	Color

}

// the Bomb is just a GamePiece with a BombType exposed
public class Bomb : GamePiece 
{
	public BombType bombType;

}
