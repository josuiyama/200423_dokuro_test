using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] GameObject deathEffect;
    [SerializeField] LayerMask blockLayer;
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
    private float enemyHAxis;

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
        Debug.Log(EnemyIsGround() + "EnemyIsGround");
    }

    private void EnemyHAxis()
    {
        switch (enemyDirection)
        {
            case DIRECTION_TYPE.STOP:
                break;
            case DIRECTION_TYPE.RIGHT:
                //enemySr.flipX = false;
                break;
            case DIRECTION_TYPE.LEFT:
                //enemySr.flipX = true;
                break;
        }
        enemyRb2D.velocity = new Vector2(enemySpeed, enemyRb2D.velocity.y);
    }

    //接地判定
    private bool EnemyIsGround()
    {
        Vector3 startVec = transform.position + transform.right * 0.5f * transform.localScale.x;
        Vector3 endVec = startVec - transform.up * 0.5f;
        Debug.DrawLine(startVec, endVec);
        return Physics2D.Linecast(startVec, endVec, blockLayer);
    }

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
    public void DestroyEnemy()
    {
        Instantiate(deathEffect, this.transform.position, this.transform.rotation);
        Destroy(this.gameObject);
    }

}
