using MathCore.Hosting;
using MathCore.WPF.ViewModels;
using SolutionTemplate.TestWPF.Services.Interfaces;

namespace SolutionTemplate.TestWPF.ViewModels;

[Service]
public class MainwindowViewModel : TitledViewModel
{
    public MainwindowViewModel() => Title = "Главное окно";

    [Inject]
    public IUserDialog UserDialog { get; init; }
}