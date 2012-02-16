using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using Kinect.SpinToWin.Shapes;
using System.Windows.Media.Animation;

namespace Kinect.SpinToWin.Controls
{
    /// <summary>
    /// Renders a bound dataset as a pie chart
    /// </summary>
    public partial class PiePlotter
    {
        /// <summary>
        /// A list which contains the current piece pieces, where the piece index
        /// is the same as the index of the item within the collection view which 
        /// it represents.
        /// </summary>
        private readonly List<PiePiece> _piePieces = new List<PiePiece>();
        private PiePiece _currentPie = null;
        private double _pieSize = 0;
        private bool _wheelSpinning = true;

        private double _currentAngle = -720;
        private bool DemoRun = true;

        public event EventHandler<WinnerEventArgs> Win;
        

        #region dependency properties

        /// <summary>
        /// The property of the bound object that will be plotted (CLR wrapper)
        /// </summary>
        public String PieSize
        {
            private get { return GetPieSizeProperty(this); }
            set { SetPieSizeProperty(this, value); }
        }

        /// <summary>
        /// The property of the bound object that will be plotted (CLR wrapper)
        /// </summary>
        public String PieName
        {
            private get { return GetPieNameProperty(this); }
            set { SetPieNameProperty(this, value); }
        }

        // PlottedProperty dependency property
        public static readonly DependencyProperty PieSizeProperty =
                       DependencyProperty.RegisterAttached("PieSizeProperty", typeof(String), typeof(PiePlotter),
                       new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.Inherits));

        // PlottedProperty attached property accessors
        public static void SetPieSizeProperty(UIElement element, String value)
        {
            element.SetValue(PieSizeProperty, value);
        }

        public static String GetPieSizeProperty(UIElement element)
        {
            return (String)element.GetValue(PieSizeProperty);
        }

