namespace Aino.Agents.Core
{
    /// <summary>
    /// Response from Aino.io API
    /// </summary>
    public interface IApiResponse
    {
        int GetStatus();
        string GetPayload();
    }
}
