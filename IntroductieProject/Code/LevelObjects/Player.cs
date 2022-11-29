using Engine;
using IntroductieProject;
using IntroductieProject.Code.LevelObjects;
using IntroductieProject.Code.View.LevelObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

class Player : AnimatedGameObject
{
    const float walkingSpeed = 400; // Standard walking speed, in game units per second.
    const float jumpSpeed = 900; // Lift-off speed when the character jumps.
    const float gravity = 2300; // Strength of the gravity force that pulls the character down.
    const float maxFallSpeed = 1200; // The maximum vertical speed at which the character can fall.

    //const float iceFriction = 1; // Friction factor that determines how slippery the ice is; closer to 0 means more slippery.
    const float normalFriction = 20; // Friction factor that determines how slippery a normal surface is.
    //const float airFriction = 5; // Friction factor that determines how much (horizontal) air resistance there is.

    bool facingLeft; // Whether or not the character is currently looking to the left.

    bool isGrounded; // Whether or not the character is currently standing on something.
    // bool standingOnIceTile, standingOnHotTile; // Whether or not the character is standing on an ice tile or a hot tile.
    float desiredHorizontalSpeed; // The horizontal speed at which the character would like to move.

    public int hearts = 5;
    protected int maxhearts = 5;
    Level level;
    Vector2 startPosition;
    public List<Hearts> Hearts = new List<Hearts>();

    bool isCelebrating; // Whether or not the player is celebrating a level victory.

    public bool IsAlive { get; private set; }

    public bool CanCollideWithObjects { get { return IsAlive && !isCelebrating; } }

    public bool IsMoving { get { return velocity != Vector2.Zero; } }

    public Player(Level level, Vector2 startPosition) : base(OfficeBaby.Depth_LevelPlayer)
    {
        this.level = level;
        this.startPosition = startPosition;

        // load all animations
        LoadAnimation("HIER MOET DE IDLE ANIMATION KOMEN", "idle", true, 0.1f);
        LoadAnimation("HIER MOET DE RUN/CRAWL ANIMATION KOMEN", "run", true, 0.04f);
        LoadAnimation("HIER MOET DE JUMP ANIMATION KOMEN", "jump", false, 0.08f);
        LoadAnimation("HIER MOET DE CELEBRATE ANIMATION KOMEN", "celebrate", false, 0.05f);
        LoadAnimation("HIER MOET DE DIE ANIMATION KOMEN", "die", true, 0.1f);

        //load all hearts
        foreach (Hearts Heart in Hearts)
        {

        }

        Reset();
    }

    public override void Reset()
    {
        // go back to the starting position
        localPosition = startPosition;
        velocity = Vector2.Zero;
        desiredHorizontalSpeed = 0;

        // start with the idle sprite
        //PlayAnimation("idle", true);
        SetOriginToBottomCenter();
        facingLeft = false;
        isGrounded = true;

        IsAlive = true;
        isCelebrating = false;
    }

    public override void HandleInput(InputHelper inputHelper)
    {
        if (!CanCollideWithObjects)
            return;

        // arrow keys: move left or right
        if (inputHelper.KeyDown(Keys.Left))
        {
            facingLeft = true;
            desiredHorizontalSpeed = -walkingSpeed;
            //if (isGrounded)
               // PlayAnimation("run");
        }
        else if (inputHelper.KeyDown(Keys.Right))
        {
            facingLeft = false;
            desiredHorizontalSpeed = walkingSpeed;
            //if (isGrounded)
                //PlayAnimation("run");
        }

        // no arrow keys: don't move
        else
        {
            desiredHorizontalSpeed = 0;
            //if (isGrounded)
               // PlayAnimation("idle");
        }

        // spacebar: jump
        if (isGrounded && inputHelper.KeyPressed(Keys.Space))
            Jump();

        // falling?
        if (!isGrounded)
            PlayAnimation("jump", false, 8);

        // set the origin to the character's feet
        SetOriginToBottomCenter();

        // make sure the sprite is facing the correct direction
        sprite.Mirror = facingLeft;
    }

    public void Jump(float speed = jumpSpeed)
    {
        velocity.Y = -speed;
        // play the jump animation; always make sure that the animation restarts
        //PlayAnimation("jump", true);
        // play a sound
        //ExtendedGame.AssetManager.PlaySoundEffect("Sounds/snd_player_jump");
    }

    /// <summary>
    /// Returns whether or not the Player is currently falling down.
    /// </summary>
    public bool IsFalling
    {
        get { return velocity.Y > 0 && !isGrounded; }
    }

    void SetOriginToBottomCenter()
    {
        Origin = new Vector2(sprite.Width / 2, sprite.Height);
    }

