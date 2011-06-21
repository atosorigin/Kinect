using System;
using Kinect.Plugins.Common;
using Kinect.Plugins.PowerPoint2010.Properties;
using Microsoft.Office.Tools.Ribbon;

namespace Kinect.Plugins.PowerPoint2010
{
    public partial class KinectRibbon
    {
        private void KinectRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            KinectManager.Instance.UserFound += KinectManager_UserFound;
            KinectManager.Instance.UserLost += KinectManager_UserLost;
            KinectManager.Instance.KinectStarted += KinectManager_KinectStarted;
            KinectManager.Instance.KinectStopped += KinectManager_KinectStopped;
        }

        private void KinectManager_KinectStarted(object sender, EventArgs e)
        {
            StartKinect.Image = Resources.btnStop;
            StartKinect.Label = "Stop Kinect";
        }

        private void KinectManager_KinectStopped(object sender, EventArgs e)
        {
            StartKinect.Image = Resources.btnStart;
            StartKinect.Label = "Start Kinect";
        }

        private void KinectManager_UserLost(object sender, EventArgs e)
        {
            btnCalibrate.Image = Resources.user_red;
            btnCalibrate.Enabled = false;
        }

        private void KinectManager_UserFound(object sender, EventArgs e)
        {
            btnCalibrate.Image = Resources.user_green;
            btnCalibrate.Enabled = true;
        }

        private void StartKinect_Click(object sender, RibbonControlEventArgs e)
        {
            if (!KinectManager.Instance.Running)
            {
                KinectManager.Instance.StartKinect();
            }
            else
            {
                KinectManager.Instance.StopKinect();
            }
        }

        private void Configure_Click(object sender, RibbonControlEventArgs e)
        {
            KinectManager.Instance.OpenConfiguration();
        }

        private void btnCalibrate_Click(object sender, RibbonControlEventArgs e)
        {
            //TODO: Jan needs to fix this
            //KinectManager.Instance.StartCalibration();
        }
    }
}