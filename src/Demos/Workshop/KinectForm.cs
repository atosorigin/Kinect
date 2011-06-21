using System;
using System.Windows.Forms;
using Kinect.Core;

namespace Kinect.Workshop
{
    public partial class KinectForm : Form
    {
        private MyKinect _kinect;

        public KinectForm()
        {
            InitializeComponent();
            AddItemToListbox(lbMessages, "Application started");
            AddItemToListbox(lbMessages, "Please start Kinect");
            InitializeKinect();
        }

        private void InitializeKinect()
        {
            _kinect = MyKinect.Instance;
            _kinect.SingleUserMode = true;
            _kinect.CameraMessage += KinectCameraMessage;
            //TODO: Workshop -> Stap 1:
            //TODO: Workshop -> Abonneren op de overige events die voor jou belangrijk zijn
            //TODO: Workshop -> En zorg er dan voor dat er een opmerking in de lbMessages wordt opgenomen
            //TODO: Workshop -> De functie _kinect_UserCreated is al beschikbaar 
            //TODO: Workshop -> Dus koppel _kinect_UserCreated aan het UserCreated event
            //En start de Kinect op
        }

        private void BtnStartClick(object sender, EventArgs e)
        {
            AddItemToListbox(lbMessages, "Button start hit: starting Kinect");
            if (_kinect != null)
            {
                _kinect.StartKinect();
                AddItemToListbox(lbMessages, "Kinect started");
            }
        }

        private void BtnStopClick(object sender, EventArgs e)
        {
            AddItemToListbox(lbMessages, "Button stop hit: stopping Kinect");
            StopKinect();
        }

        private void StopKinect()
        {
            //Als het kinect opbject nog geldig is
            if (_kinect != null)
            {
                //Stop dan de kinect
                _kinect.StopKinect();
                AddItemToListbox(lbMessages, "Kinect stopped");
            }
        }

        #region Kinect events

        private void _kinect_UserCreated(object sender, KinectUserEventArgs e)
        {
            AddItemToListbox(lbMessages, string.Format("Kinect created a new user with id: {0}", e.User.ID));
            //TODO: Workshop -> Stap 2:
            //TODO: Workshop -> Vraag de gebruiker op bij Kinect met behulp van de eventargs
            //TODO: Workshop -> Vergeet niet te controleren of de gebruiker niet null is (ivm multi threading)
            //TODO: Workshop -> En Abonneren je dan op het updated event van de gebruiker
            //TODO: Workshop -> Werk dan alle labels bij in het skelet. Labels beginnen met lbl (zijn al voor gedefinieerd)

            //TODO: Workshop -> Stap 3:
            //TODO: Workshop -> Vul de MyFilter class en MyGesture Class
            //TODO: Workshop -> Koppel ze dan aan elkaar en aan de gebruiker zodat de pipe zoals in de presentatie onstaat
            //TODO: Workshop -> Dus: user.AttachPipeline(myfilter); Nu is de filter aan de gebruiker gekoppeld
            //TODO: Workshop -> filter.AttachPipeline(mygesture); en dan is de gesture aan de filter gekoppeld
            //TODO: Workshop -> zie http://atosorig.in/aonlkinect voor documentatie
            //TODO: Workshop -> Abonneer je dan op het MyGestureDetected event
        }

        private void KinectCameraMessage(object sender, KinectMessageEventArgs e)
        {
            //Alle camera messages worden weergegeven in de onderste listbox
            AddItemToListbox(lbCameraMessages, e.Message);
        }

        #endregion

        #region Helper functions

        private static void UpdateLabel(Control label, string text)
        {
            if (label.InvokeRequired)
            {
                label.Invoke(new MethodInvoker(() => label.Text = text));
            }
            else
            {
                label.Text = text;
            }
        }

        private static void AddItemToListbox(ListBox listbox, string message)
        {
            if (listbox.InvokeRequired)
            {
                listbox.Invoke(new MethodInvoker(() => listbox.Items.Insert(0, message)));
            }
            else
            {
                listbox.Items.Insert(0, message);
            }
        }

        private void KinectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopKinect();
        }

        #endregion
    }
}