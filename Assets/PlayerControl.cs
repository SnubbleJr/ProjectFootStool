using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
    
    private bool debug;

    public float jumpForce;
    public float fastFallForce;
    public float moveForce;
    public float maxSpeed;

    public AudioClip jumpSound;
    public AudioClip dJumpSound;
    public AudioClip deathSound;
    
    private int playerNo;
    private int score;
    private int opponentLayerMask;

    private bool hit = false;
    private bool dead = false;
    private bool grounded = false;
    private bool jump = false;              //jump and double jump are technically flags so we can apply force in fixed update
    private bool doubleJump = false;
    private bool hasDoubleJumped = true;   //a state for if we have already double jumped still needs to be held.
    private bool inAir = true;

    private bool down = false;              //similar to jump - we can't really get key downs in fixed update

    private int lastPressed = 0;

    private Vector2 playerTopLeft;
    private Vector2 playerTopRight;

    private Transform headCheckLeft;
    private Transform headCheckRight;
    private Transform groundCheck;

    private string horizontalAxis = "Horizontal";
    private string jumpKey = "Jump";
    private string rightKey = "Right";
    private string leftKey = "Left";
    private string downKey = "Down";

    private Animator anim;

	// Use this for initialization
    void Start()
    {
        headCheckLeft = transform.FindChild("HeadCheckL");
        headCheckRight = transform.FindChild("HeadCheckR");
        groundCheck = transform.FindChild("GroundCheck");

        if (groundCheck == null || headCheckLeft == null || headCheckRight == null)
        {
            UnityEngine.Debug.LogError("Can't find ground or either head check!");
            this.enabled = false;
        }

        if (jumpSound == null || dJumpSound == null || deathSound == null)
        {
            UnityEngine.Debug.LogError("Sounds have not been given!");
            this.enabled = false;
        }

        anim = GetComponent<Animator>();
	}

	void Update ()
    {
        playerTopLeft = new Vector2(transform.position.x - rigidbody2D.collider2D.bounds.extents.x, transform.position.y + rigidbody2D.collider2D.bounds.extents.y);
        playerTopRight = new Vector2(transform.position.x + rigidbody2D.collider2D.bounds.extents.x, transform.position.y + rigidbody2D.collider2D.bounds.extents.y);

        if (debug)
        {
            debugDrawHitBoxes();
        }

        hit = Physics2D.Linecast(playerTopLeft, headCheckLeft.position, opponentLayerMask) || Physics2D.Linecast(playerTopRight, headCheckRight.position, opponentLayerMask);
        grounded = Physics2D.Linecast(transform.position, groundCheck.position, (1 << LayerMask.NameToLayer("Ground")));

        if (grounded)
        {
            inAir = false;
            hasDoubleJumped = false;
        }

        //direction input fixing
        if (Input.GetButtonDown(leftKey))
            lastPressed = -1;        
        if (Input.GetButtonDown(rightKey))
            lastPressed = 1;
        if (Input.GetButtonUp(rightKey) || Input.GetButtonUp(leftKey))
            lastPressed = 0;

        down = Input.GetButton(downKey);

        //if jump pressed
        if(Input.GetButtonDown(jumpKey))
        {
            //if grounded, normal jump, else double jump if can
            if (grounded)
                jump = true;
            else if (!hasDoubleJumped)
                doubleJump = true;
        }
	}

    void FixedUpdate()
    {
        if (hit &! dead)
            die();
        else
        {
            inputHandler();
            jumpHandler();
        }
    }

    void die()
    {
        dead = true;

        audio.PlayOneShot(deathSound);

        //death animation
        rigidbody2D.gravityScale = 0;
        anim.SetTrigger("Dead");

        GameObject.Find("Player Manager").GetComponent<PlayerManagerBehaviour>().playerLost(playerNo);
        this.enabled = false;
    }

    void inputHandler()
    {
        // Cache the input.
        float h = Input.GetAxisRaw(horizontalAxis);

        // the last key to be pressed takes priority
        if (h == 0)
            h = lastPressed;

        // If the player is holding down, apply fall force
        if (down)
            rigidbody2D.AddForce(Vector2.up *-1 * fastFallForce);


        // If the player is changing direction, stop all horizontal movement
        if (h != Mathf.Sign(rigidbody2D.velocity.x) & !inAir)
            rigidbody2D.velocity = new Vector2(0f, rigidbody2D.velocity.y);

        // If the player is changing direction or hasn't reached maxSpeed yet, add a force
        if (h * rigidbody2D.velocity.x < maxSpeed)
        {
            rigidbody2D.AddForce(Vector2.right * h * moveForce);
            //flip sprite
            //Vector3 scale = transform.localScale;
            //scale.x *= h;
            //transform.localScale = scale;
        }

        // If the player's horizontal velocity is greater than the maxSpeed, set it to max speed
        if (rigidbody2D.velocity.x > maxSpeed)
            rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * maxSpeed, rigidbody2D.velocity.y);
    }

    void jumpHandler()
    {
        if (jump)
        {
            audio.PlayOneShot(jumpSound);

            rigidbody2D.AddForce(new Vector2(0f, jumpForce));
            jump = false;
            inAir = true;
        }
        if (doubleJump)
        {
            audio.PlayOneShot(dJumpSound);

            //nulify current force from gravity
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0f);

            rigidbody2D.AddForce(new Vector2(0f, jumpForce));
            doubleJump = false;
            hasDoubleJumped = true;
        }
    }

    private void debugDrawHitBoxes()
    {
        Vector2 playerBottomLeft = new Vector2(transform.position.x - rigidbody2D.collider2D.bounds.extents.x, transform.position.y - rigidbody2D.collider2D.bounds.extents.y);
        Vector2 playerBottomRight = new Vector2(transform.position.x + rigidbody2D.collider2D.bounds.extents.x, transform.position.y - rigidbody2D.collider2D.bounds.extents.y);

        Debug.DrawLine(playerTopLeft, playerTopRight, Color.green);
        Debug.DrawLine(playerTopRight, playerBottomRight, Color.green);
        Debug.DrawLine(playerBottomRight, playerBottomLeft, Color.green);
        Debug.DrawLine(playerBottomLeft, playerTopLeft, Color.green);

        Debug.DrawLine(playerTopLeft, headCheckLeft.position, Color.red);
        Debug.DrawLine(headCheckLeft.position, headCheckRight.position, Color.red);
        Debug.DrawLine(playerTopRight, headCheckRight.position, Color.red);

        Debug.DrawLine(transform.position, groundCheck.position);
    }

    public void respawn()
    {
        dead = false;
        rigidbody2D.gravityScale = 1;
        lastPressed = 0;
        anim.SetTrigger("Reset");
    }

    public int getPlayerNo()
    {
        return playerNo;
    }

    public void setPlayerNo(int player, int opponent)
    {
        playerNo = player;

        setLayerMask("Player " + playerNo, "Player " + opponent);

        //set up the inputs for this player
        horizontalAxis = "P" + player + horizontalAxis;
        jumpKey = "P" + player + jumpKey;
        rightKey = "P" + player + rightKey;
        leftKey = "P" + player + leftKey;
        downKey = "P" + player + downKey;
    }

    private void setLayerMask(string mask, string opMask)
    {
        this.gameObject.layer = LayerMask.NameToLayer(mask);
        opponentLayerMask = (1 << LayerMask.NameToLayer(opMask));
        //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(mask), LayerMask.NameToLayer(opMask), true);
    }

    public int getScore()
    {
        return score;
    }

    public void increaseScore()
    {
        score++;
    }

    public void setDebug(bool value)
    {
        debug = value;
    }
}

