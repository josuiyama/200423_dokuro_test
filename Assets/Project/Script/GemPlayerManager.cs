using UnityEngine;
using System.Collections;

public class GemPlayerManager : MonoBehaviour
{
    [SerializeField]
    private ChangeChara changeChara;

    public enum DIRECTION_TYPE
    {
        STOP,
        RIGHT,
        LEFT,
    }
    DIRECTION_TYPE direction = DIRECTION_TYPE.STOP;

    // スプライトレンダラーコンポーネントを入れる
    SpriteRenderer sr;
    private Rigidbody2D rigidbody2D;

    public float speed = 100.0F;    // 移動早さ
    float h;
    float v;

    private bool control;
    bool CanBite;
    bool IsBite;

    // Use this for initialization
    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        // スプライトレンダラーのコンポーネントを取得する
        this.sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsBite)
        {
            if (Input.GetKeyDown("space") && CanBite)
            {
                DoBite();
                //噛み付いたときにミサ＝1にする
                changeChara.ChangeCharacter(1);
            }
        }
        else
        {
            if (Input.GetKeyDown("space"))
            {
                ReleaseBite();
            }
        }
        //噛み付いてる時に方向を変えないようにする
        if (!IsBite)
        {
            // 矢印キーの入力情報を取得
            if (control)
            {
                h = Input.GetAxis("Horizontal");
                v = Input.GetAxis("Vertical");
            }

            if (h == 0)
            {
                //止まっている
                direction = DIRECTION_TYPE.STOP;
            }
            else if (h > 0)
            {
                //右へ
                direction = DIRECTION_TYPE.RIGHT;
            }
            else if (h < 0)
            {
                //左へ
                direction = DIRECTION_TYPE.LEFT;
            }

            // 移動する向きを作成する
            Vector2 directionVector = new Vector2(h, v).normalized;

            // 移動する向きとスピードを代入 
            rigidbody2D.velocity = directionVector * speed;
        }
    }

    private void FixedUpdate()
    {
        switch (direction)
        {
            case DIRECTION_TYPE.STOP:
                break;
            case DIRECTION_TYPE.RIGHT:
                //transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                sr.flipX = false;
                break;
            case DIRECTION_TYPE.LEFT:
                sr.flipX = true;
                //transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);
                break;
        }
    }

    //噛みつきをする
    private void DoBite()
    {
        //動く噛みつき場所とかぶらさがり場所に対して固定してたらまずいのでは？？
        //どうやって離れさせる？？？
        //噛み付いた部分とターゲットレイヤーのオブジェクトを起点にヒンジジョイントを追加するとか……

        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        IsBite = true;
    }

    //噛みつき解除
    private void ReleaseBite()
    {
        rigidbody2D.constraints = RigidbodyConstraints2D.None;
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        IsBite = false;
    }

    public void ChangeControl(bool controlFlag)
    {
        h = 0;
        v = 0;
        control = controlFlag;
    }

    //噛みつける場所判定
    public void OnTriggerEnter2D(Collider2D collision)
    {
        string layerName = LayerMask.LayerToName(collision.gameObject.layer);

        if (layerName == "Bitten")
        {
            CanBite = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        string layerName = LayerMask.LayerToName(collision.gameObject.layer);

        if (layerName == "Bitten")
        {
            CanBite = false;
        }
    }
}