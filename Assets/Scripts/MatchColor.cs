using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class MatchColor : MonoBehaviour
{
    public bool usingSpriteShape;
    LevelGenerator lg;
    [SerializeField] SpriteShapeRenderer spriteShape;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Color[] colors;
    void Start()
    {
        lg = FindAnyObjectByType<LevelGenerator>();
        if(usingSpriteShape)
        {
            if(spriteShape != null) 
            {
                if (lg.biomeSpawning == LevelGenerator.Biome.Bog)
                {
                    spriteShape.color = colors[0];
                }
                if (lg.biomeSpawning == LevelGenerator.Biome.Cypress)
                {
                    spriteShape.color = colors[1];
                }
                if (lg.biomeSpawning == LevelGenerator.Biome.Amazon)
                {
                    spriteShape.color = colors[2];
                }
            }
        }
        else
        {
            if(sprite != null) 
            {
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
    }

}