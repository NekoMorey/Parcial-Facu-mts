using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private PlayerController controller;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (animator == null) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        float speed = Mathf.Clamp01(new Vector2(moveX, moveZ).magnitude);

        animator.SetFloat("Speed", speed, 0.08f, Time.deltaTime);
        animator.SetBool("Grounded", controller == null || controller.IsGrounded);
    }

    public void Dodge()
    {
        if (animator != null) animator.SetTrigger("Dodge");
    }

    public void Throw()
    {
        if (animator != null) animator.SetTrigger("Throw");
    }
}
