namespace tsf4g_tdr_csharp
{
    using System;

    public class TdrVisualBuf
    {
        private string visualBuf = string.Empty;

        public string getVisualBuf()
        {
            return this.visualBuf;
        }

        public TdrError.ErrorType sprintf(string format, params object[] args)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            string str = string.Empty;
            try
            {
                str = string.Format(format, args);
            }
            catch (ArgumentNullException exception)
            {
                Console.WriteLine("Error: " + exception.Message);
                type = TdrError.ErrorType.TDR_ERR_ARGUMENT_NULL_EXCEPTION;
            }
            catch (FormatException exception2)
            {
                Console.WriteLine("Error: " + exception2.Message);
                type = TdrError.ErrorType.TDR_ERR_INVALID_FORMAT;
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                this.visualBuf = this.visualBuf + str;
            }
            return type;
        }
    }
}

