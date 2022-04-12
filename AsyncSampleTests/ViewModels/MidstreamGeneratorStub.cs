namespace AsyncSample.ViewModels.Tests;

/// <summary>
/// 実行データ生成 途中データ作成用stub
/// </summary>
public class MidstreamGeneratorStub : IGenerator
{
    public string CorrectWord { get; } = @"ASYNC";

    public IEnumerable<string> CorrectWords { get; } = new[] { "A", "S", "Y", "N", "C" };

    public void Generate()
    {
    }

    public int GetInterval() => 200;

    public string GetRandomString() => "C"; //部分しかマッチしない
}
