using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using GalaSoft.MvvmLight;
using Microsoft.Research.Kinect.Nui;

namespace Kinect.Plugins.Common.ViewModels
{
    public class ConfigureKinectViewModel : ViewModelBase
    {
        private const string _calibrationmessage = "Put {0} on {1}";
        private int _countdown;

        public ConfigureKinectViewModel()
        {
            MovePointer = JointID.HandRight;
            TogglePointer1 = JointID.HandRight;
            TogglePointer2 = JointID.Head;
            NextSlide1 = JointID.HandLeft;
            NextSlide2 = JointID.ShoulderRight;
            PreviousSlide1 = JointID.HandRight;
            PreviousSlide2 = JointID.ShoulderLeft;
            Countdown = 5;

            //Calibration
            CalibrateTogglePointer = true;
            CalibrateNextSlide = true;
            CalibratePreviousSlide = true;

            //Enable
            DefaultEnableMovePointer = true;
            EnableNextSlide = true;
            EnablePreviousSlide = true;
            EnableTogglePointer = true;
            FillJoints();
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real": Connect to service, etc...
            ////}
        }

        public List<JointID> AvailebleJoints { get; set; }

        public JointID MovePointer { get; set; }
        public bool DefaultEnableMovePointer { get; set; }
        public JointID TogglePointer1 { get; set; }
        public JointID TogglePointer2 { get; set; }
        public bool CalibrateTogglePointer { get; set; }
        public bool EnableTogglePointer { get; set; }
        public JointID NextSlide1 { get; set; }
        public JointID NextSlide2 { get; set; }
        public bool CalibrateNextSlide { get; set; }
        public bool EnableNextSlide { get; set; }
        public JointID PreviousSlide1 { get; set; }
        public JointID PreviousSlide2 { get; set; }
        public bool CalibratePreviousSlide { get; set; }
        public bool EnablePreviousSlide { get; set; }

        public string TogglePointerCalibrationMessage
        {
            get { return string.Format(_calibrationmessage, TogglePointer1, TogglePointer2); }
        }

        public string NextSlideCalibrationMessage
        {
            get { return string.Format(_calibrationmessage, NextSlide1, NextSlide2); }
        }

        public string PreviousSlideCalibrationMessage
        {
            get { return string.Format(_calibrationmessage, PreviousSlide1, PreviousSlide2); }
        }

        public Point3D NextSlideCorrection { get; set; }
        public Point3D PreviousSlideCorrection { get; set; }
        public Point3D TogglePointerCorrection { get; set; }

        public int Countdown
        {
            get { return _countdown; }
            set
            {
                _countdown = value;
                RaisePropertyChanged("Countdown");
            }
        }

        private void FillJoints()
        {
            AvailebleJoints = new List<JointID>();
            foreach (JointID joint in Enum.GetValues(typeof (JointID)))
            {
                AvailebleJoints.Add(joint);
            }
        }

        public override void Cleanup()
        {
            // Clean own resources if needed
            AvailebleJoints.Clear();
            AvailebleJoints = null;
            base.Cleanup();
        }
    }
}