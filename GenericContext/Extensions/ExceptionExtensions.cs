using System;

namespace GenericContext.Extensions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Looks up for all exception messages, including InnerExceptions.
        /// </summary>
        /// <param name="source">Exception class.</param>
        /// <returns>Returns a new line separated string with all exception messages found.</returns>
        public static string GetMessages(this Exception source)
        {
            if (source == null)
            {
                throw new Exception($"Exception not found for {source.ToString()}.");
            }
            else
            {
                if (string.IsNullOrEmpty(source.Message) == true)
                {
                    throw new Exception($"No exception message found for {source.ToString()}.");
                }
                else
                {
                    var mensagem = source.Message;

                    if (source.InnerException != null)
                    {
                        mensagem += Environment.NewLine + InnerExceptionMessage(source.InnerException);
                    }

                    return mensagem;
                }
            }
        }

        private static string InnerExceptionMessage(Exception exception)
        {
            if (exception.InnerException == null)
            {
                return exception.Message;
            }
            else
            {
                return exception.Message + Environment.NewLine + InnerExceptionMessage(exception.InnerException);
            }
        }
    }
}
