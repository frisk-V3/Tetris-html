using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

public class Tetris : Form {
    const int CanvasWidth = 10, CanvasHeight = 20, TileSize = 30;
    int[,] grid = new int[CanvasWidth, CanvasHeight];
    int score = 0;
    Timer timer = new Timer();
    Point currentPos;
    int[,] currentPiece;
    Random rand = new Random();

    // テトリミノの定義
    int[][,] shapes = {
        new int[,] { {1,1,1,1} }, // I
        new int[,] { {1,1}, {1,1} }, // O
        new int[,] { {0,1,0}, {1,1,1} }, // T
        new int[,] { {0,1,1}, {1,1,0} }, // S
        new int[,] { {1,1,0}, {0,1,1} }  // Z
    };

    public Tetris() {
        this.Text = "Tetris";
        this.ClientSize = new Size(CanvasWidth * TileSize, CanvasHeight * TileSize + 30);
        this.DoubleBuffered = true;
        this.KeyDown += OnKeyDown;
        
        timer.Interval = 500;
        timer.Tick += (s, e) => { MovePiece(0, 1); this.Invalidate(); };
        SpawnPiece();
        timer.Start();
    }

    void SpawnPiece() {
        currentPiece = shapes[rand.Next(shapes.Length)];
        currentPos = new Point(CanvasWidth / 2 - 1, 0);
        if (CheckCollision(0, 0)) {
            timer.Stop();
            MessageBox.Show("Game Over! Score: " + score);
            Application.Restart();
        }
    }

    bool CheckCollision(int dx, int dy, int[,] piece = null) {
        int[,] p = piece ?? currentPiece;
        for (int y = 0; y < p.GetLength(0); y++)
            for (int x = 0; x < p.GetLength(1); x++)
                if (p[y, x] != 0) {
                    int nx = currentPos.X + x + dx, ny = currentPos.Y + y + dy;
                    if (nx < 0 || nx >= CanvasWidth || ny >= CanvasHeight || (ny >= 0 && grid[nx, ny] != 0)) return true;
                }
        return false;
    }

    void MovePiece(int dx, int dy) {
        if (!CheckCollision(dx, dy)) {
            currentPos.X += dx; currentPos.Y += dy;
        } else if (dy > 0) {
            LockPiece();
            ClearLines();
            SpawnPiece();
        }
    }

    void LockPiece() {
        for (int y = 0; y < currentPiece.GetLength(0); y++)
            for (int x = 0; x < currentPiece.GetLength(1); x++)
                if (currentPiece[y, x] != 0) grid[currentPos.X + x, currentPos.Y + y] = 1;
    }

    void ClearLines() {
        for (int y = CanvasHeight - 1; y >= 0; y--) {
            bool full = true;
            for (int x = 0; x < CanvasWidth; x++) if (grid[x, y] == 0) full = false;
            if (full) {
                for (int ty = y; ty > 0; ty--)
                    for (int tx = 0; tx < CanvasWidth; tx++) grid[tx, ty] = grid[tx, ty - 1];
                score += 100; y++;
            }
        }
    }

    void OnKeyDown(object sender, KeyEventArgs e) {
        if (e.KeyCode == Keys.Left) MovePiece(-1, 0);
        if (e.KeyCode == Keys.Right) MovePiece(1, 0);
        if (e.KeyCode == Keys.Down) MovePiece(0, 1);
        if (e.KeyCode == Keys.Up) RotatePiece();
        this.Invalidate();
    }

    void RotatePiece() {
        int r = currentPiece.GetLength(0), c = currentPiece.GetLength(1);
        int[,] rotated = new int[c, r];
        for (int y = 0; y < r; y++)
            for (int x = 0; x < c; x++) rotated[x, r - 1 - y] = currentPiece[y, x];
        if (!CheckCollision(0, 0, rotated)) currentPiece = rotated;
    }

    protected override void OnPaint(PaintEventArgs e) {
        Graphics g = e.Graphics;
        // 背景と固定済みブロック
        for (int x = 0; x < CanvasWidth; x++)
            for (int y = 0; y < CanvasHeight; y++)
                if (grid[x, y] != 0) g.FillRectangle(Brushes.Blue, x * TileSize, y * TileSize, TileSize - 1, TileSize - 1);
        
        // 現在のブロック
        for (int y = 0; y < currentPiece.GetLength(0); y++)
            for (int x = 0; x < currentPiece.GetLength(1); x++)
                if (currentPiece[y, x] != 0)
                    g.FillRectangle(Brushes.Red, (currentPos.X + x) * TileSize, (currentPos.Y + y) * TileSize, TileSize - 1, TileSize - 1);
        
        g.DrawString("Score: " + score, this.Font, Brushes.Black, 10, CanvasHeight * TileSize + 5);
    }

    [STAThread]
    public static void Main() { Application.Run(new Tetris()); }
}
