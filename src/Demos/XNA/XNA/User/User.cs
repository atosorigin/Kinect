using System.Collections.Generic;
using Microsoft.Research.Kinect.Nui;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Kinect.Common;
using System.Windows;
using log4net;

namespace Kinect.XNA.User
{
    public class User
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(User));
        private Kinect.Core.User _user;
        private Dictionary<JointID, Bodypart> _bodyparts = new Dictionary<JointID, Bodypart>();
        private ContentManager _content;
        private Size WindowSize;

        public User(Kinect.Core.User user, ContentManager content, Size windowSize)
        {
            // TODO: Complete member initialization
            _user = user;
            this._content = content;
            this.WindowSize = windowSize;
            AddBodyParts();
            _user.Updated += Kinect_User_Updated;
        }
        
        private void AddBodyParts()
        {
            _bodyparts.Add(JointID.Spine,new Bodypart(_content.Load<Texture2D>("Images/Torso")));
            _bodyparts.Add(JointID.ShoulderLeft, new Bodypart(_content.Load<Texture2D>("Images/LeftArm")));
            _bodyparts.Add(JointID.ShoulderRight, new Bodypart(_content.Load<Texture2D>("Images/RightArm")));
            _bodyparts.Add(JointID.HipLeft, new Bodypart(_content.Load<Texture2D>("Images/LeftLeg")));
            _bodyparts.Add(JointID.HipRight, new Bodypart(_content.Load<Texture2D>("Images/RightLeg")));
        }

        private void Kinect_User_Updated(object sender, Core.Eventing.ProcessEventArgs<Core.IUserChangedEvent> e)
        {
            _bodyparts[JointID.Spine].LeftPoint = e.Event.Torso.ToScreenPosition(new System.Windows.Size(640, 480), WindowSize);
            _log.DebugFormat("{0}:{1}", JointID.ElbowLeft, _bodyparts[JointID.ElbowLeft].LeftPoint);
            _log.DebugFormat("{0}:{1}", JointID.ShoulderLeft, _bodyparts[JointID.ShoulderLeft].RightPoint);
            //_bodyparts[JointID.RightShoulder].LeftTop = e.Event.RightShoulder.ToScreenPosition(new System.Windows.Size(640, 480), WindowSize); 
            //_bodyparts[JointID.RightShoulder].RightBottom = e.Event.RightElbow.ToScreenPosition(new System.Windows.Size(640, 480), WindowSize); 
            //_bodyparts[JointID.LeftHip].LeftTop = e.Event.LeftHip.ToScreenPosition(new System.Windows.Size(640, 480), WindowSize); ;
            //_bodyparts[JointID.LeftHip].RightBottom = e.Event.LeftKnee.ToScreenPosition(new System.Windows.Size(640, 480), WindowSize);
            //_bodyparts[JointID.RightHip].LeftTop = e.Event.RightHip.ToScreenPosition(new System.Windows.Size(640, 480), WindowSize);
            //_bodyparts[JointID.RightHip].RightBottom = e.Event.RightKnee.ToScreenPosition(new System.Windows.Size(640, 480), WindowSize);
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            foreach (var bodypart in _bodyparts)
            {
                bodypart.Value.Draw(spriteBatch);
            }
        }
    }
}