    public override void Update(GameTime gameTime)
    {
        Vector2 previousPosition = localPosition;

        if (CanCollideWithObjects)
            ApplyFriction(gameTime);
        else
            velocity.X = 0;

            
        base.Update(gameTime);

        if (IsAlive)
        {
            ApplyGravity(gameTime);
            // check for collisions with tiles
            HandleTileCollisions(previousPosition);
            // check if we've fallen down through the level
            if (BoundingBox.Center.Y > level.BoundingBox.Bottom)
                Die();
        }

    }

    void ApplyFriction(GameTime gameTime)
    {
        // determine the friction coefficient for the character
        float friction = normalFriction;       
            
        // calculate how strongly the horizontal speed should move towards the desired value
        float multiplier = MathHelper.Clamp(friction * (float)gameTime.ElapsedGameTime.TotalSeconds, 0, 1);

        // update the horizontal speed
        velocity.X += (desiredHorizontalSpeed - velocity.X) * multiplier;
        if (Math.Abs(velocity.X) < 1)
            velocity.X = 0;
    }

    void ApplyGravity(GameTime gameTime)
    {
        velocity.Y += gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (velocity.Y > maxFallSpeed)
            velocity.Y = maxFallSpeed;
    }

    // Checks for collisions between the character and the level's tiles, and handles these collisions when needed.
    void HandleTileCollisions(Vector2 previousPosition)
    {
        isGrounded = false;

        // determine the range of tiles to check
        Rectangle bbox = BoundingBoxForCollisions;
        Point topLeftTile = level.GetTileCoordinates(new Vector2(bbox.Left, bbox.Top)) - new Point(1, 1);
        Point bottomRightTile = level.GetTileCoordinates(new Vector2(bbox.Right, bbox.Bottom)) + new Point(1, 1);

        for (int y = topLeftTile.Y; y <= bottomRightTile.Y; y++)
        {
            for (int x = topLeftTile.X; x <= bottomRightTile.X; x++)
            {
                Tile.Type tileType = level.GetTileType(x, y);

                // ignore empty tiles
                if (tileType == Tile.Type.Empty)
                    continue;

                // ignore platform tiles if the player is standing below them
                Vector2 tilePosition = level.GetCellPosition(x, y);
                if (tileType == Tile.Type.Platform && localPosition.Y > tilePosition.Y && previousPosition.Y > tilePosition.Y)
                    continue;

                // if there's no intersection with the tile, ignore this tile
                Rectangle tileBounds = new Rectangle((int)tilePosition.X, (int)tilePosition.Y, Level.TileWidth, Level.TileHeight);
                if (!tileBounds.Intersects(bbox))
                    continue;

                // calculate how large the intersection is
                Rectangle overlap = CollisionDetection.CalculateIntersection(bbox, tileBounds);

                // if the x-component is smaller, treat this as a horizontal collision
                if (overlap.Width < overlap.Height)
                {
                    if ((velocity.X >= 0 && bbox.Center.X < tileBounds.Left) || // right wall
                        (velocity.X <= 0 && bbox.Center.X > tileBounds.Right)) // left wall
                    {
                        localPosition.X = previousPosition.X;
                        velocity.X = 0;
                    }
                }

                // otherwise, treat this as a vertical collision
                else
                {
                    if (velocity.Y >= 0 && bbox.Center.Y < tileBounds.Top && overlap.Width > 6) // floor
                    {
                        isGrounded = true;
                        velocity.Y = 0;
                        localPosition.Y = tileBounds.Top;
                    }
                    else if (velocity.Y <= 0 && bbox.Center.Y > tileBounds.Bottom && overlap.Height > 2) // ceiling
                    {
                        localPosition.Y = previousPosition.Y;
                        velocity.Y = 0;
                    }
                }
            }
        }
    }


    public void AddHearts()
    {
        if (hearts < maxhearts)
        {
            hearts++;
        }

    }
    public void RemoveHearts()
    {
        if (hearts > 0)
        {
            hearts--;
        }
        else
        {
            Die();
        }

    }
    Rectangle BoundingBoxForCollisions
    {
        get
        {
            Rectangle bbox = BoundingBox;
            // adjust the bounding box
            bbox.X += 20;
            bbox.Width -= 40;
            bbox.Height += 1;

            return bbox;
        }
    }

    public void Die()
    {
        IsAlive = false;
        //PlayAnimation("die");
        velocity = new Vector2(0, -jumpSpeed);

        //ExtendedGame.AssetManager.PlaySoundEffect("Sounds/snd_player_die");
    }

    /// <summary>
    /// Lets this Player object start celebrating due to completing a level.
    /// The Player will show an animation, and it will stop responding to keyboard input.
    /// </summary>
    public void Celebrate()
    {
        isCelebrating = true;
        //PlayAnimation("celebrate");
        SetOriginToBottomCenter();

        // stop moving
        velocity = Vector2.Zero;
    }
}