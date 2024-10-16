using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MainGameController : MonoBehaviour {
    [SerializeField] private int m_Width;
    [SerializeField] private int m_Height;
    [SerializeField] private Scrollbar m_UpdatesPerSecondScroll;
    [SerializeField] private Scrollbar m_RandomizationCoefficientScroll;

    private GameCell[,] m_Cells;

    private bool m_Process = false;

    private float m_Timer = 0;

    [SerializeField] private Button m_StartStopButton;
    [SerializeField] private Button m_ClearAllButton;
    [SerializeField] private Button m_RandomizeButton;

    private float m_UpdatesPerSecond;
    private float m_RandomizationCoefficient;

    private Vector3 shift = new Vector3(0f, -4f, 0f);

    private const float MAX_UPDATES_PER_SECOND = 30;

    // Start is called before the first frame update
    void Start() {
        m_Cells = new GameCell[m_Width, m_Height];

        var cellPrefab = Resources.Load("Prefabs/CellPrefab", typeof(GameCell));

        for (int i = 0; i < m_Width; i++) {
            for (int j = 0; j < m_Height; j++) {
                var position = new Vector3(i, j, 0);
                GameCell cell = Instantiate(cellPrefab, position + shift, Quaternion.identity) as GameCell;
                m_Cells[i, j] = cell;
                m_Cells[i, j].ChangeState();
            }
        }

        m_StartStopButton.onClick.AddListener(StartOrStop);
        m_ClearAllButton.onClick.AddListener(ClearAll);
        m_RandomizeButton.onClick.AddListener(Randomize);

        m_UpdatesPerSecondScroll.onValueChanged.AddListener(UpdatesPerSecondChange);
        m_RandomizationCoefficientScroll.onValueChanged.AddListener(RandomizationCoefficientChanged);
    }

    private void UpdatesPerSecondChange(float value) {
        m_UpdatesPerSecond = value * MAX_UPDATES_PER_SECOND;
    }

    private void RandomizationCoefficientChanged(float value) {
        m_RandomizationCoefficient = value;
    }

    private bool CheckIsOneOfCells(int x, int y) {
        return x >= 0 && y >= 0 && x < m_Width && y < m_Height;
    }

    private int CountAliveNeighbours(int i, int j) {
        int result = 0;
        for (int x = i - 1; x <= i + 1; ++x) {
            for (int y = j - 1; y <= j + 1; ++y) {
                if (x == i && y == j) continue;
                if (CheckIsOneOfCells(x, y) && m_Cells[x, y].GetState() == GameCell.CellState.Alive) {
                    ++result;
                } 
            }
        }
        return result;
    }

    private void MakeStatesValid() {
        for (int i = 0; i < m_Width; ++i) {
            for (int j = 0; j < m_Height; ++j) {
                m_Cells[i, j].aliveNeighboursCount = CountAliveNeighbours(i, j);
            }
        }
        for (int i = 0; i < m_Width; ++i) {
            for (int j = 0; j < m_Height; ++j) {
                m_Cells[i, j].UpdateState();
            }
        }
    }

    private void UserInput() {
        if (Input.GetMouseButtonDown(0)) {
            var mousePosition = Input.mousePosition;
            var worldPosition = Camera.main.ScreenToWorldPoint(mousePosition) - shift;

            int x = (int)(worldPosition.x + 0.5f);
            int y = (int)(worldPosition.y + 0.5f);

            if (CheckIsOneOfCells(x, y)) {
                m_Cells[x, y].ChangeState();
            }
        }
    }

    void Update() {
        if (!m_Process) {
            UserInput();
            m_Timer = 0;
            return;
        }
        if (m_Timer >= 1 / m_UpdatesPerSecond) {
            m_Timer = 0;
            MakeStatesValid();
        } else {
            m_Timer += Time.deltaTime;
        }
    }

    private void StartOrStop() {
        var buttonText = m_StartStopButton.transform.Find("ButtonText").GetComponent<TextMeshProUGUI>();

        if (m_Process) {
            buttonText.text = "Start";
        } else {
            buttonText.text = "Stop";
        }
        m_Process = !m_Process;
    }

    private void ClearAll() {
        if (m_Process) {
            return;
        }

        for (int i = 0; i < m_Width; ++i) {
            for (int j = 0; j < m_Height; ++j) {
                if (m_Cells[i, j].GetState() == GameCell.CellState.Alive) {
                    m_Cells[i, j].ChangeState();
                }
            }
        }
    }

    private void Randomize() {
        ClearAll();
        for (int i = 0; i < m_Width; i++) {
            for (int j = 0; j < m_Height; j++) {
                if (UnityEngine.Random.Range(1, 101) <= (int)(m_RandomizationCoefficient * 100)) {
                    m_Cells[i, j].ChangeState();
                }
            }
        }
    }
}
