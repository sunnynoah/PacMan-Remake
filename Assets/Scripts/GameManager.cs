 using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.UI.Image;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI scoredisp;
    public TextMeshProUGUI leveldisp;
    public TextMeshProUGUI highscoredisp;
    int highestamt = 0;
    public int ghostMult = 1;

    [Header("Ghosts")]
    public GameObject blinky;
    public GameObject pinky;
    public GameObject inky;
    public GameObject clyde;

    GhostMove binkyscript;
    GhostMove pinkyscript;
    GhostMove inkyscript;
    GhostMove clydescript;

    [Header("Life Stuff")]

    public GameObject[] livesDisp;

    List<GameObject> ghosts = new List<GameObject>();

    StateManager stateManager;

    GameObject player;

    [Header("Sounds")]
    public AudioClip death;

    void Start()
    {
        binkyscript = blinky.GetComponent<GhostMove>();
        pinkyscript = pinky.GetComponent<GhostMove>();
        inkyscript = inky.GetComponent<GhostMove>();
        clydescript = clyde.GetComponent<GhostMove>();

        binkyscript.activated = true;
        pinkyscript.activated = true;

        ghosts.Add(blinky);
        ghosts.Add(pinky);
        ghosts.Add(inky);
        ghosts.Add(clyde);

        stateManager = GameObject.FindGameObjectWithTag("StateManager").GetComponent<StateManager>();

        player = GameObject.FindGameObjectWithTag("Player");

        Cursor.visible = false;
    }
    void Update()
    {

        //UI

        //Highscore

        if (PlayerPrefs.GetInt("score", 0) > PlayerPrefs.GetInt("highscore", 0))
        {
            PlayerPrefs.SetInt("highscore", PlayerPrefs.GetInt("score", 0));
        }

        string highscoretext = $"{PlayerPrefs.GetInt("highscore", 0)}";
        highscoredisp.text = highscoretext.PadLeft(6, '0');



        //Level
        int level = PlayerPrefs.GetInt("level", 1);
        leveldisp.text = $"Level {level}";

        // Score
        string scoretext = $"{PlayerPrefs.GetInt("score", 0)}";
        scoredisp.text = scoretext.PadLeft(6, '0');

        //Lives

        int index = 0;
        foreach (GameObject life in livesDisp)
        {
            index++;
            if (PlayerPrefs.GetInt("lives", 3) < index)
            {
                life.SetActive(false);
            }
            else
            {
                life.SetActive(true);
            }
        }

        //Pellet Checking and ghost activation
        GameObject[] pellets = GameObject.FindGameObjectsWithTag("Pellet");

        if (pellets.Length > highestamt )
        {
            highestamt = pellets.Length;
        }

        if (pellets.Length <= highestamt- 30 && !inkyscript.activated)
        {
            inkyscript.activated = true;
        }

        if (pellets.Length <= (highestamt / 3) * 2)
        {
            clydescript.activated = true;
        }


        if (pellets.Length == 0)
        {
            PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level", 1) + 1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void PacmanDeath(Vector2 origin)
    {

        PlaySound(death);
        PlayerPrefs.SetInt("lives", PlayerPrefs.GetInt("lives", 3) -1);

        foreach (GameObject ghost in ghosts)
        {
            ghost.SetActive(false);
        }

        player.GetComponent<Animator>().SetBool("dead", true);
        player.GetComponent<PacmanMove>().enabled = false;
        player.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (PlayerPrefs.GetInt("lives", 3) <= 0)
        {
            foreach (GameObject ghost in ghosts)
            {
                ghost.SetActive(false);
            }

            StartCoroutine(CompleteDeath());
        }
        else
        {
            foreach (GameObject ghost in ghosts)
            {
                ghost.SetActive(false);
            }

            StartCoroutine(LifeLost(origin));
        }
    }

    IEnumerator LifeLost(Vector2 origin)
    {
        yield return new WaitForSeconds(1.3f);

        player.GetComponent<PacmanMove>().enabled = true;
        player.GetComponent<Animator>().SetBool("dead", false);
        player.transform.position = origin;
        stateManager.time = 0;
        stateManager.state = "scatter";
        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<GhostMove>().ResetPosition();
            ghost.SetActive(true);
        }
    }

    IEnumerator CompleteDeath()
    {
        yield return new WaitForSeconds(1.3f);

        PlayerPrefs.SetInt("level", 1);
        PlayerPrefs.SetInt("lives", 3);
        PlayerPrefs.SetInt("score", 0);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PlaySound(AudioClip clip)
    {
        AudioSource audioplayer = GetComponent<AudioSource>();

        audioplayer.clip = clip;
        audioplayer.Play();
    }
}
