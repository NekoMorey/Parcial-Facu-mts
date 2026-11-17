using UnityEngine;

public class AnimacionJugador : MonoBehaviour
{
    private Animator animator;
    private ControlJugador controller;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<ControlJugador>();
    }

    void Update()
    {
        if (animator == null) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        float speed = Mathf.Clamp01(new Vector2(moveX, moveZ).magnitude);

        animator.SetFloat("Velocidad", speed, 0.08f, Time.deltaTime);
        animator.SetBool("EnSuelo", controller == null || controller.IsGrounded);
    }

    public void Dodge()
    {
        if (animator != null) animator.SetTrigger("Esquive");
    }

    public void Throw()
    {
        ArmasJugador weapons = GetComponent<ArmasJugador>();
        if (weapons != null) weapons.Aim();
        if (animator != null) animator.SetTrigger("Disparo");
    }

    public void Slash()
    {
        if (animator != null) animator.SetTrigger("Tajo");
    }
}
