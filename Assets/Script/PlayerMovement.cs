using UnityEngine;

public class PlayerMovement : Livingentity
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private Vector3 movement;
    private Animator playerAnimator;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal"); //A D 좌우
        float v = Input.GetAxis("Vertical"); //W S 상하

        movement = new Vector3(h, 0f , v); //Y축은 고정 필요

        bool isMoving = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;
        playerAnimator.SetBool("Moving", isMoving);

        RotateToMouse();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void RotateToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 lookPoint = hit.point;

            // Y값 고정 (기울어지는 거 방지)
            lookPoint.y = transform.position.y;

            transform.LookAt(lookPoint);
        }
    }

}
