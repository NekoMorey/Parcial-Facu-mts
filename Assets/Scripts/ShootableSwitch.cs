using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ShootableSwitch : MonoBehaviour
{
    public Transform movingPiece;
    public Vector3 moveOffset = new Vector3(0f, -4.5f, 0f);
    public float moveTime = 1.2f;
    public string message = "Algo se movió cerca";
    public UnityEvent onActivated = new UnityEvent();

    private bool activated;

    private void OnCollisionEnter(Collision collision)
    {
        if (activated || collision.gameObject.GetComponent<Rock>() == null) return;

        activated = true;

        Renderer seal = GetComponent<Renderer>();
        if (seal != null) seal.material.color = new Color(0.2f, 0.9f, 0.5f);

        GameFeedback.Play(GameFeedback.Cue.Reward);
        GameFeedback.Burst(transform.position, new Color(0.2f, 0.9f, 0.8f), 24, 0.18f);
        HUDManager.Notify(message, 3f);
        onActivated.Invoke();

        if (movingPiece != null) StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        Vector3 start = movingPiece.position;
        Vector3 end = start + moveOffset;
        float timer = 0f;

        while (timer < moveTime)
        {
            timer += Time.deltaTime;
            movingPiece.position = Vector3.Lerp(start, end, timer / moveTime);
            yield return null;
        }

        movingPiece.gameObject.SetActive(false);
    }
}
