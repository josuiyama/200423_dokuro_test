using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] LayerMask blockLayer;
    public enum DIRECTION_TYPE
    {
        STOP,
        RIGHT,
        LEFT,
    }

    DIRECTION_TYPE direction = DIRECTION_TYPE.STOP;

    Rigidbody2D rigidbody2D;
    float speed;
    bool isDead = false;

    private float hAxis;
    private bool control;

    Animator animator;

    //SE
    [SerializeField] AudioClip getItemSE;
    [SerializeField] AudioClip jumpSE;
    [SerializeField] AudioClip stampSE;
    AudioSource audioSource;


    float jumppower = 400;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        if (control)
        {
            hAxis = Input.GetAxis("Horizontal"); //方向キーの取得
        }
        animator.SetFloat("speed", Mathf.Abs(hAxis));

        if (hAxis == 0)
        {
            //止まっている
            direction = DIRECTION_TYPE.STOP;
        }
        else if (hAxis > 0)
        {
            //右へ
            direction = DIRECTION_TYPE.RIGHT;
        }
        else if (hAxis < 0)
        {
            //左へ
            direction = DIRECTION_TYPE.LEFT;
        }

        // ジャンプさせたい
        if (IsGround())
        {
            if (Input.GetKeyDown("space"))
            {
                Jump();
            }
            else
            {
                animator.SetBool("isJumping", false);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        switch (direction)
        {
            case DIRECTION_TYPE.STOP:
                speed = 0;
                break;
            case DIRECTION_TYPE.RIGHT:
                speed = 3;
                transform.localScale = new Vector3(1, 1, 1);
                break;
            case DIRECTION_TYPE.LEFT:
                speed = -3;
                transform.localScale = new Vector3(-1, 1, 1);
                break;
        }
        rigidbody2D.velocity = new Vector2(speed, rigidbody2D.velocity.y);
    }

    void Jump()
    {
        //上に力を加える
        rigidbody2D.AddForce(Vector2.up * jumppower);
        audioSource.PlayOneShot(jumpSE);
        animator.SetBool("isJumping", true);
    }

    bool IsGround()
    {
        // 始点と終点を作成
        Vector3 leftStartPoint = transform.position - Vector3.right * 0.2f;
        Vector3 rightStartPoint = transform.position + Vector3.right * 0.2f;
        Vector3 endPoint = transform.position - Vector3.up * 0.1f;
        Debug.DrawLine(leftStartPoint, endPoint);
        Debug.DrawLine(rightStartPoint, endPoint);
        return Physics2D.Linecast(leftStartPoint, endPoint, blockLayer)
            || Physics2D.Linecast(rightStartPoint, endPoint, blockLayer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead)
        {
            return;
        }
        if (collision.gameObject.tag == "Trap")
        {
            PlayerDeath();
        }
        if (collision.gameObject.tag == "Finish")

        {
            Debug.Log("クリア");
            gameManager.GameClear();
        }
        if (collision.gameObject.tag == "Item")

        {
            audioSource.PlayOneShot(getItemSE);
            collision.gameObject.GetComponent<ItemManeger>().GetItem();
        }

        if (collision.gameObject.tag == "Enemy")
        {
            EnemyManager enemy = collision.gameObject.GetComponent<EnemyManager>();
            if (this.transform.position.y + 0.2f > enemy.transform.position.y)
            {
                //上から踏んだら
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
                Jump();
                audioSource.PlayOneShot(stampSE);
                enemy.DestroyEnemy();
            }
            else
            {
                //横からぶつかったら
                PlayerDeath();
            }
        }
    }
    void PlayerDeath()
    {
        isDead = true;
        rigidbody2D.velocity = new Vector2(0, 0);
        rigidbody2D.AddForce(Vector2.up * jumppower);
        animator.Play("PlayerDeathAnimation");
        gameManager.GameOver();
        BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
        Destroy(boxCollider2D);
        gameManager.GameOver();
    }

    public void ChangeControl(bool controlFlag)
    {
        //切り替えした後に動きを止める
        hAxis = 0;
        control = controlFlag;
    }
}
