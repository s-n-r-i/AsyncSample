using System.Globalization;

namespace AsyncSample.ViewModels;

/// <summary>
/// 実行データ生成クラス
/// </summary>
public class Generator : IGenerator
{
    /// <summary>
    /// 正解文字列
    /// </summary>
    public string CorrectWord { get; private set; } = string.Empty;

    /// <summary>
    /// 正解文字列を1文字ずつに分解したリスト
    /// </summary>
    public IEnumerable<string> CorrectWords { get; private set; } = Enumerable.Empty<string>();

    /// <summary>
    /// 生成回数
    /// </summary>
    private int GenerateCount { get; set; }

    /// <summary>
    /// 使用する文字
    /// </summary>
    private const string Characters = @"ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <summary>
    /// 乱数
    /// </summary>
    private Random Random { get; } = new();

    /// <summary>
    /// 生成
    /// </summary>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public void Generate()
    {
        CorrectWord = GenerateCount++ switch
        {
            0 => @"ASYNC",
            1 => @"CSHARP",
            2 => @"VISUALSTUDIO",
            _ => throw new IndexOutOfRangeException(),
        };

        CorrectWords = StringToStrings(CorrectWord);

        if (GenerateCount >= 3)
        {
            GenerateCount = 0;
        }
    }

    /// <summary>
    /// ランダムに1文字取得
    /// </summary>
    /// <returns></returns>
    public string GetRandomString() => Characters[Random.Next(Characters.Length)].ToString();

    /// <summary>
    /// インターバル用数値取得
    /// </summary>
    /// <returns></returns>
    public int GetInterval()
    {
        var randInt = Random.Next(0, 5);
        return (randInt + 1) * 200; // 0.2～1秒
    }

    /// <summary>
    /// string → 一文字ずつのstring[]
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    private static IEnumerable<string> StringToStrings(string source)
    {
        if (string.IsNullOrEmpty(source))
        {
            yield break;
        }

        var si = new StringInfo(source);
        for (int i = 0; i <= si.LengthInTextElements - 1; i++)
        {
            yield return si.SubstringByTextElements(i, 1);
        }
    }
}