        // PlottedProperty dependency property
        public static readonly DependencyProperty PieNameProperty =
                       DependencyProperty.RegisterAttached("PieNameProperty", typeof(String), typeof(PiePlotter),
                       new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.Inherits));

        // PlottedProperty attached property accessors
        public static void SetPieNameProperty(UIElement element, String value)
        {
            element.SetValue(PieNameProperty, value);
        }
        public static String GetPieNameProperty(UIElement element)
        {
            return (String)element.GetValue(PieNameProperty);
        }

        /// <summary>
        /// A class which selects a color based on the item being rendered.
        /// </summary>
        public IColorSelector ColorSelector
        {
            get { return GetColorSelector(this); }
            set { SetColorSelector(this, value); }
        }

        // ColorSelector dependency property
        public static readonly DependencyProperty ColorSelectorProperty =
                       DependencyProperty.RegisterAttached("ColorSelectorProperty", typeof(IColorSelector), typeof(PiePlotter),
                       new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        // ColorSelector attached property accessors
        public static void SetColorSelector(UIElement element, IColorSelector value)
        {
            element.SetValue(ColorSelectorProperty, value);
        }
        public static IColorSelector GetColorSelector(UIElement element)
        {
            return (IColorSelector)element.GetValue(ColorSelectorProperty);
        }


        /// <summary>
        /// The size of the hole in the centre of circle (as a percentage)
        /// </summary>
        public double HoleSize
        {
            get { return (double)GetValue(HoleSizeProperty); }
            set
            {
                SetValue(HoleSizeProperty, value);
                ConstructPiePieces();
            }
        }

        public static readonly DependencyProperty HoleSizeProperty =
                       DependencyProperty.Register("HoleSize", typeof(double), typeof(PiePlotter), new UIPropertyMetadata(0.0));

       
        #endregion

        public PiePlotter()
        {
            InitializeComponent();
            DataContextChanged += DataContextChangedHandler;
            storyBoard.Completed += new EventHandler(storyBoard_Completed);
        }

        void storyBoard_Completed(object sender, EventArgs e)
        {
            if (DemoRun)
            {
                DemoRun = false;
                _wheelSpinning = false;
                return;
            }
            var calculatedAngle = Math.Abs(_currentAngle%360);
            var piece = _piePieces.FirstOrDefault(p => p.RotationAngle <= calculatedAngle && (p.RotationAngle + _pieSize) >= calculatedAngle);
            PushPie(piece);
            _currentPie = piece;
            if (Win != null)
            {
                Win.Invoke(sender, new WinnerEventArgs(piece.NameValue));
            }
            _wheelSpinning = false;
        }
        
        public void RotatePies(int angle, TimeSpan timeSpan)
        {
            if (_wheelSpinning) return;
            ReturnPie(_currentPie);
            var animation = storyBoard.Children[0] as DoubleAnimation;
            if(animation == null) throw new Exception("No DoubleAnimation found on the PiePlotter");
            _wheelSpinning = true;
            _currentAngle = (animation.To.Value % 360) - angle;

            //Correct angle, because the winner isn't obvious
            var calculatedAngle = Math.Abs(_currentAngle % 360);
            var winnner = _piePieces.FirstOrDefault(p => p.RotationAngle <= calculatedAngle && (p.RotationAngle + _pieSize) >= calculatedAngle);
            if (winnner != null)
            {
                if (winnner.RotationAngle % calculatedAngle < 2)
                {
                    _currentAngle -= 2;
                }
                else if ((winnner.RotationAngle + 18) % calculatedAngle < 2)
                {
                    _currentAngle += 2;
                }
            }

            animation.From = animation.To % 360;
            animation.To = _currentAngle;
            animation.Duration = new Duration(timeSpan);
            
            storyBoard.Begin();
        }

        #region property change handlers

        /// <summary>
        /// Handle changes in the datacontext. When a change occurs handlers are registered for events which
        /// occur when the collection changes or any items within teh collection change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DataContextChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            // handle the events that occur when the bound collection changes
            if (DataContext is INotifyCollectionChanged)
            {
                var observable = (INotifyCollectionChanged)DataContext;
                observable.CollectionChanged += BoundCollectionChanged;
            }

            ConstructPiePieces();
            ObserveBoundCollectionChanges();
        }

        ///// <summary>
        ///// Handles changes to the PlottedProperty property.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void PlottedPropertyChanged(object sender, EventArgs e)
        //{
        //    ConstructPiePieces();
        //}

        #endregion

        #region event handlers

        /// <summary>
        /// Handles the MouseUp event from the individual Pie Pieces
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PiePieceMouseUp(object sender, MouseButtonEventArgs e)
        {
            var piece = sender as PiePiece;
            if (piece == null) return;
            if (_currentPie == piece)
            {
                ReturnPie(_currentPie);
                _currentPie = null;
            }
            else
            {
                ReturnPie(_currentPie);
                PushPie(piece);
                _currentPie = piece;
            }
        }

        void PushPie(PiePiece pie)
        {
            if (pie == null) return;
            var animation = new DoubleAnimation { To = 10, Duration = new Duration(TimeSpan.FromMilliseconds(200)) };
            pie.BeginAnimation(PiePiece.PushOutProperty, animation);
        }

        void ReturnPie(PiePiece pie)
        {
            if (pie == null) return;
            var animation = new DoubleAnimation { To = 0, Duration = new Duration(TimeSpan.FromMilliseconds(200)) };
            pie.BeginAnimation(PiePiece.PushOutProperty, animation);
        }

        /// <summary>
        /// Handles events which are raised when the bound collection changes (i.e. items added/removed)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BoundCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ConstructPiePieces();
            ObserveBoundCollectionChanges();
        }

        /// <summary>
        /// Iterates over the items in the bound collection, adding handlers for PropertyChanged events
        /// </summary>
        private void ObserveBoundCollectionChanges()
        {
            var myCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(DataContext);

            foreach (var observable in myCollectionView.OfType<INotifyPropertyChanged>())
            {
                observable.PropertyChanged += ItemPropertyChanged;
            }
        }


        /// <summary>
        /// Handles events which occur when the properties of bound items change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // if the property which this pie chart represents has changed, re-construct the pie
            if (e.PropertyName.Equals(PieSize))
            {
                ConstructPiePieces();
            }
        }

        #endregion

        private double GetSizePropertyValue(object item)
        {
            var filterPropDesc = TypeDescriptor.GetProperties(item);
            var itemValue = filterPropDesc[PieSize].GetValue(item);

            if (itemValue != null) return (double)itemValue;
            throw new Exception("Object not of type double");
        }

        private string GetNamePropertyValue(object item)
        {
            var filterPropDesc = TypeDescriptor.GetProperties(item);
            var itemValue = filterPropDesc[PieName].GetValue(item);

            if (itemValue != null) return (string)itemValue;
            throw new Exception("Object not of type string");
        }

        /// <summary>
        /// Constructs pie pieces and adds them to the visual tree for this control's canvas
        /// </summary>
        private void ConstructPiePieces()
        {
            var myCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(DataContext);
            if (myCollectionView == null)
                return;

            var halfWidth = Width / 2;
            var innerRadius = halfWidth * HoleSize;            

            // compute the total for the property which is being plotted
            var total = myCollectionView.Cast<object>().Sum(item => GetSizePropertyValue(item));
            _pieSize = 360/total;
            // add the pie pieces
            canvas.Children.Clear();
            _piePieces.Clear();
                        
            double accumulativeAngle=0;
            foreach (var item in myCollectionView)
            {
                var wedgeAngle = GetSizePropertyValue(item) * 360 / total;

                var piece = new PiePiece
                    {
                        Radius = halfWidth,
                        InnerRadius = innerRadius,
                        CentreX = halfWidth,
                        CentreY = halfWidth,
                        WedgeAngle = wedgeAngle,
                        PieceValue = GetSizePropertyValue(item),
                        NameValue = GetNamePropertyValue(item),
                        RotationAngle = accumulativeAngle,
                        Fill = ColorSelector != null ? ColorSelector.SelectBrush(item, myCollectionView.IndexOf(item)) : Brushes.Black,
                        // record the index of the item which this pie slice represents
                        Tag = myCollectionView.IndexOf(item)
                    };

                //piece.ToolTipOpening += new ToolTipEventHandler(PiePieceToolTipOpening);
                piece.MouseUp += PiePieceMouseUp;

                _piePieces.Add(piece);
                canvas.Children.Insert(0, piece);

                accumulativeAngle += wedgeAngle;
            }

            var logo = new Image
            {
                Source = new BitmapImage(new Uri(@"pack://application:,,,/Images/to_small.png",UriKind.RelativeOrAbsolute)),
                Stretch = Stretch.None,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Canvas.SetLeft(logo, 225);
            Canvas.SetTop(logo, 225);
            canvas.Children.Insert(0, logo);
        }
    }
}
