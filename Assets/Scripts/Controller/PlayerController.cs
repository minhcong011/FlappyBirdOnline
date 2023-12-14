using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Player
{
    [SerializeField] private float jumpForce;
    [SerializeField] private float fallSpeed;
    [SerializeField] private float speedMove;
    [SerializeField] private float speedRotationUp;
    [SerializeField] private float speedRotationDown;
    [SerializeField] private float velocityLowerToRotaion;

    private int speedBuftStage;
    private float currentSpeedMove;
    public static PlayerController instance;
    private int score;
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;

    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        data = new PlayerData();
    }
    public override void Start()
    {
        base.Start();
        UIManager.Instance.CreatePlayerName(gameObject, data);
    }

    // Update is called once per frame
    public override void Update()
    {
        SendDataToServer();
        HandleInput();
    }
    private void FixedUpdate()
    {
        Moving();
    }
    private void SendDataToServer()
    {

        if (GameManager.Instance.GetGameStage() != GameManager.GameStage.Playing && GameManager.Instance.GetGameStage() != GameManager.GameStage.StopPlay && GameManager.Instance.GetGameStage() != GameManager.GameStage.BeginPlay)
            return;
        data.x = (float)Math.Round(transform.position.x, 2);
        data.y = (float)Math.Round(transform.position.y, 2);
        data.rotationZ = (float)Math.Round(transform.rotation.z, 2);
        ServerManager.Instance.SendPlayerDataToServer(data.name, data.highScore, data.x, data.y, data.rotationZ);
    }
  
    private void HandleInput()
    {
    
    #if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBGL
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            StartPlay();
            Jump();
        }
    #endif
    #if UNITY_ANDROID || UNITY_IPHONE
        Touch touch = Input.GetTouch(0);
        if(touch.phase == TouchPhase.Began)
        {
            StartPlay();
            Jump();
        }
    #endif
    }
    private void Jump()
    {
        if (GameManager.Instance.GetGameStage() != GameManager.GameStage.Playing) return;
        rb.velocity = Vector2.up * jumpForce;
        AudioManager.Instance.PlaySound("wing");
    }
    private void Moving()
    {
        if (GameManager.Instance.GetGameStage() != GameManager.GameStage.Playing) return;

        transform.position = new(transform.position.x + (currentSpeedMove + (0.2f*speedBuftStage)) * Time.deltaTime, transform.position.y);

        Vector3 newRotaion = new(transform.rotation.x, transform.rotation.y, GetRotationZ());
        float speedRotation = rb.velocity.y > 0 ? speedRotationUp : speedRotationDown;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newRotaion), speedRotation * Time.fixedDeltaTime);
    }
    private float GetRotationZ()
    {
        if (rb.velocity.y > 0) return 30;
        else
        {
            return -90;
        }
    }
    public void StartPlay()
    {
        if (GameManager.Instance.GetGameStage() == GameManager.GameStage.BeginPlay)
        {
            GameManager.Instance.SetGameStage(GameManager.GameStage.Playing);
            circleCollider.enabled = true;
            currentSpeedMove = speedMove;
            rb.gravityScale = fallSpeed;
        }
    }
    public void ResetPlayerInGame()
    {
        GameManager.Instance.SetGameStage(GameManager.GameStage.BeginPlay);
        ResetPlayer();
    }
    public void ResetPlayerMainMenu()
    {
        GameManager.Instance.SetGameStage(GameManager.GameStage.MainMenu);
        data.highScore = 0;
        ResetPlayer();
    }
    private void ResetPlayer()
    {
        TileMapManager.Instance.ClearHolder();
        rb.gravityScale = 0;
        transform.position = new(0, 0);
        rb.velocity = Vector2.zero;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        score = 0;
        speedBuftStage = 0;
        UIManager.Instance.SetSpeedBuftText(speedBuftStage);
        animator.Play("Birt1");
    }
    private void IncreaseScore()
    {
        score++;
        if (score > data.highScore) data.highScore = score;
    }
    private void OnLostGame()
    {
        circleCollider.enabled = false;

        GameManager.Instance.SetGameStage(GameManager.GameStage.StopPlay);
        AudioManager.Instance.PlaySound("hit");
        UIManager.Instance.ShowLostGamePanel(score);

        score = 0;
        currentSpeedMove = 0;

        UIManager.Instance.UpdateLocalPlayerScoreText(score);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnLostGame();

    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.gameObject.CompareTag("UpScore") || collision.gameObject.CompareTag("ChangeSkin"))
        {
            IncreaseScore();
            AudioManager.Instance.PlaySound("point");
            UIManager.Instance.UpdateLocalPlayerScoreText(score);
        }
        if (collision.gameObject.CompareTag("ChangeSkin"))
        {
            speedBuftStage++;
            UIManager.Instance.SetSpeedBuftText(speedBuftStage);
            UIManager.Instance.CreateTranformTextEf();
        }
    }
}
