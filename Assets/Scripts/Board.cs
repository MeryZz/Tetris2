using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static int w = 10;
    public static int h = 20;
    public static GameObject[,] grid = new GameObject[w, h];

    private static Spawner spawner;

    // Rounds Vector2 so does not have decimal values
    // Used to force Integer coordinates (without decimals) when moving pieces
    public static Vector2 RoundVector2(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
    }

    // Returns true if pos (x,y) is inside the grid, false otherwise
    public static bool InsideBorder(Vector2 pos)
    {
        return pos.x >= 0 && pos.x < w && pos.y >= 0 && pos.y < h;
    }

    // Initializes the grid with blocks and disables them
    public static void InitializeGrid(GameObject blockPrefab)
    {
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                GameObject block = Instantiate(blockPrefab, new Vector3(x, y, 0), Quaternion.identity);
                block.SetActive(false);  // Desactivar los bloques al principio
                grid[x, y] = block;
            }
        }
    }

    // Deactivates a specific block at position (x, y)
    public static void DeactivateBlock(int x, int y)
    {
        if (grid[x, y] != null)
        {
            grid[x, y].SetActive(false);
        }
    }

    // Activates a block at position (x, y)
    public static void ActivateBlock(int x, int y)
    {
        if (grid[x, y] != null)
        {
            grid[x, y].SetActive(true);
        }
    }

    // Deletes all GameObjects in the row Y and set the row cells to null.
    public static void DeleteRow(int y)
    {
        for (int x = 0; x < w; x++)
        {
            if (grid[x, y] != null)
            {
                grid[x, y].SetActive(false);  // Desactivar en lugar de destruir
                grid[x, y] = null;
            }
        }
    }

    // Moves all gameobjects on row Y to row Y-1
    public static void DecreaseRow(int y)
    {
        for (int x = 0; x < w; x++)
        {
            if (grid[x, y] != null)
            {
                // Mover el objeto una fila hacia abajo
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;

                // Actualizar la posición del objeto
                grid[x, y - 1].transform.position += new Vector3(0, -1, 0);
            }
        }
    }

    // Decreases all rows above Y
    public static void DecreaseRowsAbove(int y)
    {
        for (int i = y; i < h; i++)
        {
            DecreaseRow(i);
        }
    }

    // Returns true if all cells in a row have a GameObject (are not null), false otherwise
    public static bool IsRowFull(int y)
    {
        for (int x = 0; x < w; ++x)
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }
        return true;
    }

    // Deletes full rows
    public static void DeleteFullRows()
    {
        for (int y = 0; y < h; ++y)
        {
            if (IsRowFull(y))
            {
                DeleteRow(y);
                DecreaseRowsAbove(y + 1);
                --y;  // Recheck the current row after moving others
            }
        }
    }

    // Initializes the static reference to the Spawner
    public static void InitializeSpawner()
    {
        spawner = Object.FindFirstObjectByType<Spawner>();
    }
}
