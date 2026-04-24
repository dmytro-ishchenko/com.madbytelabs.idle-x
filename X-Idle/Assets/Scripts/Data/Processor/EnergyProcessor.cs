namespace Data.Processor
{
    internal class EnergyProcessor : Processor
    {
        public override void StartProcess<T>(T context)
        {
            StartProcess();
        }

        public override void UpdateContext<T>(T context)
        {
        }
    }
}