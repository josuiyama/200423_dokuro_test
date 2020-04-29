using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] LayerMask blockLayer;
    [SerializeField]
    private GemPlayerManager gemPlayerManager;

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
    SpriteRenderer sr;
    Rigidbody2D rb2D;
    Transform tf;

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
        if (isDead)
        {
            return;
        }

        if (control)
        {
            GetHorizontalVertical();
            HAxis();
            CanClimb();
            //CanCliff();
            IsGround();
            FindSlope();
        }
        //Debug.Log(canClimb + " canClimb");

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

    //死亡判定
    private void PlayerDeath()
    {
        isDead = true;
        rb2D.velocity = new Vector2(0, 0);
        gameManager.GameOver();
        BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
        Destroy(boxCollider2D);
        gameManager.GameOver();
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
        RaycastHit2D hitInfoLadder = Physics2D.Raycast(transform.position, Vector2.up, 2f, ladderLayer);
        //chainレイヤーと接触した時登れる
        RaycastHit2D hitInfoChain = Physics2D.Raycast(transform.position, Vector2.up, 2f, chainLayer);

        Debug.DrawRay(transform.position, Vector2.up * 2f, Color.green);

        //0個よりも多くのレイヤーに接触した時
        if (hitInfoLadder.collider != null)
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

        if (hitInfoChain.collider != null)
        {
            //噛み付いてる最中に
            if (gemPlayerManager.IsBite)
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
        }
        else
        {
            canClimb = false;
            rb2D.gravityScale = 5;
        }
    }

    //壁の判定
    //private void FindCliff()
    //{

    //    Vector3 upStartPoint = transform.position + Vector3.up * 1f;
    //    Vector3 downStartPoint = transform.position + Vector3.down * 1f;
    //    Vector3 clifEndPoint = transform.position + Vector3.left * 1f　* hAxis;
    //    Debug.DrawLine(upStartPoint, clifLeftEndPoint);
    //    Debug.DrawLine(downStartPoint, clifLeftEndPoint);
    //    Debug.DrawLine(upStartPoint, clifEndPoint);
    //    return Physics2D.Linecast(upStartPoint, clifEndPoint, blockLayer)
    //        || Physics2D.Linecast(downStartPoint, clifEndPoint, blockLayer);

    //}

    //private void CanCliff()
    //{
    //    // 座標を取得
    //    Vector3 pos = tf.position;
    //    //1ドット分
    //    pos.x += 1f;
    //    pos.y += 1f;

    //    tf.position = pos;  // 座標を設定
    //}

    //坂道の判定
    private void FindSlope()
    {
        RaycastHit2D hitInfoSlopeBlockLeft = Physics2D.Raycast(transform.position + Vector3.up * 0.1f, Vector2.left, 0.5f, blockLayer);
        RaycastHit2D hitInfoSlopeBlockRIght = Physics2D.Raycast(transform.position + Vector3.up * 0.1f, Vector2.right, 0.5f, blockLayer);

        if (hitInfoSlopeBlockRIght.collider != null 
            || hitInfoSlopeBlockRIght.collider != null)
        {
            this.gameObject.transform.Translate(0.000001f, 0f, 0f);
        }

        Debug.DrawRay(transform.position + Vector3.up * 0.1f, Vector2.left * 0.5f, Color.red);
        Debug.DrawRay(transform.position + Vector3.up * 0.1f, Vector2.right * 0.5f, Color.red);

    }
}