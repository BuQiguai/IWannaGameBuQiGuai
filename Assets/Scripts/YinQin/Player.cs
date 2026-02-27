using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;
[DefaultExecutionOrder(-100)] // 优先级越小，越早执行
public class Player : MonoBehaviour
{
    float jump = -8.5f;
    float jump2 = -7;
    float maxSpeed = 3;
    float maxVspeed = 9;

    public bool djump = true;

    public float hspeed = 0;
    public float vspeed = 0;
    float gravity = -0.4f;

    public float x { get => transform.position.x; set => transform.position = new Vector3(value, y, transform.position.z); }
    public float y { get => transform.position.y; set => transform.position = new Vector3(x, value, transform.position.z); }

    float xprevious;
    float yprevious;

    bool onPlatform = false;

    public GameObject sprite;
    SpriteAnimator animator;

    public World world;
    public GameObject worldPrefab;
    PixelPerfectCollider collider;
    public BloodEmitter bloodEmitter;
    public Bullet bullet;


    public static bool invincible = false;

#if UNITY_EDITOR
    private void Awake()
    {

        if (PlayInput._instance == null)
        {
            //初始化一个PlayInput单例
            new GameObject("PlayInput").AddComponent<PlayInput>();
        }


        // Do something to help us debug the level
        if (GameObject.FindObjectsOfType<World>().Length < 1)
        {
            var 对象 = GameObject.Instantiate(worldPrefab);
            对象.name = "全局世界单位";
            World.instance.gameStarted = true;
            World.instance.saveScene = SceneManager.GetActiveScene().name;
            World.instance.autosave = true;
        }
    }
#endif

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        collider = GetComponent<PixelPerfectCollider>();
        animator = sprite.GetComponent<SpriteAnimator>();

