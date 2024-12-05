using UnityEngine;
using UnityEngine.SceneManagement;

public class PacmanMove : MonoBehaviour
{
    public Vector2 direction = Vector2.zero;
    Rigidbody2D rb;

    public float speed = 2f;

    private int rotation = 0;

    private Vector2 prevDir = Vector2.right;

    StateManager stateManager;

    GameManager gameManager;

    InputControls controls;

    Vector2 origin;

    private void OnEnable()
    {
        controls = new InputControls();
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        stateManager = GameObject.FindGameObjectWithTag("StateManager").GetComponent<StateManager>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        origin = transform.position;
    }

    void Update()
    {
        int prevRot = rotation;
        if (controls.Player.Up.IsPressed())
        {
            direction = Vector2.up;
            rotation = 90;
        }
        else if (controls.Player.Down.IsPressed())
        {
            direction = Vector2.down;
            rotation = 270;
        }
        else if (controls.Player.Right.IsPressed())
        {
            direction = Vector2.right;
            rotation = 0;
        }
        else if (controls.Player.Left.IsPressed())
        {
            direction = Vector2.left;
            rotation = 180;
        }

        if (prevDir != direction)
        {
            Vector2 centerTile = new Vector2(Mathf.FloorToInt(transform.position.x) + 0.5f, Mathf.FloorToInt(transform.position.y) + 0.5f);
            if (Physics2D.Raycast(transform.position, direction, 0.75f, LayerMask.GetMask("Wall")) || Vector2.Distance(centerTile, transform.position) > 0.2)
            {
                rotation = prevRot;
                direction = prevDir;
            }
            else
            {
                transform.position = centerTile;
            }
        }
        transform.rotation = Quaternion.Euler(0, 0, rotation);

        prevDir = direction;

        //Menu Stuff

        if (controls.Player.Menu.triggered)
        {
            SceneManager.LoadScene("Menu");
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ghost"))
        {
            if (stateManager.state != "frightened")
            {
                rb.velocity = Vector2.zero;
                direction = Vector2.right;
                prevDir = Vector2.right;
                rotation = 0;
                gameManager.PacmanDeath(origin);
            }
        }
    }
}
