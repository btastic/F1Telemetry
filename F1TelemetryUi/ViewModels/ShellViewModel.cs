using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace F1TelemetryUi.ViewModels
{
    [Export(typeof(IShell))]
    public class ShellViewModel : PropertyChangedBase, IShell
    {
    }
}
