using GeneracionNumeracionAvalonia;
using GeneracionNumeracionAvalonia.Services;
using GeneracionNumeracionAvalonia.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MsBox.Avalonia.Enums;

namespace GeneradorNumeracionTest;

public class Tests
{
    MainWindowViewModel mainWindowViewModel;
    Mock<IMessageBox> mockIMessage;
    Mock<ILogger> mockILogger;
    Mock<IServiceProvider> mockProvider;

    [SetUp]
    public void Setup()
    {
        mockIMessage = new Mock<IMessageBox>();
        mockILogger = new Mock<ILogger>();
        mockProvider = new Mock<IServiceProvider>();

        mockProvider.Setup(m => m.GetService(typeof(IMessageBox))).Returns(mockIMessage.Object);
        mockProvider.Setup(m => m.GetService(typeof(ILogger))).Returns(mockILogger.Object);

        App.ServiceProvider = mockProvider.Object;

        mainWindowViewModel = new();
    }

    [Test]
    public void ExistsExtensions()
    {
        mainWindowViewModel.Counters = 1;
        mainWindowViewModel.Start = 1;
        mainWindowViewModel.End = 100;

        Assert.That(mainWindowViewModel.Extensions, Is.Not.Null);
    }

    [Test]
    public void InitApp_InitVM_CheckExtensions()
    {
        Assert.That(mainWindowViewModel.Extensions, Is.Not.Null);
    }

    [Test]
    public void GenerateExecute_ValidationFails_ReturnsWithoutExecuting()
    {
        mainWindowViewModel.End = 0;
        mainWindowViewModel.GenerateCommand.Execute(null);

        mainWindowViewModel.Start = 10;
        mainWindowViewModel.End = 5;
        mainWindowViewModel.GenerateCommand.Execute(null);

        mainWindowViewModel.Counters = 0;
        mainWindowViewModel.GenerateCommand.Execute(null);

        mockIMessage.Setup(m => m.GetMessageBoxStandard(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<ButtonEnum>(),
            It.IsAny<Icon>()))
            .Returns(Task.FromResult<ButtonResult>(ButtonResult.No));

        mainWindowViewModel.Counters = 2;
        mainWindowViewModel.Start = 1;
        mainWindowViewModel.End = 51;
        mainWindowViewModel.GenerateCommand.Execute(null);

        mockIMessage.Verify(
            m => m.GetMessageBoxStandard(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<ButtonEnum>(),
                It.IsAny<Icon>()),
            Times.Exactly(4));
    }
}
