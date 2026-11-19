using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuertaCerrada : MonoBehaviour
{
    public float openDistance = 5f;
    public Vector3 openOffset = new Vector3(0f, -5.2f, 0f);
    public float openTime = 1.6f;

    public static readonly List<PuertaCerrada> activas = new List<PuertaCerrada>();

    private Transform player;
    private EstadisticasJugador playerStats;
    private bool opening;

    void OnEnable()
    {
        activas.Add(this);
    }

    void OnDisable()
    {
        activas.Remove(this);
    }

    void Update()
    {
        if (opening) return;

        if (player == null)
        {
            GameObject found = GameObject.FindWithTag("Player");
            if (found == null) return;
            player = found.transform;
            playerStats = found.GetComponent<EstadisticasJugador>();
        }

        if (Vector3.Distance(transform.position, player.position) > openDistance) return;

        if (playerStats != null && playerStats.hasUmbralKey)
            StartCoroutine(Open());
        else
            GestorHUD.Notify("La puerta está sellada. Buscá la llave del Umbral", 0.3f);
    }

    IEnumerator Open()
    {
        opening = true;
        EfectosJuego.Play(EfectosJuego.TipoSonido.Recompensa);
        EfectosJuego.Burst(transform.position, new Color(1f, 0.75f, 0.25f), 40, 0.25f);
        GestorHUD.Notify("La llave abre el camino al Guardián", 3f);

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
