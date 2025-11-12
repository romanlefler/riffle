namespace Riffle.Utilities
{
    public static class AsyncUtil
    {
        public static async Task<bool> TaskDelay(int ms, CancellationToken tok)
        {
            try
            {
                await Task.Delay(ms, tok);
                return true;
            }
            catch(OperationCanceledException)
            {
                return false;
            }
        }
    }
}