using Prism.Mvvm;
using Reactive.Bindings;

namespace AsyncSample.ViewModels;

/// <summary>
/// ViewModel
/// </summary>
public class MainViewModel : BindableBase
{
    /// <summary>
    /// 開始・停止コマンド
    /// </summary>
    public ReactiveCommand StartCommand { get; } = new();

    /// <summary>
    /// ボタン文言
    /// </summary>
    public ReactivePropertySlim<string> ButtonText { get; } = new(string.Empty);

    /// <summary>
    /// 正解セルリスト
    /// </summary>
    public ReactivePropertySlim<IEnumerable<CellViewModel>> Corrects { get; } = new(Enumerable.Empty<CellViewModel>());

    /// <summary>
    /// 回答セルリスト
    /// </summary>
    public ReactivePropertySlim<IEnumerable<CellViewModel>> Answers { get; } = new(Enumerable.Empty<CellViewModel>());

    /// <summary>
    /// 文字数
    /// </summary>
    public ReactivePropertySlim<int> WordCount { get; } = new();

    /// <summary>
    /// 作業中フラグ
    /// </summary>
    public ReactivePropertySlim<bool> IsWorking { get; } = new(false);

    /// <summary>
    /// 実行データ生成クラス
    /// </summary>
    private IGenerator Generator { get; }

    /// <summary>
    /// 中断用CancellationTokenSource
    /// </summary>
    private CancellationTokenSource Cancellation { get; set; } = new();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="generator"></param>
    public MainViewModel(IGenerator generator)
    {
        Generator = generator ?? throw new ArgumentNullException(nameof(generator));

        IsWorking.Subscribe(ChangeButtonText);
        StartCommand.Subscribe(async () => await Main());
    }

    /// <summary>
    /// 処理メイン
    /// </summary>
    /// <returns></returns>
    private async Task Main()
    {
        if (IsWorking.Value)
        {
            // 実行中ならばキャンセル発行して抜ける
            IsWorking.Value = false;
            Cancellation.Cancel();
            return;
        }

        IsWorking.Value = true;

        // 正解を生成
        Generator.Generate();
        WordCount.Value = Generator.CorrectWords.Count();
        Corrects.Value = Generator.CorrectWords.Select((s, i) => new CellViewModel(i + 1, s)).ToArray();

        // 回答枠作成
        Answers.Value = Enumerable.Range(1, WordCount.Value).Select(i => new CellViewModel(i, string.Empty)).ToArray();

        try
        {
            Cancellation = new();
            // 全ての回答が正解と一致するまで待つ
            await Task.WhenAll(Answers.Value.Select(x => ChangeText(x, Corrects.Value.ElementAt(x.Index - 1).Text.Value, Cancellation.Token)))
                    .ConfigureAwait(false);

            foreach (var correct in Corrects.Value)
            {
                correct.IsFinished.Value = true;
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
            throw;
        }

        IsWorking.Value = false;
    }

    /// <summary>
    /// セル処理メイン
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="answer"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    private Task ChangeText(CellViewModel cell, string answer, CancellationToken token)
    {
        return Task.Run(async () =>
        {
            while (true)
            {
                // 回答として1文字取得して表示
                var value = Generator.GetRandomString();
                cell.Text.Value = value;

                if (value == answer)
                {
                    // 回答と正解が一致しらたフラグ立てて終了
                    cell.IsFinished.Value = true;
                    return;
                }

                await Task.Delay(Generator.GetInterval());

                token.ThrowIfCancellationRequested();
            }
        }, token);
    }

    /// <summary>
    /// ボタン文字列変更
    /// </summary>
    /// <param name="isWorking"></param>
    private void ChangeButtonText(bool isWorking) => ButtonText.Value = isWorking ? "停止" : "開始";
}

/// <summary>
/// セルViewModel
/// </summary>
public class CellViewModel : BindableBase
{
    /// <summary>
    /// 何文字目か(1スタート)
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// 表示文字
    /// </summary>
    public ReactivePropertySlim<string> Text { get; } = new(string.Empty);

    /// <summary>
    /// 処理完了フラグ(View側で適用するテンプレートの判定用)
    /// </summary>
    public ReactivePropertySlim<bool> IsFinished { get; } = new(false);

    public CellViewModel(int index, string text)
    {
        Index = index;
        Text.Value = text;
    }
}