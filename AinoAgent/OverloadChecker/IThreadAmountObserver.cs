namespace Aino.Agents.Core.OverloadChecker
{
    /// <summary>
    /// Interface for observing need in sender thread amounts.
    /// </summary>
    interface IThreadAmountObserver
    {
        //Increase sender threads by one.
        void IncreaseThreads();

        //Decrease sender threads by one.
        void DecreaseThreads();
    }
}
