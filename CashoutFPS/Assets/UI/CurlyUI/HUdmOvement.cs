using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUdmOvement : MonoBehaviour
{
    public float speed = 5.0f; // Nesne hareket h�z�
    public float boundaryX = 5.0f; // Sa�a ve sola s�n�r
    public float boundaryY = 5.0f; // Yukar� ve a�a�� s�n�r

    void Update()
    {
        // Mouse hareketlerini al
        float moveHorizontal = Input.GetAxis("Mouse X");
        float moveVertical = Input.GetAxis("Mouse Y");

        // Nesnenin yeni konumunu hesapla
        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0.0f) * speed * Time.deltaTime;

        // Nesnenin s�n�rlar�n� kontrol et
        Vector3 newPosition = transform.position + movement;
        newPosition.x = Mathf.Clamp(newPosition.x, -boundaryX, boundaryX);
        newPosition.y = Mathf.Clamp(newPosition.y, -boundaryY, boundaryY);

        // Yeni konumu uygula
        transform.position = newPosition;
    }
}