        if (World.instance.autosave)
        {
            World.instance.SaveGame(true);
            World.instance.autosave = false;
        }
    }
    public void Die()
    {
        if (!invincible)
        {
            var inst = GameObject.Instantiate(bloodEmitter);
            inst.transform.position = transform.position;
            GameObject.Destroy(gameObject);
            World.instance.KillPlayer();
        }
    }
    void FixedUpdate()
    {
        // 在需要计时的代码块前后直接插入 Stopwatch

        xprevious = x;
        yprevious = y;

        var L = PlayInput.获取按键状态(KeyCode.A);
        var R = PlayInput.获取按键状态(KeyCode.D);

        var h = 0;

        if (R)
            h = 1;
        else if (L)
            h = -1;

        var notOnBlock = !collider.PlaceMeeting(x, y - 1, "Block");
        var onVineL = collider.PlaceMeeting(x - 1, y, "WalljumpL") && notOnBlock;
        var onVineR = collider.PlaceMeeting(x + 1, y, "WalljumpR") && notOnBlock;

        if (h != 0)
        {
            if (h == -1)
                sprite.transform.localScale = new Vector3(-1, 1);
            else if (h == 1)
                sprite.transform.localScale = new Vector3(1, 1);

            animator.currentAnimation = "Running";
            animator.imageSpeed = 0.5f;
        }
        else
        {
            animator.currentAnimation = "Idle";
            animator.imageSpeed = 0.2f;
        }
        hspeed = maxSpeed * h;

        if (!onPlatform)
        {
            if (vspeed > 0.05)
            {
                animator.currentAnimation = "Jump";
            }
            else if (vspeed < -0.05)
            {
                animator.currentAnimation = "Fall";
            }
        }
        else
        {
            if (!collider.PlaceMeeting(x, y - 4, "Platform"))
            {
                onPlatform = false;
            }
        }

        if (Abs(vspeed) > maxVspeed)
        {
            vspeed = Sign(vspeed) * maxVspeed;
        }

        if (PlayInput.获取按键按下状态(KeyCode.K))
            Shoot();

        if (PlayInput.获取按键按下状态(KeyCode.J) || PlayInput.获取按键按下状态(KeyCode.J))
            Jump();

        if (PlayInput.获取按键抬起状态(KeyCode.J) || PlayInput.获取按键抬起状态(KeyCode.J))
            VJump();

        // Walljumps
        if (onVineL || onVineR)
        {
            if (onVineR)
                sprite.transform.localScale = new Vector3(-1, 1);
            else
                sprite.transform.localScale = new Vector3(1, 1);

            vspeed = -2;
            animator.currentAnimation = "Sliding";
            animator.imageSpeed = 0.5f;

            if ((onVineL && PlayInput.获取按键状态(KeyCode.D)) || (onVineR && PlayInput.获取按键状态(KeyCode.A)))
            {
                if (PlayInput.获取按键状态(KeyCode.J) || PlayInput.获取按键状态(KeyCode.J))
                {
                    if (onVineR)
                        hspeed = -15;
                    else
                        hspeed = 15;

                    vspeed = 9;

                    声音.Play音效(声音.SoundEffect.sndWallJump);

                    animator.currentAnimation = "Jump";
                }
                else
                {
                    if (onVineR)
                        hspeed = -3;
                    else
                        hspeed = 3;

                    animator.currentAnimation = "Fall";
                }
            }
        }

        // Move
        vspeed += gravity;
        x += hspeed;
        y += vspeed;

        // Collision

        // Block check
        if (collider.PlaceMeeting(x, y, "Block"))
        {
            x = xprevious;
            y = yprevious;

            if (collider.PlaceMeeting(x + hspeed, y, "Block"))
            {
                if (hspeed <= 0) while (!collider.PlaceMeeting(x - 1, y, "Block")) x--;
                if (hspeed > 0) while (!collider.PlaceMeeting(x + 1, y, "Block")) x++;
                hspeed = 0;
            }

            if (collider.PlaceMeeting(x, y + vspeed, "Block"))
            {
                if (vspeed >= 0) while (!collider.PlaceMeeting(x, y + 1, "Block")) y++;
                if (vspeed < 0)
                {
                    while (!collider.PlaceMeeting(x, y - 1, "Block")) y--;
                    djump = true;
                }
                vspeed = 0;
            }

            if (collider.PlaceMeeting(x + hspeed, y + vspeed, "Block"))
            {
                hspeed = 0;
            }

            x += hspeed;
            y += vspeed;
            if (collider.PlaceMeeting(x, y, "Block"))
            {
                x = xprevious;
                y = yprevious;
            }
        }

        // Platform check
        var platform = collider.InstancePlace(x, y, "Platform");
        if (platform != null)
        {
            if (y - vspeed / 2 >= platform.transform.position.y)
            {
                var vsp = platform.GetComponent<MovingPlatform>().vspeed;
                if (vsp <= 0)
                {
                    y = platform.transform.position.y + 9; //平面  那  
                    vspeed = vsp;
                }
                onPlatform = true;
                djump = true;
            }
        }

        // Killer check
        if (collider.PlaceMeeting(x, y, "Killer"))
        {
            Die();
        }
        // FixedUpdate position
        transform.position = new Vector3(x, y);

    }
    void Jump()
    {


        if (collider.PlaceMeeting(x, y - 1, "Block") || collider.PlaceMeeting(x, y - 1, "Platform") || onPlatform
            || collider.PlaceMeeting(x, y - 1, "Water"))
        {

            // Debug.Log("Jump");
            vspeed = -jump;
            djump = true;

            声音.Play音效(声音.SoundEffect.sndJump);
        }
        else if (djump || collider.PlaceMeeting(x, y - 1, "Water2") || Player.invincible)
        {

            // Debug.Log("Jump");
            animator.currentAnimation = "Jump";
            vspeed = -jump2;
            djump = false;
            // djumpSound.Play();
            声音.Play音效(声音.SoundEffect.sndDJump);

            if (!collider.PlaceMeeting(x, y - 1, "Water3"))
                djump = false;
            else
                djump = true;
        }
    }
    void VJump()
    {
        if (vspeed > 0)
            vspeed *= 0.45f;
    }
    void Shoot()
    {
        if (GameObject.FindObjectsOfType<Bullet>().Length < 4)
        {
            var inst = GameObject.Instantiate(bullet);
            inst.transform.position = new Vector3(x, y);
            // shootSound.Play();
            声音.Play音效(声音.SoundEffect.sndShoot);

        }
    }
}
