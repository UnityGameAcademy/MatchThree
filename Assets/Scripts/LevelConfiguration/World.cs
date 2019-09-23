using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfiguration", menuName = "LevelConfiguration/World")]
public class World : ScriptableObject
{
    public Level[] levels;

}
