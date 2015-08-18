using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {
    
    private bool debug;

    public float jumpForce;
    public float fastFallForce;
    public float moveForce;
    public float maxSpeed;

    public SFX jumpSound = SFX.Jump;
    public SFX dJumpSound = SFX.DoubleJump;
    public SFX fallingSound = SFX.Fall;
    public SFX wallSlidingSound = SFX.WallSlide;
    public SFX landingSound = SFX.Land;
    public SFX flumpSound = SFX.Flump;
    public SFX deathSound = SFX.Death;

    private int playerNo;
    private int teamNo;
    private int score;      //can be lives as well as time on hill
    private List<int> opponentLayerMasks;

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
    private bool firstTimeOnWall = false;
    private bool hasBeenOnWall = false;

    private float platRayHeight = 0.75f;
    private bool aboveJumpablePlate = false;

    private Collider2D fallThroughPlat;

    private bool letGoOfJump;
    private float h;

    private bool down = false;              //similar to jump - we can't really get key downs in fixed update

    private bool keyboard = false;

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

    private string horizontalAxis;
    private string jumpKey;
    private string rightKey;
    private string leftKey;
    private string downKey;

    private Player player;

    private GameObject spriteGO;
    private SpriteRenderer spriteRenderer;

    private Animator anim;

    private ParticleSystem groundParticleSystem;
    private ParticleSystem wallLeftParticleSystem;
    private ParticleSystem wallRightParticleSystem;

    private GameObject viewingCamera;

    private Rigidbody2D rigidbody2D;
    private Collider2D collider2D;

    private int groundLayerMask;

	// Use this for initialization
    void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        collider2D = rigidbody2D.GetComponent<Collider2D>();

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

        spriteGO = transform.FindChild("Sprite").gameObject;

        viewingCamera = GameObject.Find("Viewing Camera");

        if (groundCheck == null || headCheck == null || sideCheckLeft == null || sideCheckRight == null || spriteGO == null)
        {
            UnityEngine.Debug.LogError("Can't find ground or head or side checks or sprite child!");
            this.enabled = false;
        }
        
        if (viewingCamera == null)
        {
            UnityEngine.Debug.LogError("No viewing camera found!");
            this.enabled = false;
        }

        spriteRenderer = spriteGO.GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        groundParticleSystem = groundCheck.GetComponentInChildren<ParticleSystem>();
        wallLeftParticleSystem = sideCheckLeft.GetComponentInChildren<ParticleSystem>();
        wallRightParticleSystem = sideCheckRight.GetComponentInChildren<ParticleSystem>();

        groundLayerMask = 1 << LayerMask.NameToLayer("Ground");
	}

    void OnDestroy()
    {
        try
        {
            SFXManagerBehaviour.Instance.stopLoop(this.gameObject, wallSlidingSound);
            SFXManagerBehaviour.Instance.stopLoop(this.gameObject, fallingSound);
        }
        catch
        {
        }
    }

	void Update ()
    {
        //get player boundries - HAS to be in update
        playerTopLeft = new Vector2(transform.position.x - collider2D.bounds.extents.x, transform.position.y + collider2D.bounds.extents.y);
        playerTopRight = new Vector2(transform.position.x + collider2D.bounds.extents.x, transform.position.y + collider2D.bounds.extents.y);
        playerBottomLeft = new Vector2(transform.position.x - collider2D.bounds.extents.x, transform.position.y - collider2D.bounds.extents.y);
        playerBottomRight = new Vector2(transform.position.x + collider2D.bounds.extents.x, transform.position.y - collider2D.bounds.extents.y);

        if (debug)
            debugDrawHitBoxes();

        hit = false;

        foreach (int opponentLayerMask in opponentLayerMasks)
        {
            hit = Physics2D.Linecast(playerTopLeft, headCheckLeft.position, opponentLayerMask) || Physics2D.Linecast(playerTopRight, headCheckRight.position, opponentLayerMask) || hit;
            if (hit)
            {
                SFXManagerBehaviour.Instance.stopLoop(this.gameObject, wallSlidingSound);
                SFXManagerBehaviour.Instance.stopLoop(this.gameObject, fallingSound);

                if (!dead)
                    die(opponentLayerMask);
            }
        }

            
        grounded = Physics2D.Linecast(playerBottomLeft, groundCheckLeft.position, groundLayerMask) || Physics2D.Linecast(playerBottomRight, groundCheckRight.position, groundLayerMask);
        
        RaycastHit2D ray;
        aboveJumpablePlate = false;

        ray = Physics2D.Raycast(playerTopLeft, Vector2.up, platRayHeight, groundLayerMask);
        if (ray.collider != null)
        {
            if (ray.collider.gameObject.CompareTag("FallThroughPlatform"))
            {
                fallThroughPlat = ray.collider.gameObject.GetComponent<Collider2D>();
                aboveJumpablePlate = true;
            }
        }

        ray = Physics2D.Raycast(playerTopRight, Vector2.up, platRayHeight, groundLayerMask);
        if (ray.collider != null)
        {
            if (ray.collider.gameObject.CompareTag("FallThroughPlatform"))
            {
                fallThroughPlat = ray.collider.gameObject.GetComponent<Collider2D>();
                aboveJumpablePlate = true;
            }
        }

        onLeftWall = Physics2D.Linecast(playerTopLeft, sideCheckTopLeft.position, groundLayerMask) || Physics2D.Linecast(playerBottomLeft, sideCheckBottomLeft.position, groundLayerMask);
        onRightWall = Physics2D.Linecast(playerTopRight, sideCheckTopRight.position, groundLayerMask) || Physics2D.Linecast(playerBottomRight, sideCheckBottomRight.position, groundLayerMask);

        onWall = onLeftWall || onRightWall;
        
        letGoOfJump = !Input.GetButton(jumpKey);
        h = Input.GetAxisRaw(horizontalAxis);

        //if not holding into the wall, then disable being on the wall
        //else, they get penalised for just being next to the wall
        if (grounded)
            onWall = false;

        groundChecker();

        wallChecker();

        if (keyboard)
        {
            //direction input fixing
            if (Input.GetButtonDown(leftKey))
                lastPressed = -1;
            if (Input.GetButtonDown(rightKey))
                lastPressed = 1;
            if (Input.GetButtonUp(rightKey) || Input.GetButtonUp(leftKey))
                lastPressed = 0;
        }

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

    private void groundChecker()
    {
        //here we check if wer'e on ground to reset jumps
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
    }

    private void wallChecker()
    {
        //if on wall, set gorunded, so we can jump again
        if (onWall)
        {
            SFXManagerBehaviour.Instance.loopSound(this.gameObject, wallSlidingSound);

            //reset jumps
            hasJumped = false;
            hasDoubleJumped = false;

            //if we've hit the wall for the first time, set first time to true
            if (!hasBeenOnWall)
                firstTimeOnWall = true;

            hasBeenOnWall = true;
        }
        else
        {
            SFXManagerBehaviour.Instance.stopLoop(this.gameObject, wallSlidingSound);

            hasBeenOnWall = false;
        }
    }

    void FixedUpdate()
    {
        // If down pressed and on fallthough platofrom, or if enttering from above, remove their collision
        if (fallThroughPlat != null && ((down && !inAir) || (aboveJumpablePlate && inAir)))
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), fallThroughPlat, true);
        else
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), fallThroughPlat, false);

        if (!hit)
        {
            inputHandler();
            jumpHandler();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //unset the platform, also make it care about those colission again
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), fallThroughPlat, false);

        if (collision.gameObject.CompareTag("FallThroughPlatform"))
            fallThroughPlat = collision.collider;
    }

    private void die()
    {
        //we've SD'd, set out own layer mask as killer
        die((int)Mathf.Pow(2, (8 + playerNo)));
    }

    private void die(int killer)
    {
        SFXManagerBehaviour.Instance.playSound(deathSound);
        
        dead = true;
        spriteRenderer.color = Color.grey;
        GetComponent<BoxCollider2D>().enabled = false;
        anim.SetTrigger("Dead");

        GameObject.Find("Player Manager").GetComponent<PlayerManagerBehaviour>().playerHit(this, killer);
                
        //screen ripple
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        viewingCamera.SendMessage("splashAtPoint", new Vector2(pos.x, pos.y));
    }

    private void inputHandler()
    {
        // Snapping for analogsticks
        if (h > 0)
            h = 1;
        if (h < 0)
            h = -1;

        // the last key to be pressed takes priority
        if (h == 0)
            h = lastPressed;
                
        // If the player is holding down, apply fall force, unless they're on the floor
        if (down && !grounded)
        {
            rigidbody2D.AddForce(Vector2.up * -1 * fastFallForce);
            //flump loop
            SFXManagerBehaviour.Instance.loopSound(this.gameObject, fallingSound);
        }
        else
            //SFXManagerBehaviour.Instance.stopLoop(this.gameObject, fallingSound);

        // If the player is changing direction or letting go of controls, stop all horizontal movement, but only on ground
        if (h != Mathf.Sign(rigidbody2D.velocity.x))
            rigidbody2D.velocity = new Vector2(0f, rigidbody2D.velocity.y);

        // If the player is changing direction or hasn't reached maxSpeed yet, add a force
        if (h * rigidbody2D.velocity.x < maxSpeed)
        {
            //if we're clining to a wall, then add a portion of the force so we don't get stuck into it
            if (!onWall)
                rigidbody2D.AddForce(Vector2.right * h * moveForce);
            else
                rigidbody2D.AddForce(Vector2.right * h * moveForce * 0.25f);

            //flip sprite
            //Vector3 scale = transform.localScale;
            //scale.x *= h;
            //transform.localScale = scale;
        }
        

        // If the player's horizontal velocity is greater than the maxSpeed, set it to max speed
        if (rigidbody2D.velocity.x > maxSpeed)
            rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * maxSpeed, rigidbody2D.velocity.y);
    }
    
    private void jumpHandler()
    {        
        //if hit wall with force, play sound and particles, and screen shake
        //only do this if we've hit thge wall for the first time
        if (firstTimeOnWall)
        {
            firstTimeOnWall = false;

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

            if (down)
                //flump sound
                SFXManagerBehaviour.Instance.playSound(flumpSound);
            else
                //landing sound
                SFXManagerBehaviour.Instance.playSound(landingSound);

            groundParticleSystem.Play();
            //screen shake

            //screen ripple
            Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
            viewingCamera.SendMessage("splashAtPoint", new Vector2(pos.x, pos.y));
        }
        
        //if jump is let go and we are jumping up, then stop veritcal movement
        if (rigidbody2D.velocity.y > 0.2f && letGoOfJump)
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0f);

        //normal air friciton
        rigidbody2D.AddForce(new Vector2((-rigidbody2D.velocity.x * 4), 0));

        //air friction if h let go
        if (inAir && h == 0)
            rigidbody2D.AddForce(new Vector2((-rigidbody2D.velocity.x * 6), 0));

        if (jump)
        {
            SFXManagerBehaviour.Instance.playSound(jumpSound);

            //nulify current force from gravity
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);

            //if we are on a wall then jump off it
            if (onWall && !grounded)
            {
                int direction = 1;

                if (onLeftWall)
                    direction = 1;

                if (onRightWall)
                    direction = -1;
                
                rigidbody2D.AddForce(Vector2.right * direction * moveForce * 5);
            }

            rigidbody2D.AddForce(Vector2.up * jumpForce);

            jump = false;
            inAir = true;
            hasJumped = true;
        }
        if (doubleJump)
        {
            SFXManagerBehaviour.Instance.playSound(dJumpSound);

            //nulify current force from gravity
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);

            rigidbody2D.AddForce(Vector2.up * jumpForce);

            doubleJump = false;
            inAir = true;
            hasDoubleJumped = true;
        }
    }

    private void debugDrawHitBoxes()
    {
        Vector2 playerBottomLeft = new Vector2(transform.position.x - collider2D.bounds.extents.x, transform.position.y - collider2D.bounds.extents.y);
        Vector2 playerBottomRight = new Vector2(transform.position.x + collider2D.bounds.extents.x, transform.position.y - collider2D.bounds.extents.y);

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

        Debug.DrawRay(playerTopLeft, Vector2.up * platRayHeight, Color.grey);
        Debug.DrawRay(playerTopRight, Vector2.up * platRayHeight, Color.grey);
    }

    public void setHit(bool val)
    {
        hit = val;
        if (hit & !dead)
            die();
    }

    public void respawn()
    {
        //null momentum
        rigidbody2D.velocity = Vector2.zero;

        hit = false;
        dead = false;

        invertColors();

        GetComponent<BoxCollider2D>().enabled = true;
        lastPressed = 0;
        anim.SetTrigger("Reset");
    }

    public void invertColors()
    {
        //invert color
        player.color.color = (new Color(1 - player.color.color.r, 1 - player.color.color.g, 1 - player.color.color.b, 1));

        spriteRenderer.color = player.color.color;
        groundParticleSystem.startColor = player.color.color;
        wallLeftParticleSystem.startColor = player.color.color;
        wallRightParticleSystem.startColor = player.color.color;
    }

    public int getPlayerNo()
    {
        return playerNo;
    }

    public int getTeamNo()
    {
        return teamNo;
    }

    public Player getPlayer()
    {
        return player;
    }

    public void setPlayer(Player plyer, List<int> opponentNumbers)
    {
        player = plyer;
        player.color.color.a = 1;
        spriteRenderer.color = player.color.color;
        spriteRenderer.sprite = player.sprite.sprite;
        groundParticleSystem.startColor = player.color.color;
        wallLeftParticleSystem.startColor = player.color.color;
        wallRightParticleSystem.startColor = player.color.color;

        playerNo = plyer.playerNo;
        teamNo = plyer.teamNo;

        //set our layermask
        this.gameObject.layer = LayerMask.NameToLayer("Player " + teamNo);

        opponentLayerMasks = new List<int>();

        //set opponentNumbers
        foreach (int opponentNumber in opponentNumbers)
            setLayerMask("Player " + opponentNumber.ToString());

        //set up the inputs for this player
        horizontalAxis = plyer.playerInputScheme.inputs[PlayerInput.HorizontalInput].inputName;
        jumpKey = plyer.playerInputScheme.inputs[PlayerInput.UpInput].inputName;
        downKey = plyer.playerInputScheme.inputs[PlayerInput.DownInput].inputName;

        if (plyer.playerInputScheme.inputs.ContainsKey(PlayerInput.RightInput) && plyer.playerInputScheme.inputs.ContainsKey(PlayerInput.LeftInput))
        {
            rightKey = plyer.playerInputScheme.inputs[PlayerInput.RightInput].inputName;
            leftKey = plyer.playerInputScheme.inputs[PlayerInput.LeftInput].inputName;
            keyboard = true;
        }
    }

    private void setLayerMask(string opMask)
    {
        opponentLayerMasks.Add(1 << LayerMask.NameToLayer(opMask));
        //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(mask), LayerMask.NameToLayer(opMask), true);
    }

    public int getScore()
    {
        return score;
    }

    public void setScore(int l)
    {
        score = l;
    }

    public void increaseScore()
    {
        score++;
    }

    public void decreaseScore()
    {
        score--;
    }

    public void increaseScore(int value)
    {
        score += value;
    }

    public void decreaseScore(int value)
    {
        score -= value;
    }

    public void setDebug(bool value)
    {
        debug = value;
    }

    public Color getColor()
    {
        return player.color.color;
    }

    public Sprite getSprite()
    {
        return player.sprite.sprite;
    }

    public bool getHit()
    {
        return hit;
    }
}

