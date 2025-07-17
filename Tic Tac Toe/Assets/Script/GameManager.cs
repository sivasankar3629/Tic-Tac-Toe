using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Button[,] buttons = new Button[3, 3];
    public TMP_Text[,] texts = new TMP_Text[3, 3];
    public TMP_Text statusText;
    public Button restartButton;

    private char[,] board = new char[3, 3];
    private bool isPlayerTurn = true;
    private bool gameOver = false;

    void Start()
    {
        SetupBoard();
    }

    void SetupBoard()
    {
        int i = 0;
        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++, i++)
            {
                GameObject cell = GameObject.Find($"Cell_{r}_{c}");
                buttons[r, c] = cell.GetComponent<Button>();
                texts[r, c] = cell.GetComponentInChildren<TMP_Text>();
                int row = r, col = c;

                buttons[r, c].onClick.RemoveAllListeners(); 
                buttons[r, c].onClick.AddListener(() => OnCellClick(row, col));

                board[r, c] = ' ';
                texts[r, c].text = "";
                buttons[r, c].interactable = true; 
            }
        }

        statusText.text = "Your Turn (X)";
        gameOver = false;
        isPlayerTurn = true;
    }


    void OnCellClick(int row, int col)
    {
        if (!isPlayerTurn || board[row, col] != ' ' || gameOver) return;

        board[row, col] = 'X';
        texts[row, col].text = "X";
        buttons[row, col].interactable = false;

        if (CheckGameOver()) return;

        isPlayerTurn = false;
        statusText.text = "AI Thinking...";
        Invoke(nameof(PlayAIMove), 0.5f); // Slight delay for realism
    }

    void PlayAIMove()
    {
        (int row, int col) = FindBestMove(board);
        board[row, col] = 'O';
        texts[row, col].text = "O";
        buttons[row, col].interactable = false;

        if (CheckGameOver()) return;

        isPlayerTurn = true;
        statusText.text = "Your Turn (X)";
    }

    bool CheckGameOver()
    {
        if (IsWinner(board, 'X'))
        {
            statusText.text = "You Win!";
            gameOver = true;
            return true;
        }
        else if (IsWinner(board, 'O'))
        {
            statusText.text = "AI Wins!";
            gameOver = true;
            return true;
        }
        else if (IsDraw(board))
        {
            statusText.text = "Draw!";
            gameOver = true;
            return true;
        }
        return false;
    }

    public void RestartGame()
    {
        SetupBoard();
    }

    (int, int) FindBestMove(char[,] board)
    {
        int bestVal = int.MinValue;
        int bestRow = -1, bestCol = -1;

        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                if (board[r, c] == ' ')
                {
                    board[r, c] = 'O';
                    int moveVal = Minimax(board, false);
                    board[r, c] = ' ';

                    if (moveVal > bestVal)
                    {
                        bestVal = moveVal;
                        bestRow = r;
                        bestCol = c;
                    }
                }
            }
        }
        return (bestRow, bestCol);
    }

    int Minimax(char[,] board, bool isMax)
    {
        int score = Evaluate(board);
        if (score == 10 || score == -10 || IsDraw(board)) return score;

        if (isMax)
        {
            int best = int.MinValue;
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    if (board[r, c] == ' ')
                    {
                        board[r, c] = 'O';
                        best = Mathf.Max(best, Minimax(board, false));
                        board[r, c] = ' ';
                    }
                }
            }
            return best;
        }
        else
        {
            int best = int.MaxValue;
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    if (board[r, c] == ' ')
                    {
                        board[r, c] = 'X';
                        best = Mathf.Min(best, Minimax(board, true));
                        board[r, c] = ' ';
                    }
                }
            }
            return best;
        }
    }

    int Evaluate(char[,] b)
    {
        if (IsWinner(b, 'O')) return 10;
        if (IsWinner(b, 'X')) return -10;
        return 0;
    }

    bool IsWinner(char[,] b, char p)
    {
        for (int i = 0; i < 3; i++)
        {
            if (b[i, 0] == p && b[i, 1] == p && b[i, 2] == p) return true;
            if (b[0, i] == p && b[1, i] == p && b[2, i] == p) return true;
        }
        if (b[0, 0] == p && b[1, 1] == p && b[2, 2] == p) return true;
        if (b[0, 2] == p && b[1, 1] == p && b[2, 0] == p) return true;
        return false;
    }

    bool IsDraw(char[,] b)
    {
        foreach (char c in b)
            if (c == ' ') return false;
        return true;
    }
}
