using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Kinect.Semaphore.Models
{
    public sealed class SemaphoreGames
    {
        private int _currentGame;
        private static object _syncRoot = new object();
        private static readonly SemaphoreGames _instance = new SemaphoreGames();

        public ObservableCollection<SemaphoreGame> Games { get; private set; }

        private SemaphoreGames()
        {
            this.Initialize();
        }

        public static SemaphoreGames Instance
        {
            get { return _instance; }
        }

        private void Initialize()
        {
            this._currentGame = 0;
            this.Games = new ObservableCollection<SemaphoreGame>();
            this.GenerateGames();
        }

        public void ResetGameCounter()
        {
            this._currentGame = 0;
        }

        public SemaphoreGame GetRandomGame()
        {
            if (this.Games == null || this.Games.Count == 0)
            {
                return null;
            }
            return this.Games[new Random().Next(this.Games.Count)].CreateCopy();
        }

        public SemaphoreGame GetNextGame()
        {
            if (this.Games == null || this.Games.Count == 0)
            {
                return null;
            }
            var game = this.Games[_currentGame].CreateCopy();
            _currentGame = _currentGame + 1 == this.Games.Count ? 0 : _currentGame + 1;

            return game;
        }

        public SemaphoreGame GetGame(int id)
        {
            if (id >= this.Games.Count)
            {
                return null;
            }
            return this.Games[id].CreateCopy();
        }

        private void GenerateGames()
        {
            ////TODO: Read XML File
            this.Games.Clear();
            using (StreamReader sr = new StreamReader(Properties.Settings.Default.SemaphoreGamesFilePath))
            {
                var gameline = sr.ReadLine();
                while (gameline != null)
                {
                    var game = new SemaphoreGame();
                    foreach (char semaphore in gameline)
                    {
                        game.Semaphores.Add(new SemaphoreImage(semaphore));
                    }

                    this.Games.Add(game);
                    gameline = sr.ReadLine();
                }
            }
        }
    }
}
