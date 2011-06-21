using System.Windows;

namespace Kinect.Plugins.Common.Views
{
    /// <summary>
    /// Description for ConfigureKinect.
    /// </summary>
    public partial class ConfigureKinect : Window
    {
        /// <summary>
        /// Initializes a new instance of the ConfigureKinect class.
        /// </summary>
        public ConfigureKinect()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}