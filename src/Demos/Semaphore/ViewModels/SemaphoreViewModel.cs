using System;
using System.Windows;
using GalaSoft.MvvmLight;
using Kinect.Semaphore.Models;

namespace Kinect.Semaphore.ViewModels
{
    public class SemaphoreViewModel : ViewModelBase
    {
        private readonly SemaphoreGame _game;

        private bool _isRunning;
        private Visibility _winner;

        public SemaphoreViewModel()
        {
            _game = SemaphoreGames.Instance.GetNextGame();
            _winner = Visibility.Hidden;
            _game.Start += _game_Start;
            _game.Updated += _game_Updated;
            _game.Finished += _game_Finished;
        }

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

        public Visibility Winner
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
            Winner = Visibility.Visible;
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