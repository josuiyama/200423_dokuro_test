using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] LayerMask blockLayer;
    [SerializeField]
    private GemPlayerManager gemPlayerManager;

    [SerializeField]
    private Vector3 m_moveDirection = Vector3.left;

    public enum DIRECTION_TYPE
    {
        STOP,
        RIGHT,
        LEFT,
        UP,
        DOWN,
    }

    DIRECTION_TYPE direction = DIRECTION_TYPE.STOP;

    // スプライトレンダラーコンポーネントを入れる
    private SpriteRenderer sr;
    private Rigidbody2D rb2D;
    private Transform tf;

    public float speed;
    private float hAxis;
    private float vAxis;

    private bool control;
    private bool isDead = false;
    private bool canClimb = false;
    public float distance;
    public LayerMask ladderLayer;
    public LayerMask chainLayer;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        // スプライトレンダラーのコンポーネントを取得する
        this.sr = GetComponent<SpriteRenderer>();
        tf = GetComponent<Transform>();
    }

    private void Update()
    {
        IsDead();

        if (control)
        {
            GetHorizontalVertical();
            HAxis();
            CanClimb();
            //CanHighCliff();
            //IsGround();
            //IsSlope();
            //NormalizeSlope();
        }
    }

    // 矢印キーの入力情報を取得
    //横移動スピードの取得
    private void GetHorizontalVertical()
    {
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");
        rb2D.velocity = new Vector2(hAxis * speed, rb2D.velocity.y);
    }

    //横動き方向の取得
    //横方向変更でイラスト反転
    private void HAxis()
    {
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

        switch (direction)
        {
            case DIRECTION_TYPE.STOP:
                break;
            case DIRECTION_TYPE.RIGHT:
                sr.flipX = false;
                break;
            case DIRECTION_TYPE.LEFT:
                sr.flipX = true;
                break;
        }
    }

    //接地判定
    //これなんでVector2じゃなくて3なんだろ？
    private bool IsGround()
    {
        // 始点と終点を作成
        Vector3 leftStartPoint = transform.position + Vector3.left * 0.2f;
        Vector3 rightStartPoint = transform.position + Vector3.right * 0.2f;
        Vector3 endPoint = transform.position - Vector3.up * 0.1f;
        Debug.DrawLine(leftStartPoint, endPoint, Color.red);
        Debug.DrawLine(rightStartPoint, endPoint, Color.red);
        return Physics2D.Linecast(leftStartPoint, endPoint, blockLayer)
            || Physics2D.Linecast(rightStartPoint, endPoint, blockLayer);
    }

    //クリア・ゲームオーバー判定
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
            collision.gameObject.GetComponent<ItemManeger>().GetItem();
        }
    }

    //死亡判定時の処理
    private void PlayerDeath()
    {
        isDead = true;
        rb2D.velocity = new Vector2(0, 0);
        gameManager.GameOver();
        BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
        Destroy(boxCollider2D);
        gameManager.GameOver();
    }

    //死亡判定のときは処理を止める
    private void IsDead()
    {
        if (isDead)
        {
            return;
        }
    }

    //操作キャラ変更
    public void ChangeControl(bool controlFlag)
    {
        //切り替えした後に動きを止める
        hAxis = 0;
        control = controlFlag;
    }

    //はしごや鎖を登るのと縦スピードの取得
    private void CanClimb()
    {
        //Ladderレイヤーと接触した時登れる
        //Physics2D.Raycast(どこから　どの方向に　どれくらいの距離で　検出対象のレイヤー);
        RaycastHit2D hitLadder = Physics2D.Raycast(transform.position, Vector2.up, 2f, ladderLayer);
        //chainレイヤーと接触した時登れる
        RaycastHit2D hitChain = Physics2D.Raycast(transform.position, Vector2.up, 2f, chainLayer);

        Debug.DrawRay(transform.position, Vector2.up * 2f, Color.green);

        //0個よりも多くのレイヤーに接触した時
        if (hitLadder.collider != null)
        {
            //プレイヤーが上下キーを押す（=上下に値を入れる）とcanClimbがtrueになる=登れるようになる
            if (vAxis != 0)
            {
                canClimb = true;
            }
            //登るときのスピード設定と重力設定
            if (canClimb)
            {
                rb2D.velocity = new Vector2(rb2D.velocity.x, vAxis * speed);
                rb2D.gravityScale = 0;
            }
        }

        //if (hitChain.collider != null)
        //{
        //    //噛み付いてる最中に
        //    if (gemPlayerManager.IsBite)
        //    {
        //        //プレイヤーが上下キーを押す（=上下に値を入れる）とcanClimbがtrueになる=登れるようになる
        //        if (vAxis != 0)
        //        {
        //            canClimb = true;
        //        }
        //        //登るときのスピード設定と重力設定
        //        if (canClimb)
        //        {
        //            rb2D.velocity = new Vector2(rb2D.velocity.x, vAxis * speed);
        //            rb2D.gravityScale = 0;
        //        }
        //    }
        //}
        else
        {
            canClimb = false;
            rb2D.gravityScale = 5;
        }
    }

    //壁の判定
    private void CanHighCliff()
    {

        RaycastHit2D hitInfoHighCliffLeft = Physics2D.Raycast(transform.position + Vector3.up * 1f, Vector2.left, 0.5f, blockLayer);
        RaycastHit2D hitInfoHighCliffRIght = Physics2D.Raycast(transform.position + Vector3.up * 1f, Vector2.right, 0.5f, blockLayer);

        if (hitInfoHighCliffLeft.collider != null
            || hitInfoHighCliffRIght.collider != null)
        {
            this.gameObject.transform.Translate(0f, 1f, 0f);

        }
        Debug.DrawRay(transform.position + Vector3.up * 1f, Vector2.left * 0.5f, Color.red);
        Debug.DrawRay(transform.position + Vector3.up * 1f, Vector2.right * 0.5f, Color.red);
    }

    ////坂道の判定
    //カプセルコライダーによってお役御免！！悲しい
    private void IsSlope()
    {
        RaycastHit2D hitInfoSlopeBlockLeft = Physics2D.Raycast(transform.position + Vector3.up * 0.1f, Vector2.left, 0.5f, blockLayer);
        RaycastHit2D hitInfoSlopeBlockRIght = Physics2D.Raycast(transform.position + Vector3.up * 0.1f, Vector2.right, 0.5f, blockLayer);

        if (hitInfoSlopeBlockLeft.collider != null
            || hitInfoSlopeBlockRIght.collider != null)
        {
            rb2D.gravityScale = 2;

        }
        else
        {
            rb2D.gravityScale = 5;
        }
        Debug.DrawRay(transform.position + Vector3.up * 0.1f, Vector2.left * 0.5f, Color.red);
        Debug.DrawRay(transform.position + Vector3.up * 0.1f, Vector2.right * 0.5f, Color.red);
    }

    private void NonSlope()
    {
        RaycastHit2D hitInfoNonSlopeBlockLeft = Physics2D.Raycast(transform.position + Vector3.left * 0.5f, Vector2.down, 0.01f, blockLayer);
        RaycastHit2D hitInfoNonSlopeBlockRIght = Physics2D.Raycast(transform.position + Vector3.right * 0.5f, Vector2.down, 0.01f, blockLayer);

        //if (hitInfoNonSlopeBlockLeft.collider != null
        //    || hitInfoNonSlopeBlockRight.collider != null)
        //{
        //    this.gameObject.transform.Translate(0.000001f, 0f, 0f);
        //    rb2D.gravityScale = 2;
        //}

        Debug.DrawRay(transform.position + Vector3.left * 0.2f, Vector2.down * 0.05f, Color.red);
        Debug.DrawRay(transform.position + Vector3.right * 0.2f, Vector2.down * 0.05f, Color.red);
    }
    /// <summary>
    /// 坂をノーマライズしてくれるらしい…
    /// https://www.youtube.com/watch?v=xMhgxUFKakQ
    /// </summary>
    private void NormalizeSlope()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 1f, blockLayer);

        if (hit.collider != null && Mathf.Abs(hit.normal.x) > 0.1f)
        {
            Rigidbody2D body = GetComponent<Rigidbody2D>();

            //Apply the opposite force against the slope force
            //傾斜力に対して反対の力を適用します
            //You will need to provide your own slopeFriction to stabalize movement
            //動きを安定させるために独自のスロープ摩擦を提供する必要があります
            body.velocity = new Vector2(body.velocity.x - (hit.normal.x * 0.6f), body.velocity.y);

            //Move Player up or down to compensate for the slope below them
            //プレイヤーを上下に動かして、その下の傾斜を補正します
            Vector3 pos = transform.position;
            pos.y += -hit.normal.x * Mathf.Abs(body.velocity.x) * (body.velocity.x - hit.normal.x > 0 ? 1 : -1);
            transform.position = pos;
        }
        Debug.DrawRay(transform.position, -Vector2.up * 1f, Color.red);
    }
}


