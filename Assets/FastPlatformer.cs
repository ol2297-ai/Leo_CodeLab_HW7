using UnityEngine;
using UnityEngine.SceneManagement;

public class FastPlatformer : MonoBehaviour
{
    public float speed = 7f;
    public float jumpForce = 11f;
    public GameObject WINN;

    private Rigidbody2D rb;
    private bool grounded;
    private bool won;
    private float cameraY;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cameraY = Camera.main.transform.position.y;
        WINN.SetActive(false);
    }

    void Update()
    {
        // 只有胜利后才能按 ESC 重新开始
        if (won)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Restart();

            return;
        }

        // 左右移动
        float x = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(x * speed, rb.linearVelocity.y);

        // 跳跃
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            grounded = false;
        }

        // 掉落死亡
        if (transform.position.y < -8f)
            Restart();
    }

    void LateUpdate()
    {
        if (won) return;

        // 摄像机跟随
        Vector3 pos = Camera.main.transform.position;
        pos.x = transform.position.x;
        pos.y = cameraY;
        Camera.main.transform.position = pos;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckGround(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        CheckGround(collision);
    }

    void CheckGround(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
                grounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        grounded = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (won) return;

        // 碰到危险物，立即重置
        if (other.name.StartsWith("Hazard"))
        {
            Restart();
            return;
        }

        // 到达终点
        if (other.name.StartsWith("Goal"))
        {
            won = true;
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
            WINN.SetActive(true);
        }
    }

    void Restart()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
