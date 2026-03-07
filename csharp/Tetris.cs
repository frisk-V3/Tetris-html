using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

public class Tetris : Form {
    const int CanvasWidth = 10, CanvasHeight = 20, TileSize = 30;
    int[,] grid = new int[CanvasWidth, CanvasHeight];
    Color[,] gridColors = new Color[CanvasWidth, CanvasHeight];
    int score = 0;
    Timer timer = new Timer();
    Point currentPos;
    int[,] currentPiece, nextPiece;
    Color currentColor, nextColor;
    Random rand = new Random();
    bool isFullScreen = false;

    // 10種類のミノ定義
    (int[,] shape, Color color)[] shapes = {
        (new int[,] { {1,1,1,1} }, Color.Cyan),        // I
        (new int[,] { {1,1}, {1,1} }, Color.Yellow),    // O
        (new int[,] { {0,1,0}, {1,1,1} }, Color.Purple), // T
        (new int[,] { {0,1,1}, {1,1,0} }, Color.Green),  // S
        (new int[,] { {1,1,0}, {0,1,1} }, Color.Red),    // Z
        (new int[,] { {1,0,0}, {1,1,1} }, Color.Orange), // L
        (new int[,] { {0,0,1}, {1,1,1} }, Color.Blue),   // J
        (new int[,] { {1} }, Color.White),              // Dot (8)
        (new int[,] { {1,1} }, Color.Pink),             // Mini-I (9)
        (new int[,] { {1,0}, {1,1} }, Color.Lime)        // Mini-L (10)
    };

    public Tetris() {
        this.Text = "Super Tetris 10";
        this.ClientSize = new Size(CanvasWidth * TileSize + 150, CanvasHeight * TileSize + 50);
        this.DoubleBuffered = true;
        this.BackColor = Color.FromArgb(20, 20, 20);
        this.KeyDown += OnKeyDown;
        
        timer.Interval = 500;
        timer.Tick += (s, e) => { MovePiece(0, 1); this.Invalidate(); };
        
        var first = shapes[rand.Next(shapes.Length)];
        nextPiece = first.shape; nextColor = first.color;
        SpawnPiece();
        timer.Start();
    }

    void SpawnPiece() {
        currentPiece = nextPiece; currentColor = nextColor;
        var next = shapes[rand.Next(shapes.Length)];
        nextPiece = next.shape; nextColor = next.color;
        
        currentPos = new Point(CanvasWidth / 2 - currentPiece.GetLength(1) / 2, 0);
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
                if (currentPiece[y, x] != 0) {
                    grid[currentPos.X + x, currentPos.Y + y] = 1;
                    gridColors[currentPos.X + x, currentPos.Y + y] = currentColor;
                }
    }

    void ClearLines() {
        for (int y = CanvasHeight - 1; y >= 0; y--) {
            bool full = true;
            for (int x = 0; x < CanvasWidth; x++) if (grid[x, y] == 0) full = false;
            if (full) {
                for (int ty = y; ty > 0; ty--)
                    for (int tx = 0; tx < CanvasWidth; tx++) {
                        grid[tx, ty] = grid[tx, ty - 1];
                        gridColors[tx, ty] = gridColors[tx, ty - 1];
                    }
                score += 100; y++;
            }
        }
    }

    void OnKeyDown(object sender, KeyEventArgs e) {
        if (e.KeyCode == Keys.Q) MovePiece(-1, 0); // Qで左
        if (e.KeyCode == Keys.W) RotatePiece();    // Wで回転
        if (e.KeyCode == Keys.Right) MovePiece(1, 0);
        if (e.KeyCode == Keys.Down) MovePiece(0, 1);
        if (e.KeyCode == Keys.F11) ToggleFullScreen(); // F11でフルスクリーン
        this.Invalidate();
    }

    void ToggleFullScreen() {
        if (!isFullScreen) {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
        } else {
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Normal;
        }
        isFullScreen = !isFullScreen;
    }

    void RotatePiece() {
        int r = currentPiece.GetLength(0), c = currentPiece.GetLength(1);
        int[,] rotated = new int[c, r];
        for (int y = 0; y < r; y++)
            for (int x = 0; x < c; x++) rotated[x, r - 1 - y] = currentPiece[y, x];
        
        int[] kicks = { 0, 1, -1, 2, -2 };
        foreach (int k in kicks) {
            if (!CheckCollision(k, 0, rotated)) {
                currentPos.X += k;
                currentPiece = rotated;
                return;
            }
        }
    }

    protected override void OnPaint(PaintEventArgs e) {
        Graphics g = e.Graphics;
        // 描画オフセットの計算（中央寄せ用）
        int offsetX = (this.ClientSize.Width - (CanvasWidth * TileSize + 120)) / 2;
        int offsetY = (this.ClientSize.Height - (CanvasHeight * TileSize)) / 2;

        // 盤面の背景
        g.FillRectangle(Brushes.Black, offsetX, offsetY, CanvasWidth * TileSize, CanvasHeight * TileSize);
        g.DrawRectangle(Pens.Gray, offsetX, offsetY, CanvasWidth * TileSize, CanvasHeight * TileSize);

        // 固定済みブロック
        for (int x = 0; x < CanvasWidth; x++)
            for (int y = 0; y < CanvasHeight; y++)
                if (grid[x, y] != 0) DrawTile(g, gridColors[x, y], offsetX + x * TileSize, offsetY + y * TileSize);
        
        // 現在のブロック
        for (int y = 0; y < currentPiece.GetLength(0); y++)
            for (int x = 0; x < currentPiece.GetLength(1); x++)
                if (currentPiece[y, x] != 0)
                    DrawTile(g, currentColor, offsetX + (currentPos.X + x) * TileSize, offsetY + (currentPos.Y + y) * TileSize);
        
        // NEXT表示
        int nextX = offsetX + CanvasWidth * TileSize + 30;
        g.DrawString("NEXT", new Font("Arial", 12, FontStyle.Bold), Brushes.White, nextX, offsetY);
        for (int y = 0; y < nextPiece.GetLength(0); y++)
            for (int x = 0; x < nextPiece.GetLength(1); x++)
                if (nextPiece[y, x] != 0)
                    DrawTile(g, nextColor, nextX + x * TileSize, offsetY + 30 + y * TileSize);

        g.DrawString("Score: " + score, new Font("Arial", 12), Brushes.White, nextX, offsetY + 150);
        g.DrawString("Q: Left / W: Rotate\nF11: FullScreen", new Font("Arial", 9), Brushes.Gray, nextX, offsetY + 200);
    }

    void DrawTile(Graphics g, Color color, int px, int py) {
        g.FillRectangle(new SolidBrush(color), px, py, TileSize - 1, TileSize - 1);
        g.DrawRectangle(new Pen(Color.FromArgb(50, 255, 255, 255)), px, py, TileSize - 1, TileSize - 1);
    }

    [STAThread]
    public static void Main() { Application.Run(new Tetris()); }
}
