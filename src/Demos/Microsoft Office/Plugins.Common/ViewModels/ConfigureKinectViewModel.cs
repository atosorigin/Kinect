using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Windows.Controls;
using System;
using System.Windows.Media.Media3D;

namespace Kinect.Plugins.Common.ViewModels
{

    public class ConfigureKinectViewModel : ViewModelBase
    {
        private const string _calibrationmessage = "Put {0} on {1}";

        public List<xn.SkeletonJoint> AvailebleJoints { get; set; }

        public xn.SkeletonJoint MovePointer { get; set; }
        public bool DefaultEnableMovePointer { get; set; }
        public xn.SkeletonJoint TogglePointer1 { get; set; }
        public xn.SkeletonJoint TogglePointer2 { get; set; }
        public bool CalibrateTogglePointer { get; set; }
        public bool EnableTogglePointer { get; set; }
        public xn.SkeletonJoint NextSlide1 { get; set; }
        public xn.SkeletonJoint NextSlide2 { get; set; }
        public bool CalibrateNextSlide { get; set; }
        public bool EnableNextSlide { get; set; }
        public xn.SkeletonJoint PreviousSlide1 { get; set; }
        public xn.SkeletonJoint PreviousSlide2 { get; set; }
        public bool CalibratePreviousSlide { get; set; }
        public bool EnablePreviousSlide { get; set; }

        public string TogglePointerCalibrationMessage { get { return string.Format(_calibrationmessage, TogglePointer1, TogglePointer2); } }
        public string NextSlideCalibrationMessage { get { return string.Format(_calibrationmessage, NextSlide1, NextSlide2); } }
        public string PreviousSlideCalibrationMessage { get { return string.Format(_calibrationmessage, PreviousSlide1, PreviousSlide2); } }

        public Point3D NextSlideCorrection { get; set; }
        public Point3D PreviousSlideCorrection { get; set; }
        public Point3D TogglePointerCorrection { get; set; }

        private int _countdown;
        public int Countdown
        {
            get
            {
                return _countdown;
            }
            set
            {
                _countdown = value;
                RaisePropertyChanged("Countdown");
            }
        }


        public ConfigureKinectViewModel()
        {
            MovePointer = xn.SkeletonJoint.RightHand;
            TogglePointer1 = xn.SkeletonJoint.RightHand;
            TogglePointer2 = xn.SkeletonJoint.Head;
            NextSlide1 = xn.SkeletonJoint.LeftHand;
            NextSlide2 = xn.SkeletonJoint.RightShoulder;
            PreviousSlide1 = xn.SkeletonJoint.RightHand;
            PreviousSlide2 = xn.SkeletonJoint.LeftShoulder;
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

        private void FillJoints()
        {
            AvailebleJoints = new List<xn.SkeletonJoint>();
            foreach (xn.SkeletonJoint joint in Enum.GetValues(typeof(xn.SkeletonJoint)))
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