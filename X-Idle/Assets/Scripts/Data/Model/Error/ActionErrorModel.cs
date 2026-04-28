using Data.Enum;
using Data.Model.Popup;

namespace Data.Model.Error
{
    public struct ActionErrorModel
    {
        public ActionErrorModel(ActionErrorType actionErrorType, IShowPopupContext context)
        {
            ActionErrorType = actionErrorType;
            m_context = context;
        }

        private IShowPopupContext m_context;
        public ActionErrorType ActionErrorType { get; }

        public T GetContext<T>() where T : IShowPopupContext
        {
            if (m_context is T data)
                return data;

            return default(T);
        }
    }
}