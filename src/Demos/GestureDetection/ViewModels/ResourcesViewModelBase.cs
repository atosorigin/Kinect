using System.Reflection;
using GalaSoft.MvvmLight;

namespace Kinect.GestureDetection.ViewModels
{
    class ResourcesViewModelBase : ViewModelBase
    {
        public string AppTitle { get; set; }
        public string About { get; set; }
        public string CompanyUrl { get; set; }
        public string Copyright { get; set; }
        public string AppVersion { get; set; }

        public string ApplicationVersion
        {
            get
            {
                if (IsInDesignMode)
                {
                    return "x.x.x";
                }

                Assembly assembly = Assembly.GetExecutingAssembly();
                var name = new AssemblyName(assembly.FullName);
                return name.Version.ToString(3);
            }
        }

        public void LoadValuesFromResource<T>()
        {
            var targetType = GetType();
            var sourceType = typeof(T);
            foreach (var targetProperty in targetType.GetProperties())
            {
                var sourceProperty = sourceType.GetProperty(targetProperty.Name, BindingFlags.Static | BindingFlags.Public);
                if (sourceProperty != null)
                {
                    targetProperty.SetValue(this, sourceProperty.GetValue(null, null), null);
                }
            }
        }
    }
}
