using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeDependece : MonoBehaviour
{
    [SerializeField] string dependence; //The biome that the level part must be in
    [SerializeField] GameObject replacement; //What the level part will be replaced with if it spawns in the wrong biome
    [SerializeField] bool destroyOnTransition; //Destroy the gameobject when it is spawned on a transition slide
    private void Start()
    {
        if (destroyOnTransition)
        {
            StartCoroutine(CheckForTransitionSlide());
        }
        //Determine what biome the level part was spawned at by scanning for the mud under it then looking at that mud's parent name and then seeing what type of riverbed it is.
        //If the level part is in a biome it does not belong in, it replaces itself with the corresponding correct level part and destroys itself.
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, Vector2.down);
        for (int i = 0; i < hit.Length; i++) 
        {
            if (hit[i].collider.transform.gameObject.layer == 3)  
            {
                if (hit[i].transform.parent.gameObject.name.Substring(0,6) == "Amazon")
                {
                    if (!dependence.Equals("Amazon"))
                    {
                        if (replacement != null)
                        {
                            Instantiate(replacement, transform.position, Quaternion.identity);
                        }
                        Destroy(gameObject);
                    }
                }        
                if (hit[i].transform.parent.gameObject.name.Substring(0,3) == "Bog")
                {
                    if (!dependence.Equals("Bog"))
                    {
                        if (replacement != null)
                        {
                            Instantiate(replacement, transform.position, Quaternion.identity);
                        }
                        Destroy(gameObject);
                    }
                }
                if (hit[i].transform.parent.gameObject.name.Substring(0, 7) == "Cypress")
                {
                    if (!dependence.Equals("Cypress"))
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
    IEnumerator CheckForTransitionSlide() 
    {
        yield return new WaitForSeconds(4);
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, Vector2.down);
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].transform.gameObject.layer == 14)
            {
                Destroy(gameObject);
            }
        }
        StartCoroutine(CheckForTransitionSlide());
    }
}
