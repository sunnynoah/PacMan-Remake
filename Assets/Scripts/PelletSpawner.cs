using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PelletSpawner : MonoBehaviour
{
    public GameObject pellet;
    public GameObject powerpellet;

    private int[] powerPositions = { 0,2, 25,2, 0,22, 25,22 };

    float distance = 1;

    private void Start()
    {
        for (int x = 0; x < 26; x++)
        {
            float xshift = x * distance;
            for (int y = 0; y < 30; y++)
            {
                float yshift = -y * distance;
                Vector2 position = new Vector2(transform.position.x + xshift, transform.position.y + yshift);

                Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.15f);

                GameObject type = pellet;

                for (int i = 0; i < powerPositions.Length; i += 2)
                {
                    if (xshift == powerPositions[i] && yshift == -powerPositions[i+1] && PlayerPrefs.GetInt("level", 1) < 5)
                    {
                        type = powerpellet;
                        break;
                    }
                }

                if (colliders.Length == 0)
                {
                    Instantiate(type, position, Quaternion.identity);
                }
            }
        }
        Destroy(this.gameObject);

        foreach (GameObject blocker in GameObject.FindGameObjectsWithTag("Pellet Blocker"))
        {
            Destroy(blocker);
        }
    }

}
