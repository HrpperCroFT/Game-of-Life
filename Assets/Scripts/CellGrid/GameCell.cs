using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCell : MonoBehaviour {
    public enum CellState {
        Alive,
        Dead
    };

    private CellState m_CellState = CellState.Alive;

    public int aliveNeighboursCount = 0;

    public CellState GetState() {
        return m_CellState;
    }

    public void ChangeState() {
        if (m_CellState == CellState.Dead) {
            m_CellState = CellState.Alive;
            GetComponent<SpriteRenderer>().enabled = true;
        } else {
            m_CellState = CellState.Dead;
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void UpdateState() {
        if (m_CellState == CellState.Alive) {
            if (aliveNeighboursCount != 2 && aliveNeighboursCount != 3) {
                ChangeState();
            }
        } else {
            if (aliveNeighboursCount >= 3) {
                ChangeState();
            }
        }
    }
}
