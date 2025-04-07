using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worker.Helpers
{
	public static class RetryHelper
	{
		public static async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> action, int maxRetries, TimeSpan retryDelay)
		{
			int attempt = 0;

			while (attempt < maxRetries)
			{
				try
				{
					return await action();
				}
				catch (Exception ex) when (ex.Message.Contains("insufficient_quota") || ex.Message.Contains("HTTP 429"))
				{
					attempt++;
					if (attempt >= maxRetries) throw;

					await Task.Delay(retryDelay);
					retryDelay *= 2;
				}
			}

			throw new Exception("Retry limit reached.");
		}
	}
}
