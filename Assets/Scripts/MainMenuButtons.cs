using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void SpeciesMenu()
    {
        SceneManager.LoadScene("SpeciesMenu");
    }
    public void Return()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void SelectDefault()
    {
        SceneManager.LoadScene("MainMenu");
        PlayerController.species = PlayerController.Species.Default;
    }
    public void SelectTreefrog()
    {
        SceneManager.LoadScene("MainMenu");
        PlayerController.species = PlayerController.Species.Treefrog;
    }
    public void SelectFroglet()
    {
        SceneManager.LoadScene("MainMenu");
        PlayerController.species = PlayerController.Species.Froglet;
    }
    public void SelectBullfrog()
    {
        SceneManager.LoadScene("MainMenu");
        PlayerController.species = PlayerController.Species.BullFrog;
    }
    public void SelectPoisonDartFrog()
    {
        SceneManager.LoadScene("MainMenu");
        PlayerController.species = PlayerController.Species.PoisonDartFrog;
    }
}
