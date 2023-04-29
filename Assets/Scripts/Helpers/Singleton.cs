namespace Helpers
{
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        #region Fields

        private static T _instance;

        #endregion

        #region Properties

        public static T Instance => _instance ?? (_instance = new T());

        #endregion

        #region Private Constructor

        protected Singleton() { }

        #endregion
    }
}