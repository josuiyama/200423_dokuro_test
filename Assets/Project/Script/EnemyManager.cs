using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] LayerMask blockLayer;
    [SerializeField] GameObject deathEffect;

    public enum DIRECTION_TYPE
    {
        STOP,
        RIGHT,
        LEFT,
    }

    DIRECTION_TYPE enemyDirection = DIRECTION_TYPE.STOP;

    private SpriteRenderer enemySr;
    private Rigidbody2D enemyRb2D;

    public float enemySpeed;

    private void Start()
    {
        enemyRb2D = GetComponent<Rigidbody2D>();
        this.enemySr = GetComponent<SpriteRenderer>();

        enemyDirection = DIRECTION_TYPE.RIGHT;
    }

    private void Update()
    {
        EnemyHAxis();

        if (!EnemyIsGround())
        {
            EnemyChangeDirection();
        }
    }
    //方向とスピード設定
    private void EnemyHAxis()
    {
        //スピードの正負を管理する用
        int sign = 1;

        switch (enemyDirection)
        {
            case DIRECTION_TYPE.STOP:
                break;
            case DIRECTION_TYPE.RIGHT:
                sign = 1;
                transform.localScale = new Vector3(1, 1, 1);
                break;
            case DIRECTION_TYPE.LEFT:
                sign = -1;
                transform.localScale = new Vector3(-1, 1, 1);
                break;
        }
        enemyRb2D.velocity = new Vector2(enemySpeed *sign, enemyRb2D.velocity.y);
    }
    //接地判定
    private bool EnemyIsGround()
    {
        Vector3 startVec = transform.position + transform.right * 0.5f * transform.localScale.x;
        Vector3 endVec = startVec - transform.up * 0.5f;
        Debug.DrawLine(startVec, endVec);
        return Physics2D.Linecast(startVec, endVec, blockLayer);
    }
    //方向切替
    private void EnemyChangeDirection()
    {
        if (enemyDirection == DIRECTION_TYPE.RIGHT)
        {
            enemyDirection = DIRECTION_TYPE.LEFT;
        }
        else if (enemyDirection == DIRECTION_TYPE.LEFT)
        {
            enemyDirection = DIRECTION_TYPE.RIGHT;
        }
    }
    //死亡判定
    public void DestroyEnemy()
    {
        Instantiate(deathEffect, this.transform.position, this.transform.rotation);
        Destroy(this.gameObject);
    }

}
