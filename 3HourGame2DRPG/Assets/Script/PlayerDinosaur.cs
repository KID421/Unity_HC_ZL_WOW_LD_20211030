using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class PlayerDinosaur : MonoBehaviour
{
    #region 欄位
    [Header("移動速度"), Range(0, 1000)]
    public float speed = 1.5f;
    [Header("跳躍高度"), Range(0, 1000)]
    public float jump = 100;
    [Header("檢查地板位移與半徑")]
    public Vector3 v2GroundOffset;
    public float radiusGround = 0.1f;
    [Header("音效")]
    public AudioClip soundJump;
    public AudioClip soundLand;
    public AudioClip soundChest;
    public AudioClip soundPass;
    public AudioClip soundDead;

    private Rigidbody2D rig;
    private Animator ani;
    private AudioSource aud;
    private float h;
    private float hJoystick;
    private bool isGrounded;
    private bool dead;
    /// <summary>
    /// 輸入右鍵：D 與 方向右
    /// </summary>
    private bool inputRight
    {
        get
        {
            return Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
        }
    }
    /// <summary>
    /// 輸入左鍵：A 與 方向左
    /// </summary>
    private bool inputLeft
    {
        get
        {
            return Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
        }
    }
    /// <summary>
    /// 吃到寶箱
    /// </summary>
    private bool hasChest;
    /// <summary>
    /// 是否過關
    /// </summary>
    private bool pass;
    /// <summary>
    /// 是否遊戲結束
    /// </summary>
    private bool gameOver;
    /// <summary>
    /// 過關畫面
    /// </summary>
    private CanvasGroup groupGameOver;
    /// <summary>
    /// 重新挑戰
    /// </summary>
    private Button btnReplay;
    /// <summary>
    /// 跳躍按鈕
    /// </summary>
    private Button btnJump;
    /// <summary>
    /// 虛擬搖桿
    /// </summary>
    private Joystick joystick;


    /// <summary>
    /// 跳躍按鈕
    /// </summary>
    public bool jumpButton { get; set; }
    #endregion

    #region 事件
    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();

        groupGameOver = GameObject.Find("過關畫面").GetComponent<CanvasGroup>();
        btnReplay = GameObject.Find("重新挑戰").GetComponent<Button>();
        btnReplay.onClick.AddListener(Replay);
        btnJump = GameObject.Find("跳躍按鈕").GetComponent<Button>();
        btnJump.onClick.AddListener(() => { jumpButton = true; });
        joystick = GameObject.Find("虛擬搖桿").GetComponent<Joystick>();
    }

    private void Update()
    {
        if (dead || pass) return;

        MoveInput();
        Jump();
    }

    private void FixedUpdate()
    {
        if (dead || pass) return;

        Move();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawSphere(transform.position + v2GroundOffset, radiusGround);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!dead && collision.name == "死亡區域") Dead();
        Eat(collision.gameObject);
        Pass(collision.name);
    }
    #endregion

    #region 方法
    /// <summary>
    /// 移動輸入
    /// </summary>
    private void MoveInput()
    {
        h = Input.GetAxis("Horizontal");
        hJoystick = joystick.Horizontal;

        if (inputRight) transform.eulerAngles = Vector3.zero;
        else if (inputLeft) transform.eulerAngles = new Vector3(0, 180, 0);

        if (hJoystick >= 0.1f) transform.eulerAngles = Vector3.zero;
        else if (hJoystick <= -0.1f) transform.eulerAngles = new Vector3(0, 180, 0);
    }

    /// <summary>
    ///移動
    /// </summary>
    private void Move()
    {
        rig.velocity = new Vector2(h * speed * Time.fixedDeltaTime + hJoystick * speed * Time.fixedDeltaTime, rig.velocity.y);
    }

    /// <summary>
    /// 跳躍輸入
    /// </summary>
    private void Jump()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || jumpButton) && isGrounded)
        {
            jumpButton = false;
            aud.PlayOneShot(soundJump, Random.Range(0.8f, 1.2f));
            rig.AddForce(new Vector2(0, jump));
        }

        Collider2D hit = Physics2D.OverlapCircle(transform.position + v2GroundOffset, radiusGround, 1 << 8);
        if (!isGrounded && hit) aud.PlayOneShot(soundLand, Random.Range(0.8f, 1.2f));
        isGrounded = hit && hit.name.Contains("地板");
        ani.SetBool("跳躍開關", !isGrounded);
    }

    /// <summary>
    /// 死亡
    /// </summary>
    private void Dead()
    {
        dead = true;
        ani.SetBool("死亡開關", true);
        rig.velocity = Vector3.zero;
        rig.AddForce(new Vector2(0, 800));
        Invoke("Replay", 2.5f);
        aud.PlayOneShot(soundDead);
    }

    /// <summary>
    /// 吃東西
    /// </summary>
    /// <param name="collisionName">碰撞物件名稱</param>
    private void Eat(GameObject collisionObj)
    {
        if (collisionObj.name == "寶箱")
        {
            hasChest = true;
            Destroy(collisionObj);
            aud.PlayOneShot(soundChest);
        }
    }

    /// <summary>
    /// 過關
    /// </summary>
    /// <param name="collisionName">碰撞物件名稱</param>
    private void Pass(string collisionName)
    {
        if (collisionName == "過關區域" && hasChest)
        {
            pass = true;
            rig.velocity = Vector3.zero;
            aud.PlayOneShot(soundPass);

            if (SceneManager.GetActiveScene().buildIndex == (SceneManager.sceneCountInBuildSettings - 1)) StartCoroutine(GameOver());
            else Invoke("NextLevel", 2);
        }
    }

    /// <summary>
    /// 下一關
    /// </summary>
    private void NextLevel()
    {
        int index = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(index);
    }

    /// <summary>
    /// 遊戲結束
    /// </summary>
    private IEnumerator GameOver()
    {
        gameOver = true;

        while (groupGameOver.alpha < 1)
        {
            groupGameOver.alpha += 0.1f;
            yield return new WaitForSeconds(0.02f);
        }

        groupGameOver.interactable = true;
        groupGameOver.blocksRaycasts = true;
    }

    /// <summary>
    /// 重新遊戲
    /// </summary>
    public void Replay()
    {
        SceneManager.LoadScene(0);
    }
    #endregion
}
