using Xunit;

namespace AsyncSample.ViewModels.Tests;

public class MainViewModelTests
{
    [Fact(DisplayName = "1.オブジェクト構築テスト")]
    public void MainViewModelTest()
    {
        var vm = new MainViewModel(new GeneratorStub());

        Assert.Equal("開始", vm.ButtonText.Value);
        Assert.False(vm.IsWorking.Value);
        Assert.Equal(0, vm.WordCount.Value);
        Assert.True(vm.Corrects.Value.All(x => !x.IsFinished.Value));
        Assert.True(vm.Answers.Value.All(x => !x.IsFinished.Value));
    }

    [Fact(DisplayName = "2.完了確認テスト")]
    public async Task MainViewModelSuccessTest()
    {
        var vm = new MainViewModel(new GeneratorStub());
        vm.StartCommand.Execute();
        await Task.Delay(2000);

        Assert.Equal("開始", vm.ButtonText.Value);
        Assert.False(vm.IsWorking.Value);
        Assert.Equal(5, vm.WordCount.Value);
        Assert.True(vm.Corrects.Value.All(x => x.IsFinished.Value));
        Assert.True(vm.Answers.Value.All(x => x.IsFinished.Value));

        // 回答の最初と最後のセルだけピックアップして内容確認
        var first = vm.Answers.Value.First();
        var last = vm.Answers.Value.Last();
        Assert.Equal("A", first.Text.Value);
        Assert.Equal(1, first.Index);
        Assert.Equal("C", last.Text.Value);
        Assert.Equal(5, last.Index);
    }

    [Fact(DisplayName = "3.処理中確認テスト")]
    public async Task MainViewModelMidstreamTest()
    {
        var vm = new MainViewModel(new MidstreamGeneratorStub());
        vm.StartCommand.Execute();
        await Task.Delay(1000);

        Assert.Equal("停止", vm.ButtonText.Value);
        Assert.True(vm.IsWorking.Value);
        Assert.True(vm.Corrects.Value.All(x => !x.IsFinished.Value));
        Assert.Equal("C", vm.Answers.Value.Single(x => x.IsFinished.Value).Text.Value);
    }

    [Fact(DisplayName = "4.キャンセルテスト")]
    public async Task MainViewModelCancelTest()
    {
        var vm = new MainViewModel(new MidstreamGeneratorStub());
        vm.StartCommand.Execute();
        await Task.Delay(1000);
        vm.StartCommand.Execute();

        Assert.Equal("開始", vm.ButtonText.Value);
        Assert.False(vm.IsWorking.Value);
        Assert.True(vm.Corrects.Value.All(x => !x.IsFinished.Value));
        Assert.Equal("C", vm.Answers.Value.Single(x => x.IsFinished.Value).Text.Value);
    }
}
