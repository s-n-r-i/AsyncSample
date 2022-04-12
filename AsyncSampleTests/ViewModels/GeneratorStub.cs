namespace AsyncSample.ViewModels.Tests;

/// <summary>
/// 実行データ生成 完了データ作成用stub
/// </summary>
public class GeneratorStub : IGenerator
{
    public string CorrectWord { get; } = @"ASYNC";

    public IEnumerable<string> CorrectWords { get; } = new[] { "A", "S", "Y", "N", "C" };

    public void Generate()
    {
    }

    public int GetInterval() => 200;

    private int counter;

    private const string ReturnStrings = @"AAAAASSSSYYYNNC"; //正解文字列の先頭からそれぞれ1回～5回の試行でマッチする

    public string GetRandomString()
    {
        return ReturnStrings[counter++].ToString();
    }
}
