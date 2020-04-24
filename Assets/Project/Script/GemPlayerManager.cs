using UnityEngine;
using System.Collections;

public class GemPlayerManager : MonoBehaviour
{
    public float speed = 100.0F;    // 移動早さ
    private bool control;
    float h;
    float v;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        // 矢印キーの入力情報を取得
        if (control)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
        }

        // 移動する向きを作成する
        Vector2 direction = new Vector2(h, v).normalized;

        // 移動する向きとスピードを代入 
        GetComponent<Rigidbody2D>().velocity = direction * speed;
    }
    public void ChangeControl(bool controlFlag)
    {
        h = 0;
        v = 0;
        control = controlFlag;
    }
}