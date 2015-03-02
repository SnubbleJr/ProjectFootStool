using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
    
    private bool debug;

    public float jumpForce;
    public float jumpHeight = 1.5f;
    public float fastFallForce;
    public float moveForce;
    public float maxSpeed;
    public float wallFrictionForce;

    public AudioClip jumpSound;
    public AudioClip dJumpSound;
    public AudioClip landingSound;
    public AudioClip deathSound;

    private int playerNo;
    private int score;
    private int opponentLayerMask;

    private bool hit = false;
    private bool dead = false;
    private bool grounded = false;
    private bool onWall = false;
    private bool onLeftWall;
    private bool onRightWall;
    private bool jump = false;           //jump and double jump are technically flags so we can apply force in fixed update
    private bool doubleJump = false;
    private bool hasJumped = true;   //a state for if we have already double jumped still needs to be held.
    private bool hasDoubleJumped = true;   //a state for if we have already double jumped still needs to be held.
    private bool inAir = true;
    private bool landed = false;

    private bool down = false;              //similar to jump - we can't really get key downs in fixed update

    private int lastPressed = 0;

    private Vector2 playerTopLeft;
    private Vector2 playerTopRight;
    private Vector2 playerBottomLeft;
    private Vector2 playerBottomRight;

    private Transform headCheck;

    private Transform headCheckLeft;
    private Transform headCheckRight;

    private Transform groundCheck;

    private Transform groundCheckLeft;
    private Transform groundCheckRight;

    private Transform sideCheckLeft;
    private Transform sideCheckRight;

    private Transform sideCheckTopLeft;
    private Transform sideCheckBottomLeft;
    private Transform sideCheckTopRight;
    private Transform sideCheckBottomRight;

    private string horizontalAxis = "Horizontal";
    private string jumpKey = "Jump";
    private string rightKey = "Right";
    private string leftKey = "Left";
    private string downKey = "Down";

    private Color color;

    private GameObject sprite;

    private Animator anim;

    private ParticleSystem groundParticleSystem;
    private ParticleSystem wallLeftParticleSystem;
    private ParticleSystem wallRightParticleSystem;

    private GameObject viewingCamera;

	// Use this for initialization
    void Awake()
    {
        headCheck = transform.FindChild("HeadCheck");

        headCheckLeft = headCheck.FindChild("HeadCheckL");
        headCheckRight = headCheck.FindChild("HeadCheckR");

        groundCheck = transform.FindChild("GroundCheck");

        groundCheckLeft = groundCheck.FindChild("GroundCheckL");
        groundCheckRight = groundCheck.FindChild("GroundCheckR");

        sideCheckLeft = transform.FindChild("SideCheckL");
        sideCheckRight = transform.FindChild("SideCheckR");

        sideCheckTopLeft = sideCheckLeft.transform.FindChild("SideCheckLU");
        sideCheckBottomLeft = sideCheckLeft.transform.FindChild("SideCheckLL");
        sideCheckTopRight = sideCheckRight.transform.FindChild("SideCheckRU");
        sideCheckBottomRight = sideCheckRight.transform.FindChild("SideCheckRL");

        sprite = transform.FindChild("Sprite").gameObject;

        viewingCamera = GameObject.Find("Viewing Camera");

        if (groundCheck == null || headCheck == null || sideCheckLeft == null || sideCheckRight == null || sprite == null)
        {
            UnityEngine.Debug.LogError("Can't find ground or head or side checks or sprite child!");
            this.enabled = false;
        }

        if (jumpSound == null || dJumpSound == null || deathSound == null)
        {
            UnityEngine.Debug.LogError("Sounds have not been given!");
            this.enabled = false;
        }

        if (viewingCamera == null)
        {
            UnityEngine.Debug.LogError("No viewing camera found!");
            this.enabled = false;
        }

        anim = GetComponent<Animator>();
        groundParticleSystem = groundCheck.GetComponentInChildren<ParticleSystem>();
        wallLeftParticleSystem = sideCheckLeft.GetComponentInChildren<ParticleSystem>();
        wallRightParticleSystem = sideCheckRight.GetComponentInChildren<ParticleSystem>();
	}

	void Update ()
    {
        //get player boundries - HAS to be in update
        playerTopLeft = new Vector2(transform.position.x - rigidbody2D.collider2D.bounds.extents.x, transform.position.y + rigidbody2D.collider2D.bounds.extents.y);
        playerTopRight = new Vector2(transform.position.x + rigidbody2D.collider2D.bounds.extents.x, transform.position.y + rigidbody2D.collider2D.bounds.extents.y);
        playerBottomLeft = new Vector2(transform.position.x - rigidbody2D.collider2D.bounds.extents.x, transform.position.y - rigidbody2D.collider2D.bounds.extents.y);
        playerBottomRight = new Vector2(transform.position.x + rigidbody2D.collider2D.bounds.extents.x, transform.position.y - rigidbody2D.collider2D.bounds.extents.y);

        if (debug)
        {
            debugDrawHitBoxes();
        }

        hit = Physics2D.Linecast(playerTopLeft, headCheckLeft.position, opponentLayerMask) || Physics2D.Linecast(playerTopRight, headCheckRight.position, opponentLayerMask);

        grounded = Physics2D.Linecast(playerBottomLeft, groundCheckLeft.position, (1 << LayerMask.NameToLayer("Ground"))) || Physics2D.Linecast(playerBottomRight, groundCheckRight.position, (1 << LayerMask.NameToLayer("Ground")));

        onLeftWall = Physics2D.Linecast(playerTopLeft, sideCheckTopLeft.position, (1 << LayerMask.NameToLayer("Ground"))) || Physics2D.Linecast(playerBottomLeft, sideCheckBottomLeft.position, (1 << LayerMask.NameToLayer("Ground")));
        onRightWall = Physics2D.Linecast(playerTopRight, sideCheckTopRight.position, (1 << LayerMask.NameToLayer("Ground"))) || Physics2D.Linecast(playerBottomRight, sideCheckBottomRight.position, (1 << LayerMask.NameToLayer("Ground")));

        onWall = onLeftWall || onRightWall;

        //if on wall, set gorunded, so we can jump again
        if (onWall)
        {
            //if we're on the ground, then don't set landed
            if (!grounded)
                landed = true;

            grounded = true;
            inAir = false;
        }

        if (grounded)
        {
            if (inAir)
                landed = true;

            inAir = false;
            hasJumped = false;
            hasDoubleJumped = false;
        }
        else
        {
            inAir = true;
            landed = false;
        }

        //direction input fixing
        if (Input.GetButtonDown(leftKey))
            lastPressed = -1;        
        if (Input.GetButtonDown(rightKey))
            lastPressed = 1;
        if (Input.GetButtonUp(rightKey) || Input.GetButtonUp(leftKey))
            lastPressed = 0;

        down = Input.GetButton(downKey) || Input.GetAxisRaw(downKey) == 1;

        //if jump pressed
        if(Input.GetButtonDown(jumpKey))
        {
            //if grounded, normal jump, else double jump if can
            if (!hasJumped)
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
        sprite.renderer.material.color = Color.black;
        GetComponent<BoxCollider2D>().enabled = false;
        anim.SetTrigger("Dead");

        GameObject.Find("Player Manager").GetComponent<PlayerManagerBehaviour>().playerLost(playerNo);
        this.enabled = false;

        //screen ripple
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        viewingCamera.SendMessage("splashAtPoint", new Vector2(pos.x, pos.y));
    }

    void inputHandler()
    {
        // Cache the input.
        float h = Input.GetAxisRaw(horizontalAxis);

        // Snapping for analogsticks
        if (h > 0)
            h = 1;
        if (h < 0)
            h = -1;

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


        bool letGoOfJump = Input.GetButtonUp(jumpKey);
            
        //if attached to a wall, start sliding them down
        if (onWall)
            rigidbody2D.AddForce(new Vector2(0, -wallFrictionForce));
        
        //if hit wall with force, play sound and particles, and screen shake
        if (landed && onWall)
        {
            landed = false;

            //fix wall jumping not reseting double jumps
            //hasDoubleJumped = false;
            //landing sound

            //screen shake
            if (onRightWall)
            {
                wallRightParticleSystem.Play();
                //screen ripple
                Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
                viewingCamera.SendMessage("splashAtPoint", new Vector2(pos.x, pos.y));
            }
            if (onLeftWall)
            {
                wallLeftParticleSystem.Play();
                //screen ripple
                Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
                viewingCamera.SendMessage("splashAtPoint", new Vector2(pos.x, pos.y));
            }
        }

        //if hit ground with force, play sound and particles, and screen shake
        if (rigidbody2D.velocity.y < 0.2f && landed)
        {
            landed = false;
            //landing sound
            audio.PlayOneShot(landingSound);

            groundParticleSystem.Play();
            //screen shake

            //screen ripple
            Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
            viewingCamera.SendMessage("splashAtPoint", new Vector2(pos.x, pos.y));
        }
        
        //if jump is let go and we are jumping up, then stop veritcal movement
        if (rigidbody2D.velocity.y > 0 && letGoOfJump)
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0f);

        //air friction
        if (inAir)
            rigidbody2D.AddForce(new Vector2((-rigidbody2D.velocity.x), 0));

        if (jump)
        {
            audio.PlayOneShot(jumpSound);

            //nulify current force from gravity
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);

            rigidbody2D.AddForce(new Vector2(0f, jumpForce));

            jump = false;
            inAir = true;
            hasJumped = true;
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

        Debug.DrawLine(playerTopLeft, sideCheckTopLeft.position, Color.blue);
        Debug.DrawLine(sideCheckTopLeft.position, sideCheckBottomLeft.position, Color.blue);
        Debug.DrawLine(sideCheckBottomLeft.position, playerBottomLeft, Color.blue);

        Debug.DrawLine(playerTopRight, sideCheckTopRight.position, Color.blue);
        Debug.DrawLine(sideCheckTopRight.position, sideCheckBottomRight.position, Color.blue);
        Debug.DrawLine(sideCheckBottomRight.position, playerBottomRight, Color.blue);

        Debug.DrawLine(playerBottomLeft, groundCheckLeft.position, Color.yellow);
        Debug.DrawLine(groundCheckLeft.position, groundCheckRight.position, Color.yellow);
        Debug.DrawLine(groundCheckRight.position, playerBottomRight, Color.yellow);
    }

    public void respawn()
    {
        dead = false;
        sprite.renderer.material.color = color;
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
        sprite.renderer.material.color = color;
        groundParticleSystem.startColor = color;
        wallLeftParticleSystem.startColor = color;
        wallRightParticleSystem.startColor = color;

        playerNo = playerNumber;

        setLayerMask("Player " + playerNo, "Player " + opponentNumber);

        //set up the inputs for this player
        horizontalAxis = "P" + playerNumber + horizontalAxis;
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

