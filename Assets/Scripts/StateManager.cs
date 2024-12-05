using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public string state = "scatter";
    public float speed = 7;

    private int cycle = 1;

    public float time = 0;

    GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    private void Update()
    {
        time += Time.deltaTime;
        if (state == "scatter")
        {
            if (time > 7)
            {
                time = 0;
                state = "chase";
            }
        }
        else if (state == "chase")
        {
            if (time > 20)
            {
                time = 0;
                state = "scatter";
                cycle++;
            }
        }
        else if (state == "frightened")
        {
            if (time > 8)
            {
                time = 0;
                state = "chase";
            }
        }
    }

    public void ActivateFrightened()
    {
        gameManager.ghostMult = 1;
        state = "frightened";
        time = 0;
    }
}
