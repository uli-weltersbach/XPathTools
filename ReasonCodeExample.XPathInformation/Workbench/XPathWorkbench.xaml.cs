using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReasonCodeExample.XPathInformation.Workbench
{
    /// <summary>
    /// Interaction logic for XPathWorkbench.xaml
    /// </summary>
    public partial class XPathWorkbench : UserControl
    {
        private readonly XmlRepository _repository;

        internal XPathWorkbench(XmlRepository repository)
        {
            _repository = repository;
            InitializeComponent();
        }

        private void OnSearchKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                // Search.
            }
        }
    }
}
