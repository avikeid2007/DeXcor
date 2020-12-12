using Windows.UI.Input.Inking;

namespace DeXcor.EventHandlers.Ink
{
    public class RemoveEventArgs
    {
        public InkStroke RemovedStroke { get; set; }

        public RemoveEventArgs(InkStroke removedStroke)
        {
            RemovedStroke = removedStroke;
        }
    }
}
