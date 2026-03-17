using Common.Interface;

namespace Tool.Popup
{
    public class AddTemplateResult<T> where T : ITemplate
    {
        public AddTemplateResult()
        {
            Success = false;
        }

        public AddTemplateResult(T entity)
        {
            Success = true;
            Entity = entity;
        }

        public bool Success { get; }
        public T Entity { get; }
    }
}