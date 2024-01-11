namespace TerraTiler2D
{
    public class Singleton<myType>
        where myType : new()
    {
        private static myType instance;
        public static myType GetInstance()
        {
            if (instance == null)
            {
                instance = new myType();
            }
            return instance;
        }

        protected Singleton()
        {
            Initialize();
        }

        protected bool isInitialized = false;
        protected virtual void Initialize()
        {
            isInitialized = true;
        }
    }
}
