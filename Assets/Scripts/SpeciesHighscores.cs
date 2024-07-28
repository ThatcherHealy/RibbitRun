using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SpeciesHighscores : MonoBehaviour
{
    [SerializeField] TextMeshPro[] defaultHighscores;
    [SerializeField] TextMeshPro[] treeFrogHighscores;
    [SerializeField] TextMeshPro[] frogletHighscores;
    [SerializeField] TextMeshPro[] bullfrogHighscores;
    [SerializeField] TextMeshPro[] poisonDartFrogHighscores;
    [SerializeField] TextMeshPro[] defaultHardModeHighscores;
    [SerializeField] TextMeshPro[] treeFrogHardModeHighscores;
    [SerializeField] TextMeshPro[] frogletHardModeHighscores;
    [SerializeField] TextMeshPro[] bullfrogHardModeHighscores;
    [SerializeField] TextMeshPro[] poisonDartFrogHardModeHighscores;
    [SerializeField] Color hardModeHighscoreColor;


    private void Update()
    {
        InitializeHighscores();
        HighlightHighestScore();
    }
    void InitializeHighscores()
    {
        foreach (TextMeshPro defaultHighscore in defaultHighscores)
        {
            defaultHighscore.text = PlayerPrefs.GetInt("DefaultHighscore", 0).ToString();
            if (defaultHighscore.text == "0")
                defaultHighscore.text = "";
        }
        foreach (TextMeshPro treeFrogHighscore in treeFrogHighscores)
        {
            treeFrogHighscore.text = PlayerPrefs.GetInt("TreeFrogHighscore", 0).ToString();
            if (treeFrogHighscore.text == "0" && PlayerPrefs.GetInt("TreeFrogClaimed", 0) != 1)
                treeFrogHighscore.text = "";
        }
        foreach (TextMeshPro frogletHighscore in frogletHighscores)
        {
            frogletHighscore.text = PlayerPrefs.GetInt("FrogletHighscore", 0).ToString();
            if (frogletHighscore.text == "0" && PlayerPrefs.GetInt("FrogletClaimed", 0) != 1)
                frogletHighscore.text = "";
        }
        foreach (TextMeshPro bullfrogHighscore in bullfrogHighscores)
        {
            bullfrogHighscore.text = PlayerPrefs.GetInt("BullfrogHighscore", 0).ToString();
            if (bullfrogHighscore.text == "0" && PlayerPrefs.GetInt("BullfrogClaimed", 0) != 1)
                bullfrogHighscore.text = "";
        }
        foreach (TextMeshPro poisonDartFrogHighscore in poisonDartFrogHighscores)
        {
            poisonDartFrogHighscore.text = PlayerPrefs.GetInt("PoisonDartFrogHighscore", 0).ToString();
            if (poisonDartFrogHighscore.text == "0" && PlayerPrefs.GetInt("PoisonDartFrogClaimed", 0) != 1)
                poisonDartFrogHighscore.text = "";
        }

        //HARD MODE

            foreach (TextMeshPro defaultHighscore in defaultHardModeHighscores)
            {
                defaultHighscore.text = PlayerPrefs.GetInt("DefaultHardModeHighscore", 0).ToString();
                if (PlayerPrefs.GetInt("HardModeUnlocked") == 0)
                    defaultHighscore.text = "";
            }
            foreach (TextMeshPro treeFrogHighscore in treeFrogHardModeHighscores)
            {
                treeFrogHighscore.text = PlayerPrefs.GetInt("TreeFrogHardModeHighscore", 0).ToString();
                if (treeFrogHighscore.text == "0" && PlayerPrefs.GetInt("TreeFrogClaimed", 0) != 1 || PlayerPrefs.GetInt("HardModeUnlocked") == 0)
                    treeFrogHighscore.text = "";
            }
            foreach (TextMeshPro frogletHighscore in frogletHardModeHighscores)
            {
                frogletHighscore.text = PlayerPrefs.GetInt("FrogletHardModeHighscore", 0).ToString();
                if (frogletHighscore.text == "0" && PlayerPrefs.GetInt("FrogletClaimed", 0) != 1 || PlayerPrefs.GetInt("HardModeUnlocked") == 0)
                    frogletHighscore.text = "";
            }
            foreach (TextMeshPro bullfrogHighscore in bullfrogHardModeHighscores)
            {
                bullfrogHighscore.text = PlayerPrefs.GetInt("BullfrogHardModeHighscore", 0).ToString();
                if (bullfrogHighscore.text == "0" && PlayerPrefs.GetInt("BullfrogClaimed", 0) != 1 || PlayerPrefs.GetInt("HardModeUnlocked") == 0)
                    bullfrogHighscore.text = "";
            }
            foreach (TextMeshPro poisonDartFrogHighscore in poisonDartFrogHardModeHighscores)
            {
                poisonDartFrogHighscore.text = PlayerPrefs.GetInt("PoisonDartFrogHardModeHighscore", 0).ToString();
                if (poisonDartFrogHighscore.text == "0" && PlayerPrefs.GetInt("PoisonDartFrogClaimed", 0) != 1 || PlayerPrefs.GetInt("HardModeUnlocked") == 0)
                    poisonDartFrogHighscore.text = "";
            }
        
    }
    void HighlightHighestScore() 
    {
        int[] numbers = { PlayerPrefs.GetInt("DefaultHighscore", 0), PlayerPrefs.GetInt("TreeFrogHighscore", 0), PlayerPrefs.GetInt("FrogletHighscore", 0), PlayerPrefs.GetInt("BullfrogHighscore", 0), PlayerPrefs.GetInt("PoisonDartFrogHighscore", 0) };
        int greatest = numbers.Max();

        int[] hardModeNumbers = { PlayerPrefs.GetInt("DefaultHardModeHighscore", 0), PlayerPrefs.GetInt("TreeFrogHardModeHighscore", 0), PlayerPrefs.GetInt("FrogletHardModeHighscore", 0), PlayerPrefs.GetInt("BullfrogHardModeHighscore", 0), PlayerPrefs.GetInt("PoisonDartFrogHardModeHighscore", 0) };
        int hardModeGreatest = hardModeNumbers.Max();
        if (hardModeGreatest == 0)
            hardModeGreatest = -1;

        foreach (TextMeshPro defaultHighscore in defaultHighscores)
        {
            if(greatest == PlayerPrefs.GetInt("DefaultHighscore", 0))
            {
                defaultHighscore.color = Color.yellow;
            }
        }
        foreach (TextMeshPro treeFrogHighscore in treeFrogHighscores)
        {
            if (greatest == PlayerPrefs.GetInt("TreeFrogHighscore", 0))
            {
                treeFrogHighscore.color = Color.yellow;
            }
        }
        foreach (TextMeshPro frogletHighscore in frogletHighscores)
        {
            if (greatest == PlayerPrefs.GetInt("FrogletHighscore", 0))
            {
                frogletHighscore.color = Color.yellow;
            }
        }
        foreach (TextMeshPro bullfrogHighscore in bullfrogHighscores)
        {
            if (greatest == PlayerPrefs.GetInt("BullfrogHighscore", 0))
            {
                bullfrogHighscore.color = Color.yellow;
            }
        }
        foreach (TextMeshPro poisonDartFrogHighscore in poisonDartFrogHighscores)
        {
            if (greatest == PlayerPrefs.GetInt("PoisonDartFrogHighscore", 0))
            {
                poisonDartFrogHighscore.color = Color.yellow;
            }
        }

        //HARD MODE

        foreach (TextMeshPro defaultHighscore in defaultHardModeHighscores)
        {
            if (hardModeGreatest == PlayerPrefs.GetInt("DefaultHardModeHighscore", 0))
            {
                defaultHighscore.color = hardModeHighscoreColor;
            }
        }
        foreach (TextMeshPro treeFrogHighscore in treeFrogHardModeHighscores)
        {
            if (hardModeGreatest == PlayerPrefs.GetInt("TreeFrogHardModeHighscore", 0))
            {
                treeFrogHighscore.color = hardModeHighscoreColor;
            }
        }
        foreach (TextMeshPro frogletHighscore in frogletHardModeHighscores)
        {
            if (hardModeGreatest == PlayerPrefs.GetInt("FrogletHardModeHighscore", 0))
            {
                frogletHighscore.color = hardModeHighscoreColor;
            }
        }
        foreach (TextMeshPro bullfrogHighscore in bullfrogHardModeHighscores)
        {
            if (hardModeGreatest == PlayerPrefs.GetInt("BullfrogHardModeHighscore", 0))
            {
                bullfrogHighscore.color = hardModeHighscoreColor;
            }
        }
        foreach (TextMeshPro poisonDartFrogHighscore in poisonDartFrogHardModeHighscores)
        {
            if (hardModeGreatest == PlayerPrefs.GetInt("PoisonDartFrogHardModeHighscore", 0))
            {
                poisonDartFrogHighscore.color = hardModeHighscoreColor;
            }
        }
    }
}