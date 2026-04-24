using Data.Model;
using Data.Processor.Context;

namespace Data.Processor
{
    internal class UserDataProcessor
    {
        internal UserDataProcessor()
        {
            m_energyProcessor = new EnergyProcessor();
            m_foodProcessor = new FoodProcessor();
            m_scrapProcessor = new ScrapProcessor();
            m_waterProcessor = new WaterProcessor();
        }

        IProcessor m_energyProcessor;
        IProcessor m_foodProcessor;
        IProcessor m_scrapProcessor;
        IProcessor m_waterProcessor;

        public void StartProcessing(UserData userData)
        {
            m_energyProcessor.StartProcess(new EnergyProcessContext());
        }

        public void StopProcessing()
        {
            m_energyProcessor.StopProcess();
        }
    }
}