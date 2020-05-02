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
    private SpriteRenderer sr;
    private Rigidbody2D rb2D;

    public float speed;    // 移動早さ
    private float h;
    private float v;

    private bool control;
    private bool CanBite;
    public bool IsBite;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        // スプライトレンダラーのコンポーネントを取得する
        this.sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (control)
        {
            GetHV();
            H();
            BiteAction();
        }
    }

    // 矢印キーの入力情報を取得
    //横縦移動スピードの取得
    private void GetHV()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        rb2D.velocity = new Vector2(h * speed, v * speed);
    }

    //横動き方向の取得
    //横方向変更でイラスト反転
    private void H()
    {
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

        //噛み付いているときは反転できない
        if (!IsBite)
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
    }

    //噛みつきアクション
    private void BiteAction()
    {
        //ドクロ操作のときじゃないと噛みつき＆噛みつき解除できないようにする
        if (changeChara.nowChara == 1)
        {
            //IsBiteがフォルスのとき発動
            if (!IsBite)
            {
                //CanBiteのときスペースで噛みつける
                //操作で噛みつけない時はかな入力になってないか気をつけろ！

                if (Input.GetKeyDown("space") && CanBite)
                {
                    //動く噛みつき場所とかぶらさがり場所に対して固定してたらまずいのでは？？
                    //どうやって離れさせる？？？
                    //噛み付いた部分とターゲットレイヤーのオブジェクトを起点にヒンジジョイントを追加するとか……

                    rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
                    IsBite = true;
                    //噛み付いたときにミサ＝1にする
                    //changeChara.ChangeCharacter(1);
                }
            }

            //IsBiteがフォルスのときは発動しない
            //IsBiteがトゥルーのとき発動する
            else
            {
                //スペースで噛みつき離す
                if (Input.GetKeyDown("space"))
                {
                    rb2D.constraints = RigidbodyConstraints2D.None;
                    rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
                    IsBite = false;
                }
            }
        }
    }

    //ChangeControlが有効になった時
    public void ChangeControl(bool controlFlag)
    {
        h = 0;
        v = 0;
        control = controlFlag;
    }

    //Bittenレイヤーに接触している時は噛みつける（=CanBiteがトゥルーに）
    public void OnTriggerEnter2D(Collider2D collision)
    {
        string layerName = LayerMask.LayerToName(collision.gameObject.layer);

        if (layerName == "Bitten")
        {
            CanBite = true;
        }
    }
    //Bittenレイヤーに接触していない時は噛みつけない（=CanBiteがファルスに）
    public void OnTriggerExit2D(Collider2D collision)
    {
        string layerName = LayerMask.LayerToName(collision.gameObject.layer);

        if (layerName == "Bitten")
        {
            CanBite = false;
        }
    }
}