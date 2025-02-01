using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private float lastFall = 0f;
    private bool isActive = true;

    void Start()
    {
        // Verificar si la posición inicial es válida, si no, es Game Over
        if (!IsValidBoard())
        {
            Debug.Log("GAME OVER");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Si la pieza no está activa, no hacer nada
        if (!isActive)
            return;

        // Movimiento a la izquierda
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-1, 0, 0);
            if (IsValidBoard())
                UpdateBoard();
            else
                transform.position += new Vector3(1, 0, 0);
        }

        // Movimiento a la derecha
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position += new Vector3(1, 0, 0);
            if (IsValidBoard())
                UpdateBoard();
            else
                transform.position += new Vector3(-1, 0, 0);
        }

        // Rotación (tecla UpArrow)
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.rotation *= Quaternion.Euler(0, 0, 90);
            if (IsValidBoard())
                UpdateBoard();
            else
                transform.rotation *= Quaternion.Euler(0, 0, -90);  // Revertir la rotación
        }

        // Movimiento hacia abajo (tecla DownArrow)
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += Vector3.down;
            if (IsValidBoard())
                UpdateBoard();
            else
                transform.position += Vector3.up;
        }

        // Caída automática de la pieza
        if (Time.time - lastFall >= 1)
        {
            transform.position += Vector3.down;
            if (IsValidBoard())
            {
                UpdateBoard();
            }
            else
            {
                transform.position += Vector3.up;
                UpdateBoard();
                isActive = false;

                // Activar los bloques donde la pieza se ha detenido
                foreach (Transform child in transform)
                {
                    Vector2 v = Board.RoundVector2(child.position);
                    Board.ActivateBlock((int)v.x, (int)v.y);
                }

                // Desactivar la pieza y moverla a una posición no visible
                transform.position = new Vector3(-100, -100, 0);  // Colocar la pieza en una posición no visible
                gameObject.SetActive(false);  // Desactivar la pieza para reutilizarla

                // Activar la siguiente pieza
                Object.FindFirstObjectByType<Spawner>().SpawnNext();

                // Eliminar filas completas
                Board.DeleteFullRows();
                enabled = false;  // Deshabilitar el script de la pieza para evitar que se mueva más
            }

            lastFall = Time.time;
        }
    }

    // Actualiza el tablero con la posición actual de la pieza
    void UpdateBoard()
    {
        // Primero, recorrer el tablero y hacer null las posiciones actuales de la pieza
        for (int y = 0; y < Board.h; y++)
        {
            for (int x = 0; x < Board.w; ++x)
            {
                if (Board.grid[x, y] != null && Board.grid[x, y].transform.parent == transform)
                {
                    Board.grid[x, y] = null;
                }
            }
        }

        // Luego, recorrer los bloques de la pieza actual y agregarla al tablero
        foreach (Transform child in transform)
        {
            Vector2 v = Board.RoundVector2(child.position);
            Board.grid[(int)v.x, (int)v.y] = child.gameObject;
        }
    }

    // Verifica si la posición actual de la pieza es válida dentro del tablero
    bool IsValidBoard()
    {
        foreach (Transform child in transform)
        {
            Vector2 v = Board.RoundVector2(child.position);

            // No está dentro de los límites del tablero?
            if (!Board.InsideBorder(v))
                return false;

            // Bloque en la celda de la cuadrícula (y no es parte del mismo grupo)?
            if (Board.grid[(int)v.x, (int)v.y] != null &&
                Board.grid[(int)v.x, (int)v.y].transform.parent != transform)
                return false;
        }
        return true;
    }
}
