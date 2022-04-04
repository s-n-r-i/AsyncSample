using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncSample.ViewModels.Tests;

[TestClass()]
public class MainViewModelTests
{
    [TestMethod("1.オブジェクト構築テスト")]
    public void MainViewModelTest()
    {
        var vm = new MainViewModel(new GeneratorStub());

        Assert.AreEqual("開始", vm.ButtonText.Value);
        Assert.IsFalse(vm.IsWorking.Value);
        Assert.AreEqual(0, vm.WordCount.Value);
        Assert.IsTrue(vm.Corrects.Value.All(x => !x.IsFinished.Value));
        Assert.IsTrue(vm.Answers.Value.All(x => !x.IsFinished.Value));
    }

    [TestMethod("2.完了確認テスト")]
    public async Task MainViewModelSuccessTest()
    {
        var vm = new MainViewModel(new GeneratorStub());
        vm.StartCommand.Execute();
        await Task.Delay(2000);

        Assert.AreEqual("開始", vm.ButtonText.Value);
        Assert.IsFalse(vm.IsWorking.Value);
        Assert.AreEqual(5, vm.WordCount.Value);
        Assert.IsTrue(vm.Corrects.Value.All(x => x.IsFinished.Value));
        Assert.IsTrue(vm.Answers.Value.All(x => x.IsFinished.Value));

        // 回答の最初と最後のセルだけピックアップして内容確認
        var first = vm.Answers.Value.First();
        var last = vm.Answers.Value.Last();
        Assert.AreEqual("A", first.Text.Value);
        Assert.AreEqual(1, first.Index);
        Assert.AreEqual("C", last.Text.Value);
        Assert.AreEqual(5, last.Index);
    }

    [TestMethod("3.処理中確認テスト")]
    public async Task MainViewModelMidstreamTest()
    {
        var vm = new MainViewModel(new MidstreamGeneratorStub());
        vm.StartCommand.Execute();
        await Task.Delay(1000);

        Assert.AreEqual("停止", vm.ButtonText.Value);
        Assert.IsTrue(vm.IsWorking.Value);
        Assert.IsTrue(vm.Corrects.Value.All(x => !x.IsFinished.Value));
        Assert.AreEqual("C", vm.Answers.Value.Single(x => x.IsFinished.Value).Text.Value);
    }

    [TestMethod("4.キャンセルテスト")]
    public async Task MainViewModelCancelTest()
    {
        var vm = new MainViewModel(new MidstreamGeneratorStub());
        vm.StartCommand.Execute();
        await Task.Delay(1000);
        vm.StartCommand.Execute();

        Assert.AreEqual("開始", vm.ButtonText.Value);
        Assert.IsFalse(vm.IsWorking.Value);
        Assert.IsTrue(vm.Corrects.Value.All(x => !x.IsFinished.Value));
        Assert.AreEqual("C", vm.Answers.Value.Single(x => x.IsFinished.Value).Text.Value);
    }
}
