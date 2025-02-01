using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Piezas disponibles
    public GameObject[] pieces;

    // Pool de piezas
    private List<GameObject> piecesPool = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Instanciar todas las piezas en el pool y desactivarlas
        foreach (GameObject piece in pieces)
        {
            GameObject newPiece = Instantiate(piece, transform.position, Quaternion.identity);
            newPiece.SetActive(false);  // Desactivar pieza para que no se vea inicialmente
            piecesPool.Add(newPiece);
        }

        // Llamamos a SpawnNext para la primera pieza
        SpawnNext();
    }

    // Función para activar la siguiente pieza del pool
    public void SpawnNext()
    {
        // Elegir una pieza aleatoria del pool
        int i = Random.Range(0, piecesPool.Count);

        // Buscar una pieza inactiva
        GameObject piece = piecesPool[i];

        // Asegurarse de que la pieza seleccionada esté inactiva
        while (piece.activeInHierarchy)
        {
            i = Random.Range(0, piecesPool.Count);
            piece = piecesPool[i];
        }

        // Activar la pieza y moverla a la posición inicial
        piece.transform.position = new Vector3(5, 10, 0);
        piece.SetActive(true);  // Activar la pieza seleccionada
        piece.GetComponent<Piece>().enabled = true;  // Habilitar su script de movimiento
    }
}
