using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

public class TetrisGame : Form {
    Timer timer = new Timer();
    int[,] field = new int[10, 20];
    int curX, curY, curType;
    int[][,] blocks = {
        new int[,] {{1,1,1,1}}, // I
        new int[,] {{1,1},{1,1}}, // O
        new int[,] {{0,1,0},{1,1,1}}, // T
        new int[,] {{1,1,0},{0,1,1}}, // Z
        new int[,] {{0,1,1},{1,1,0}}  // S
    };

    public TetrisGame() {
        this.Text = "C# Tetris (csc.exe)";
        this.ClientSize = new Size(200, 400);
        this.DoubleBuffered = true;
        timer.Interval = 500;
        timer.Tick += (s, e) => { curY++; if(IsHit()) { curY--; LockBlock(); } Invalidated(); };
        timer.Start();
        Spawn();
    }

    void Spawn() {
        curX = 4; curY = 0; curType = new Random().Next(blocks.Length);
        if(IsHit()) { timer.Stop(); MessageBox.Show("Game Over"); }
    }

    bool IsHit() {
        var b = blocks[curType];
        for(int y=0; y<b.GetLength(0); y++)
            for(int x=0; x<b.GetLength(1); x++)
                if(b[y,x] != 0 && (curX+x<0 || curX+x>=10 || curY+y>=20 || field[curX+x, curY+y] != 0)) return true;
        return false;
    }

    void LockBlock() {
        var b = blocks[curType];
        for(int y=0; y<b.GetLength(0); y++)
            for(int x=0; x<b.GetLength(1); x++)
                if(b[y,x] != 0) field[curX+x, curY+y] = 1;
        
        for(int y=19; y>=0; y--) {
            bool full = true;
            for(int x=0; x<10; x++) if(field[x,y]==0) full = false;
            if(full) {
                for(int ty=y; ty>0; ty--) for(int tx=0; tx<10; tx++) field[tx,ty] = field[tx,ty-1];
                y++;
            }
        }
        Spawn();
    }

    protected override void OnKeyDown(KeyEventArgs e) {
        int oldX = curX, oldY = curY;
        if(e.KeyCode == Keys.Left) curX--;
        if(e.KeyCode == Keys.Right) curX++;
        if(e.KeyCode == Keys.Down) curY++;
        if(IsHit()) { curX = oldX; curY = oldY; }
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e) {
        for(int x=0; x<10; x++) for(int y=0; y<20; y++)
            if(field[x,y] != 0) e.Graphics.FillRectangle(Brushes.Blue, x*20, y*20, 19, 19);
        
        var b = blocks[curType];
        for(int y=0; y<b.GetLength(0); y++)
            for(int x=0; x<b.GetLength(1); x++)
                if(b[y,x] != 0) e.Graphics.FillRectangle(Brushes.Red, (curX+x)*20, (curY+y)*20, 19, 19);
    }

    public static void Main() { Application.Run(new TetrisGame()); }
}
