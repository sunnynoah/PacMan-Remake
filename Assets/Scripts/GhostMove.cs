using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GhostMove : MonoBehaviour
{
    public string AIType;

    [Header("General Targets")]
    public GameObject pacman;
    public GameObject deathtarget;

    [Header("Scatter Targets")]

    public GameObject blinkyScatterPoint;
    public GameObject pinkyScatterPoint;
    public GameObject inkyScatterPoint;
    public GameObject clydeScatterPoint;

    [Header("Inky Requirement")]

    public GameObject blinky;

    Vector2 direction = Vector2.right;

    Vector2 targetpos;

    Vector2[] directions = {Vector2.up, Vector2.left, Vector2.down, Vector2.right};

    Vector2 prevdir = Vector2.zero;

    float turnDistance = 0.5f;

    StateManager SM;

    bool inGhostHouse = false;

    Animator anim;

    private Rigidbody2D rb;

    bool dead = false;

    bool beendead = false;

    public bool activated = false;
    public GameObject deathDisplay;

    GameManager gm;

    Vector2 origin;

    Vector2 lastturn;

    public AudioClip eatghost;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SM = GameObject.FindGameObjectWithTag("StateManager").GetComponent<StateManager>();
        anim = GetComponent<Animator>();

        origin = transform.position;
    }

    private void FixedUpdate()
    {
        if (dead)
        {
            GetComponent<CircleCollider2D>().enabled = false;
        }
        else
        {
            GetComponent<CircleCollider2D>().enabled = true;
        }

        if (SM.state != "frightened")
        {
            beendead = false;
        }

        if (!activated)
        {
            targetpos = GameObject.FindGameObjectWithTag("Ghost House").transform.position;
        }
        else if (dead)
        {
            targetpos = deathtarget.transform.position;
            if (Vector2.Distance(targetpos, transform.position) < 1)
            {
                dead = false;
            }
        }
        else if (inGhostHouse)
        {
            targetpos = GameObject.FindGameObjectWithTag("House Exit").transform.position;
        }
        else
        {
            if (AIType == "blinky")
            {
                if (SM.state == "scatter")
                {
                    targetpos = blinkyScatterPoint.transform.position;
                }
                else
                {
                    targetpos = pacman.transform.position;
                }
            }
            else if (AIType == "pinky")
            {
                if (SM.state == "scatter")
                {
                    targetpos = pinkyScatterPoint.transform.position;
                }
                else
                {
                    targetpos = (Vector2)pacman.transform.position + (pacman.GetComponent<PacmanMove>().direction * 4);

                }
            }
            else if (AIType == "inky")
            {
                if (SM.state == "scatter")
                {
                    targetpos = inkyScatterPoint.transform.position;
                }
                else
                {
                    Vector2 forwardpos = (Vector2)pacman.transform.position + (pacman.GetComponent<PacmanMove>().direction * 2);

                    Vector2 between = forwardpos - (Vector2)blinky.transform.position;

                    targetpos = (Vector2)blinky.transform.position + (between * 2);
                }
            }
            else if (AIType == "clyde")
            {
                if (SM.state == "scatter")
                {
                    targetpos = clydeScatterPoint.transform.position;
                }
                else
                {
                    if (Vector2.Distance(pacman.transform.position, transform.position) > 8)
                    {
                        targetpos = pacman.transform.position;
                    }
                    else
                    {
                        targetpos = clydeScatterPoint.transform.position;
                    }
                }
            }
        }

        Vector2 centerTile = new Vector2(Mathf.FloorToInt(transform.position.x) + 0.5f, Mathf.FloorToInt(transform.position.y) + 0.5f);

        if (prevdir != direction)
        {
            lastturn = transform.position;

            transform.position = centerTile;
        }
        float speed = SM.speed;

        if (dead && !beendead)
        {
            speed *= 1.3f;
        }
        else if (SM.state == "frightened" && !beendead  )
        {
            speed /= 2f;
        }

        speed += (PlayerPrefs.GetInt("level", 1) / 10) - 0.1f;
        
        rb.velocity = direction * speed;

        prevdir = direction;

        if (Vector2.Distance(lastturn, (Vector2)transform.position) >= turnDistance)
        {
            if (Vector2.Distance(transform.position, centerTile) <= 0.1)
            {
                List<Vector2> freedirectionsList = new List<Vector2>();

                foreach (Vector2 dir in directions)
                {
                    bool rayhit = Physics2D.Raycast(transform.position, dir, 1, LayerMask.GetMask("Wall"));

                    if (!rayhit)
                    {
                        freedirectionsList.Add(dir);
                    }
                }

                Vector2[] freedirections = freedirectionsList.ToArray();
                if (freedirections.Length == 2)
                {
                    if (Vector2.Dot(freedirections[0].normalized, freedirections[1].normalized) != -1)
                    {
                        NewDirection(freedirections);
                    }
                }
                else if (freedirections.Length > 1)
                {
                    NewDirection(freedirections);
                }
            }
        }

        if (dead)
        {
            anim.SetInteger("state", 5);
        }
        else if (SM.state == "frightened" && !beendead)
        {
            anim.SetInteger("state", 4);
        }
        else if (direction == Vector2.right)
        {
            anim.SetInteger("state", 0);
        }
        else if (direction == Vector2.down)
        {
            anim.SetInteger("state", 1);
        }
        else if (direction == Vector2.left)
        {
            anim.SetInteger("state", 2);
        }
        else if(direction == Vector2.up)
        {
            anim.SetInteger("state", 3);
        }
    }

    private void NewDirection(Vector2[] directions)
    {
        List<float> distancesList = new List<float>();

        foreach (Vector2 dir in directions)
        {
            if (Vector2.Dot(direction, dir) == -1)
            {
                distancesList.Add(Mathf.Infinity - 1);
            }
            else
            {
                Vector2 currentpos = (Vector2)transform.position + dir;
                float distance = Vector2.Distance(targetpos, currentpos);
                distancesList.Add(distance);
            }
        }

        float lowestdist = Mathf.Infinity;
        int lowestindex = -1;

        for (int i = 0; i < distancesList.Count; i++)
        {
            if (distancesList[i] < lowestdist)
            {
                lowestdist = distancesList[i];
                lowestindex = i;
            }
        }
        if (SM.state == "frightened" && !dead && !beendead)
        {
            direction = directions[Random.Range(0, directions.Length-1)];
        }
        else
        {
            direction = directions[lowestindex];
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ghost House"))
        {
            inGhostHouse = true;
            turnDistance = 1;
        }
        else if (collision.gameObject.CompareTag("Player") && !dead && SM.state == "frightened")
        {
            gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            int amount = 200 * gm.ghostMult;
            PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score", 0) + amount);
            gm.ghostMult *= 2;
            dead = true;
            beendead = true;

            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().PlaySound(eatghost);

            GameObject disp = Instantiate(deathDisplay, transform.position, Quaternion.identity);
            disp.GetComponent<TextMeshPro>().text = $"{amount}";
            Destroy(disp, 3);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ghost House"))
        {
            turnDistance = 0.5f;
            inGhostHouse = false;
        }
    }

    public void ResetPosition()
    {
        transform.position = origin;
        prevdir = Vector2.zero;
        direction = Vector2.right;
        dead = false;
        beendead = false;
    }
}
