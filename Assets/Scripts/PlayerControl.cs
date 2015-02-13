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
    private bool landed = false;

    private bool down = false;              //similar to jump - we can't really get key downs in fixed update

    private int lastPressed = 0;

    private Vector2 playerTopLeft;
    private Vector2 playerTopRight;

    private Transform headCheckLeft;
    private Transform headCheckRight;
    private Transform groundCheck;

    private string horizontalAxis = "Horizontal";
    private string verticalAxis = "Vertical";
    private string jumpKey = "Jump";
    private string rightKey = "Right";
    private string leftKey = "Left";
    private string downKey = "Down";

    private Color color;

    private GameObject sprite;

    private Animator anim;

    private ParticleSystem particleSystem;
    
	// Use this for initialization
    void Awake()
    {
        headCheckLeft = transform.FindChild("HeadCheckL");
        headCheckRight = transform.FindChild("HeadCheckR");
        groundCheck = transform.FindChild("GroundCheck");
        sprite = transform.FindChild("Sprite").gameObject;

        if (groundCheck == null || headCheckLeft == null || headCheckRight == null || sprite == null)
        {
            UnityEngine.Debug.LogError("Can't find ground or either head check or sprite child!");
            this.enabled = false;
        }

        if (jumpSound == null || dJumpSound == null || deathSound == null)
        {
            UnityEngine.Debug.LogError("Sounds have not been given!");
            this.enabled = false;
        }

        anim = GetComponent<Animator>();
        particleSystem = GetComponentInChildren<ParticleSystem>();
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
            if (inAir)
                landed = true;

            inAir = false;
            hasDoubleJumped = false;
        }
        else
            landed = false;

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
        if (hit & !dead)
            die();
        else if (!hit)
        {
            inputHandler();
            jumpHandler();
        }
    }

    void die()
    {
        audio.PlayOneShot(deathSound);
        
        dead = true;
        sprite.renderer.material.color = Color.white;
        GetComponent<BoxCollider2D>().enabled = false;
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


        // If the player is changing direction or letting go of controls, stop all horizontal movement, but only on ground
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
        float v = Input.GetAxisRaw(verticalAxis);

        //if hit ground with force, play sound and particles, and screen shake
        if (rigidbody2D.velocity.y < 0.2f && landed)
        {
            landed = false;
            //landing sound
            //particleSystem.renderer.material.color = color;
            particleSystem.Play();
            //screen shake
        }
        
        //if jump is let go and we are jumping up, then stop veritcal movement
        if (rigidbody2D.velocity.y > 0 && v == 0)
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0f);

        //air friction
        if (inAir)
            rigidbody2D.AddForce(new Vector2(rigidbody2D.velocity.x / 10, rigidbody2D.velocity.y));

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
        //sprite.renderer.material.color = color;
        GetComponent<BoxCollider2D>().enabled = true;
        lastPressed = 0;
        anim.SetTrigger("Reset");
    }

    public int getPlayerNo()
    {
        return playerNo;
    }

    public void setPlayer(int playerNumber, int opponentNumber, Color pColor)
    {
        color = pColor;

        playerNo = playerNumber;

        setLayerMask("Player " + playerNo, "Player " + opponentNumber);

        //set up the inputs for this player
        horizontalAxis = "P" + playerNumber + horizontalAxis;
        verticalAxis = "P" + playerNumber + verticalAxis;
        jumpKey = "P" + playerNumber + jumpKey;
        rightKey = "P" + playerNumber + rightKey;
        leftKey = "P" + playerNumber + leftKey;
        downKey = "P" + playerNumber + downKey;
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

