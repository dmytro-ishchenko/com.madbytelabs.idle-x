using Common.Model;
using Common.Pattern.BobbleEvent;

namespace Common.Events
{
    public struct SelectContentEventArgs: IEventArgs
    {
        public SelectContentEventArgs(ContentModel contentModel)
        {
            ContentModel = contentModel;
        }

        public ContentModel ContentModel { get; }
    }
}