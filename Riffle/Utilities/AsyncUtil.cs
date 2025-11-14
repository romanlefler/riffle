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

        public static async Task<bool> TaskDelayDelta(int ms, DateTime firstFrame, CancellationToken tok)
        {
            try
            {
                int time = ms - (DateTime.Now - firstFrame).Milliseconds;
                if(time > 0) await Task.Delay(time, tok);
                return true;
            }
            catch(OperationCanceledException)
            {
                return false;
            }
        }

        public static async Task<bool> TaskDelayDelta(int ms, DateTime firstFrame)
        {
            try
            {
                int time = ms - (DateTime.Now - firstFrame).Milliseconds;
                if(time > 0) await Task.Delay(time);
                return true;
            }
            catch(OperationCanceledException)
            {
                return false;
            }
        }
    }
}