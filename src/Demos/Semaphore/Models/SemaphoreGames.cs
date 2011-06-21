using System;
using System.Collections.ObjectModel;
using System.IO;
using Kinect.Semaphore.Properties;

namespace Kinect.Semaphore.Models
{
    public sealed class SemaphoreGames
    {
        private static object _syncRoot = new object();
        private static readonly SemaphoreGames _instance = new SemaphoreGames();
        private int _currentGame;

        private SemaphoreGames()
        {
            Initialize();
        }

        public ObservableCollection<SemaphoreGame> Games { get; private set; }

        public static SemaphoreGames Instance
        {
            get { return _instance; }
        }

        private void Initialize()
        {
            _currentGame = 0;
            Games = new ObservableCollection<SemaphoreGame>();
            GenerateGames();
        }

        public void ResetGameCounter()
        {
            _currentGame = 0;
        }

        public SemaphoreGame GetRandomGame()
        {
            if (Games == null || Games.Count == 0)
            {
                return null;
            }
            return Games[new Random().Next(Games.Count)].CreateCopy();
        }

        public SemaphoreGame GetNextGame()
        {
            if (Games == null || Games.Count == 0)
            {
                return null;
            }
            SemaphoreGame game = Games[_currentGame].CreateCopy();
            _currentGame = _currentGame + 1 == Games.Count ? 0 : _currentGame + 1;

            return game;
        }

        public SemaphoreGame GetGame(int id)
        {
            if (id >= Games.Count)
            {
                return null;
            }
            return Games[id].CreateCopy();
        }

        private void GenerateGames()
        {
            ////TODO: Read XML File
            Games.Clear();
            using (var sr = new StreamReader(Settings.Default.SemaphoreGamesFilePath))
            {
                string gameline = sr.ReadLine();
                while (gameline != null)
                {
                    var game = new SemaphoreGame();
                    foreach (char semaphore in gameline)
                    {
                        game.Semaphores.Add(new SemaphoreImage(semaphore));
                    }

                    Games.Add(game);
                    gameline = sr.ReadLine();
                }
            }
        }
    }
}