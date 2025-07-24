namespace Core
{
    /// <summary>
    /// Определяем все типы событий, которые будут передаваться через шину
    /// Использование структур помогает избежать мусора в памяти
    /// </summary>
    public static class GameEvents
    {
        // Геймплейные события
        public struct ShapeSortedCorrectly
        {
            public int ScoreToAdd;
        }

        public struct ShapeSortedIncorrectly
        {
        }

        public struct ShapeReachedDeathZone
        {
        }

        public struct AllShapesProcessed
        {
        }

        // События состояния игры
        public struct GameWon
        {
            public int FinalScore;
        }

        public struct GameLost
        {
        }

        // События для обновления UI
        public struct ScoreUpdated
        {
            public int NewScore;
        }

        public struct HealthUpdated
        {
            public int NewHealth;
        }
    }
}