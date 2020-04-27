using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] LayerMask blockLayer;
    [SerializeField] LayerMask ladderLayer;

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

    public float speed;
    private float hAxis;
    private float vAxis;

    private bool control;
    private bool isDead = false;
    private bool canClimb;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        // スプライトレンダラーのコンポーネントを取得する
        this.sr = GetComponent<SpriteRenderer>();
        canClimb = false;
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
                                                 // 移動する向きを作成する
            rb2D.velocity = new Vector2(hAxis * speed, vAxis * speed);

            if (canClimb)
            {
                vAxis = Input.GetAxis("Vertical"); //方向キーの取得
            }
        }

        HAxis();
    }

    //方向動きの取得
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

        string layerName = LayerMask.LayerToName(collision.gameObject.layer);

        if (layerName == "Ladder")
        {
            canClimb = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        string layerName = LayerMask.LayerToName(collision.gameObject.layer);

        if (layerName == "Ladder")
        {
            canClimb = false;
        }
    }

    //死亡判定
    void PlayerDeath()
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

    //はしごや鎖に登る
    private void CanClimb()
    {
        if (canClimb)
        {

        }
    }
}