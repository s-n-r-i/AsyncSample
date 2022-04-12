namespace AsyncSample.ViewModels;

/// <summary>
/// 実行データ生成クラス インターフェース
/// </summary>
public interface IGenerator
{
    public string CorrectWord { get; }
    public IEnumerable<string> CorrectWords { get; }
    public void Generate();
    public string GetRandomString();
    public int GetInterval();
}
