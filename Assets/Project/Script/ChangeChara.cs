using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChangeChara : MonoBehaviour
{

    //　現在どのキャラクターを操作しているか
    public int nowChara;
    private int nextChara;

    //　操作可能なゲームキャラクター
    //[SerializeField]
    //private GameObject[] charaList = new GameObject[2];
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject gem;

    private void Start()
    {
        //　最初の操作キャラクターを0番目のキャラクターにする
        //charaList[0].GetComponent<PlayerManager>().ChangeControl(true);
        player.GetComponent<PlayerManager>().ChangeControl(true);
    }

    private void Update()
    {
        Debug.Log(nowChara);

        //　Shiftキーが押されたら操作キャラクターを次のキャラクターに変更する
        if (Input.GetKeyDown("left shift"))
        {
            nowChara = ChangeCharacter(nowChara);
        }
    }

    //　操作キャラクター変更メソッド
    public int ChangeCharacter(int nowChara)
    {
        //　次のキャラクターの番号を設定
        switch (nowChara)
        {
            case 0:
                nextChara = 1;
                //　現在操作しているキャラクターを動かなくする
                player.GetComponent<PlayerManager>().ChangeControl(false);
                player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                //　次のキャラクターを動かせるようにする
                gem.GetComponent<GemPlayerManager>().ChangeControl(true);
                gem.GetComponent<Rigidbody2D>().gravityScale = 0;
                break;
            case 1:
                nextChara = 0;
                //　次のキャラクターを動かせるようにする
                player.GetComponent<PlayerManager>().ChangeControl(true);
                player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                //　現在操作しているキャラクターを動かなくする
                gem.GetComponent<GemPlayerManager>().ChangeControl(false);
                gem.GetComponent<Rigidbody2D>().gravityScale = 10;
                break;
            default:
                Debug.Log("バグった");
                break;
        }

        ////　現在操作しているキャラクターを動かなくする
        //charaList[tempNowChara].GetComponent<PlayerManager>().ChangeControl(false);
        ////　次のキャラクターの番号を設定
        //var nextChara = tempNowChara + 1;
        //if (nextChara >= charaList.Length)
        //{
        //    nextChara = 0;
        //}
        ////　次のキャラクターを動かせるようにする
        //charaList[nextChara].GetComponent<PlayerManager>().ChangeControl(true);
        ////　現在のキャラクター番号を保持する
        //nowChara = nextChara;
        return nextChara;
    }

}