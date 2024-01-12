using System;
using NLog;
using BANKING_APPLICATION;


class Program
{
    static void Main()
    {
        try
        {
            BankingApplication bankingApp = new BankingApplication();
            CardholderData validatedUser = BankingApplication.Validation();
            BankingApplication.Menu(validatedUser);
        }
        catch (Exception ex)
        {
            // get a Logger object and log exception here using NLog. 
            // this will use the "fileLogger" logger from NLog.config file
            Logger logger = LogManager.GetLogger("fileLogger");

            // add custom message and pass in the exception
            logger.Error(ex, $"Error: {ex.Message}");
        }
    }
}
