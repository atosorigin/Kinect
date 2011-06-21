using System.Reflection;
using System.Runtime.InteropServices;
using log4net.Config;

// Log 4 net logging watch

[assembly: XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("Kinect XNA")]
[assembly: AssemblyProduct("Kinect XNA")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyCompany("Atos Origin")]
[assembly: AssemblyCopyright("Copyright © Atos Origin 2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type. Only Windows
// assemblies support COM.

[assembly: ComVisible(false)]

// On Windows, the following GUID is for the ID of the typelib if this
// project is exposed to COM. On other platforms, it unique identifies the
// title storage container when deploying this assembly to the device.

[assembly: Guid("fd47d3fb-e54a-42ef-bb68-fb08eefe0c52")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//

[assembly: AssemblyVersion("1.0.0.0")]