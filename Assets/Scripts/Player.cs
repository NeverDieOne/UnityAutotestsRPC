using UnityEngine;

public class Player : MonoBehaviour {
    public float speed = 2500f;
    public float jumpForce = 8f;

    private Rigidbody2D _body;
    private CapsuleCollider2D _capsule;

    void Start() {
        _body = GetComponent<Rigidbody2D>();
        _capsule = GetComponent<CapsuleCollider2D>();
    }

    void Update() {
        float deltaX = Input.GetAxis("Horizontal");
        if (deltaX != 0) {
            Move(deltaX);
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            TryJump();
        }
    }

    private void Move(float x) {
        float deltaX = x * speed * Time.deltaTime;
        Vector2 movement = new(deltaX, _body.velocity.y);
        _body.velocity = movement;
    }

    private void TryJump() {
        Vector3 max = _capsule.bounds.max;
        Vector3 min = _capsule.bounds.min;
        Vector2 corner1 = new(max.x, min.y - .1f);
        Vector2 corner2 = new(min.x, min.y - .2f);

        Collider2D hit = Physics2D.OverlapArea(corner1, corner2);
        if (hit != null) {
            _body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }   
    }
}
