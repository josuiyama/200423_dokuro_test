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


    // スプライトレンダラーコンポーネントを入れる
    SpriteRenderer sr;
    Rigidbody2D rigidbody2D;

    public float speed = 100.0F;
    private float hAxis;

    private bool control;
    bool isDead = false;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        // スプライトレンダラーのコンポーネントを取得する
        this.sr = GetComponent<SpriteRenderer>();
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
                speed = 0;
                break;
            case DIRECTION_TYPE.RIGHT:
                speed = 3;
                sr.flipX = false;
                break;
            case DIRECTION_TYPE.LEFT:
                speed = -3;
                sr.flipX = true;
                break;
        }
        rigidbody2D.velocity = new Vector2(speed, rigidbody2D.velocity.y);
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
            collision.gameObject.GetComponent<ItemManeger>().GetItem();
        }

    }
    void PlayerDeath()
    {
        isDead = true;
        rigidbody2D.velocity = new Vector2(0, 0);
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
