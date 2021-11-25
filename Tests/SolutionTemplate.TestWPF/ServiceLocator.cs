using MathCore.Hosting.WPF;
using SolutionTemplate.TestWPF.ViewModels;

namespace SolutionTemplate.TestWPF;

public class ServiceLocator : ServiceLocatorHosted
{
    public MainwindowViewModel MainModel => GetRequiredService<MainwindowViewModel>();
}