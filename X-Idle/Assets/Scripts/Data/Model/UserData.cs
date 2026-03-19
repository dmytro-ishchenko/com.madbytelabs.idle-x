namespace Data.Model
{
    internal class UserData
    {
        public UserData()
        {
            UserBuildingsData = new UserBuildingsData();
        }

        public UserBuildingsData UserBuildingsData { get; }
    }
}