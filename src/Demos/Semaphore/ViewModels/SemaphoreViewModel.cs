using System;
using GalaSoft.MvvmLight;
using Kinect.Semaphore.Models;

namespace Kinect.Semaphore.ViewModels
{
    public class SemaphoreViewModel : ViewModelBase
    {
        private SemaphoreGame _game;

        private System.Windows.Visibility _winner;
        
        private bool _isRunning;
        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                if (value != _isRunning)
                {
                    _isRunning = value;
                    RaisePropertyChanged("IsRunning");
                }
            }
        }

        public SemaphoreImage Current
        {
            get { return _game.Current; }
        }

        public SemaphoreImage Next
        {
            get { return _game.Next; }
        }

        public string Sentence
        {
            get { return _game.GetTodoSentence(); }
        }

        public System.Windows.Visibility Winner
        {
            get { return _winner; }
            set
            {
                if (value != _winner)
                {
                    _winner = value;
                    RaisePropertyChanged("Winner");
                }
            }
        }

        public SemaphoreViewModel()
        {
            _game = SemaphoreGames.Instance.GetNextGame();
            _winner = System.Windows.Visibility.Hidden;
            _game.Start += _game_Start;
            _game.Updated += _game_Updated;
            _game.Finished += _game_Finished;
        }

        private void _game_Start(object sender, EventArgs e)
        {
            IsRunning = true;
            RaisePropertyChanged("Sentence");
        }

        private void _game_Updated(object sender, EventArgs e)
        {
            RaisePropertyChanged("Sentence");
            RaisePropertyChanged("Current");
            RaisePropertyChanged("Next");
        }

        private void _game_Finished(object sender, EventArgs e)
        {
            IsRunning = false;
            Winner = System.Windows.Visibility.Visible;
        }

        public void SemaphoreDetected(Core.Gestures.Model.Semaphore Semaphore)
        {
            if (_game != null)
            {
                _game.SemaphoreDetected(Semaphore);
            }
        }
    }
}
