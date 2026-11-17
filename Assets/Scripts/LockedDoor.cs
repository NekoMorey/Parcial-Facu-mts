using System.Collections;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    public float openDistance = 5f;
    public Vector3 openOffset = new Vector3(0f, -5.2f, 0f);
    public float openTime = 1.6f;

    private Transform player;
    private PlayerStats playerStats;
    private bool opening;

    void Update()
    {
        if (opening) return;

        if (player == null)
        {
            GameObject found = GameObject.FindWithTag("Player");
            if (found == null) return;
            player = found.transform;
            playerStats = found.GetComponent<PlayerStats>();
        }

        if (Vector3.Distance(transform.position, player.position) > openDistance) return;

        if (playerStats != null && playerStats.hasUmbralKey)
            StartCoroutine(Open());
        else
            HUDManager.Notify("La puerta está sellada. Buscá la llave del Umbral", 0.3f);
    }

    IEnumerator Open()
    {
        opening = true;
        GameFeedback.Play(GameFeedback.Cue.Reward);
        GameFeedback.Burst(transform.position, new Color(1f, 0.75f, 0.25f), 40, 0.25f);
        HUDManager.Notify("La llave abre el camino al Guardián", 3f);

        Vector3 start = transform.position;
        Vector3 end = start + openOffset;
        float timer = 0f;

        while (timer < openTime)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, timer / openTime);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
