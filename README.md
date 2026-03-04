# Tetris-html Pack

このプロジェクトは、Webブラウザで動く **HTML/JS版** をメインとし、
おまけとしてWindowsネイティブで動く **C#版** を同梱したテトリス実装セットです。

## 1. 【メイン】HTML/JS 版テトリス
ブラウザさえあれば、OSを問わずどこでもすぐにプレイ可能です。

### 起動方法
1. `index.html` をブラウザで開く。

### 操作方法
- **A / D**: 左右移動
- **S**: 高速落下
- **W**: 回転
- **Q**: 左回転（JS版のみ）

---

## 2. 【おまけ】C# 版テトリス
Windows環境で `csc.exe` を使って実行ファイル (.exe) を生成するレトロな実装です。

### ビルド手順
コマンドプロンプトで以下を実行してください：
```cmd
C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /target:winexe Tetris.cs
