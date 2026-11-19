using UnityEngine;
using UnityEngine.UI;

public class CursorPantalla : MonoBehaviour
{
    RectTransform puntero;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Crear()
    {
        var objeto = new GameObject("CursorPantalla");
        DontDestroyOnLoad(objeto);
        objeto.AddComponent<CursorPantalla>();
    }

    void Awake()
    {
        var lienzo = gameObject.AddComponent<Canvas>();
        lienzo.renderMode = RenderMode.ScreenSpaceOverlay;
        lienzo.sortingOrder = 32000;

        var borde = NuevaImagen("Borde", transform, Color.black, 20f);
        borde.pivot = new Vector2(0f, 1f);
        puntero = borde;

        var relleno = NuevaImagen("Relleno", borde, Color.white, 14f);
        relleno.pivot = new Vector2(0.5f, 0.5f);
        relleno.anchorMin = relleno.anchorMax = new Vector2(0.5f, 0.5f);
        relleno.anchoredPosition = Vector2.zero;
    }

    RectTransform NuevaImagen(string nombre, Transform padre, Color color, float lado)
    {
        var imagen = new GameObject(nombre).AddComponent<Image>();
        imagen.transform.SetParent(padre, false);
        imagen.color = color;
        imagen.raycastTarget = false;
        imagen.rectTransform.sizeDelta = new Vector2(lado, lado);
        return imagen.rectTransform;
    }

    void Update()
    {
        bool libre = Cursor.lockState != CursorLockMode.Locked;
        puntero.gameObject.SetActive(libre);
        if (!libre) return;
        Cursor.visible = false;
        puntero.position = Input.mousePosition;
    }
}
