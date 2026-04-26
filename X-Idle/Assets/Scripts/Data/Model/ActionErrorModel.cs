using Data.Enum;

namespace Data.Model
{
    public struct ActionErrorModel
    {
        public ActionErrorModel(ActionErrorType actionErrorType)
        {
            ActionErrorType = actionErrorType;
        }

        public ActionErrorType ActionErrorType { get; }
    }
}