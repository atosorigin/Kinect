namespace Kinect.Plugins.PowerPoint2010
{
    partial class KinectRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public KinectRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.KinectTab = this.Factory.CreateRibbonTab();
            this.KinectGroup = this.Factory.CreateRibbonGroup();
            this.Configure = this.Factory.CreateRibbonButton();
            this.StartKinect = this.Factory.CreateRibbonButton();
            this.btnCalibrate = this.Factory.CreateRibbonButton();
            this.KinectTab.SuspendLayout();
            this.KinectGroup.SuspendLayout();
            // 
            // KinectTab
            // 
            this.KinectTab.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.KinectTab.Groups.Add(this.KinectGroup);
            this.KinectTab.Label = "Kinect";
            this.KinectTab.Name = "KinectTab";
            // 
            // KinectGroup
            // 
            this.KinectGroup.Items.Add(this.Configure);
            this.KinectGroup.Items.Add(this.StartKinect);
            this.KinectGroup.Items.Add(this.btnCalibrate);
            this.KinectGroup.Label = "Kinect";
            this.KinectGroup.Name = "KinectGroup";
            // 
            // Configure
            // 
            this.Configure.Image = global::Kinect.Plugins.PowerPoint2010.Properties.Resources.configure;
            this.Configure.Label = "Configure Kinect";
            this.Configure.Name = "Configure";
            this.Configure.ShowImage = true;
            this.Configure.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.Configure_Click);
            // 
            // StartKinect
            // 
            this.StartKinect.Image = global::Kinect.Plugins.PowerPoint2010.Properties.Resources.btnStart;
            this.StartKinect.Label = "Start Kinect";
            this.StartKinect.Name = "StartKinect";
            this.StartKinect.ShowImage = true;
            this.StartKinect.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.StartKinect_Click);
            // 
            // btnCalibrate
            // 
            this.btnCalibrate.Image = global::Kinect.Plugins.PowerPoint2010.Properties.Resources.user_red;
            this.btnCalibrate.Label = " Calibrate User";
            this.btnCalibrate.Name = "btnCalibrate";
            this.btnCalibrate.ShowImage = true;
            this.btnCalibrate.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnCalibrate_Click);
            // 
            // KinectRibbon
            // 
            this.Name = "KinectRibbon";
            this.RibbonType = "Microsoft.PowerPoint.Presentation";
            this.Tabs.Add(this.KinectTab);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.KinectRibbon_Load);
            this.KinectTab.ResumeLayout(false);
            this.KinectTab.PerformLayout();
            this.KinectGroup.ResumeLayout(false);
            this.KinectGroup.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab KinectTab;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup KinectGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton StartKinect;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton Configure;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnCalibrate;
    }

    partial class ThisRibbonCollection
    {
        internal KinectRibbon KinectRibbon
        {
            get { return this.GetRibbon<KinectRibbon>(); }
        }
    }
}
