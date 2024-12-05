using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanPickup : MonoBehaviour
{
    StateManager statemanager;

    GameManager gm;

    public AudioClip eat;

    AudioSource audioplayer;

    float eattime = 0;
    

    private void Start()
    {
        audioplayer = GetComponent<AudioSource>();
        statemanager = GameObject.FindGameObjectWithTag("StateManager").GetComponent<StateManager>();
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        if (eattime > 0)
        {
            eattime -= Time.deltaTime;
        }
        else
        {
            eattime = 0;
            audioplayer.Pause();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pellet"))
        {
            Destroy(collision.gameObject);
            PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score", 0) + 10);
            Eat();
        }
        else if (collision.CompareTag("Power Pellet"))
        {
            Destroy(collision.gameObject);
            statemanager.ActivateFrightened();
            PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score", 0) + 50);
            Eat();
        }
    }

    private void Eat()
    {
        audioplayer.UnPause();
        eattime = 0.15f;
    }
}
