using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class MatchColor : MonoBehaviour
{
    LevelGenerator lg;
    [SerializeField] SpriteShapeRenderer sprite;
    [SerializeField] Color[] colors;
    void Start()
    {
        lg = FindAnyObjectByType<LevelGenerator>();

        if (lg.biomeSpawning == LevelGenerator.Biome.Bog) 
        {
            sprite.color = colors[0];
        }
        if (lg.biomeSpawning == LevelGenerator.Biome.Cypress) 
        {
            sprite.color = colors[1];
        }
        if (lg.biomeSpawning == LevelGenerator.Biome.Amazon) 
        {
            sprite.color = colors[2];
        }
    }
}
