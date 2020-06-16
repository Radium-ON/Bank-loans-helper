using System.Linq;
using FirstFloor.ModernUI.Windows.Controls;

namespace LoanHepler.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            ContentSource = MenuLinkGroups.First().Links.First().Source;
        }
    }
}
