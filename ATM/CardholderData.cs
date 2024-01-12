using NLog;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading;

namespace BANKING_APPLICATION
{
    public class CardholderData
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public CardDetails cardDetails { get; set; }
        public string pinCode { get; set; }
        public Transaction[] transactionHistory { get; set; }
    }

    public class BankingApplication
    {
        public static CardholderData Validation()
        {
            string jsonFilePath = "C:\\Users\\george.chkhaidze\\source\\repos\\ATM\\ATM\\cardInfo.json";
            try
            {
                CardholderData[] userData = LoadUserData(jsonFilePath);

                if (userData != null)
                {
                    CardholderData validatedUser = ValidateCardInformation(userData);

                    if (validatedUser != null)
                    {
                        Console.WriteLine($"\nHello {validatedUser.firstName} {validatedUser.lastName}: \n");
                        return validatedUser; // Exit the program after successful validation
                    }

                    Console.WriteLine("Too many unsuccessful attempts! Wait for 30 seconds and try again.");
                    Thread.Sleep(30000);
                    Validation();
                }
            }
            catch (Exception ex)
            {
                // get a Logger object and log exception here using NLog. 
                // this will use the "fileLogger" logger from our NLog.config file
                Logger logger = LogManager.GetLogger("fileLogger");

                // add custom message and pass in the exception
                logger.Error(ex, $"Error: {ex.Message}");
            }

            return null;
        }

        static CardholderData[] LoadUserData(string jsonFilePath)
        {
            try
            {
                if (File.Exists(jsonFilePath))
                {
                    string jsonContent = File.ReadAllText(jsonFilePath);
                    return JsonConvert.DeserializeObject<CardholderData[]>(jsonContent);
                }

                Console.WriteLine("File not found: " + jsonFilePath);
            }
            catch (Exception ex)
            {
                // get a Logger object and log exception here using NLog. 
                // this will use the "fileLogger" logger from our NLog.config file
                Logger logger = LogManager.GetLogger("fileLogger");

                // add custom message and pass in the exception
                logger.Error(ex, $"Error: {ex.Message}");
            }
            return null;
        }

        static CardholderData ValidateCardInformation(CardholderData[] userData)
        {
            const int maxAttempts = 3;
            try
            {
                for (int attempts = 0; attempts < maxAttempts; attempts++)
                {
                    Console.Write("Enter your card number: ");
                    string cardNumber = Console.ReadLine();

                    Console.Write("Enter your CVC: ");
                    string cvc = Console.ReadLine();

                    Console.Write("Enter your expiration date (MM/YY): ");
                    string expirationDate = Console.ReadLine();

                    foreach (var user in userData)
                    {
                        if (IsValidCardInformation(user, cardNumber, cvc, expirationDate) && ValidatePIN(user))
                        {
                            return user;
                        }
                    }

                    Console.WriteLine("Invalid card information. Please try again.");

                    Console.WriteLine(); // Add an empty line for clarity
                }
            }
            catch (Exception ex)
            {
                // get a Logger object and log exception here using NLog. 
                // this will use the "fileLogger" logger from our NLog.config file
                Logger logger = LogManager.GetLogger("fileLogger");

                // add custom message and pass in the exception
                logger.Error(ex, $"Error: {ex.Message}");
            }
            return null;
        }

        static bool IsValidCardInformation(CardholderData data, string cardNumber, string cvc, string expirationDate)
        {
            return cardNumber == data.cardDetails.cardNumber &&
                   cvc == data.cardDetails.CVC &&
                   expirationDate == data.cardDetails.expirationDate;
        }

        static bool ValidatePIN(CardholderData data)
        {
            const int maxPinAttempts = 3;
            try
            {
                for (int pinAttempts = 0; pinAttempts < maxPinAttempts; pinAttempts++)
                {
                    Console.Write("Enter your PIN: ");
                    string enteredPin = Console.ReadLine();

                    if (IsValidPIN(data, enteredPin))
                    {
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Incorrect PIN. Please try again.");
                    }
                }
            }
            catch (Exception ex)
            {
                // get a Logger object and log exception here using NLog. 
                // this will use the "fileLogger" logger from our NLog.config file
                Logger logger = LogManager.GetLogger("fileLogger");

                // add custom message and pass in the exception
                logger.Error(ex, $"Error: {ex.Message}");
            }

            return false;
        }

        static bool IsValidPIN(CardholderData data, string enteredPin)
        {
            return enteredPin == data.pinCode;
        }
        public static void Menu(CardholderData user)
        {
            Console.WriteLine("1. Check Deposit");
            Console.WriteLine("2. Get Amount");
            Console.WriteLine("3. Get Last 5 Transactions");
            Console.WriteLine("4. Add Amount");
            Console.WriteLine("5. Change Pin");
            Console.WriteLine("6. Change Amount");
            Console.WriteLine("7. Log Out\n");

            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    CheckDeposit(user);
                    Menu(user);
                    break;
                case "2":
                    getAmount(user);
                    var newUser = Validation();
                    Menu(newUser);
                    break;
                case "3":
                    GetTransactionHistory(user);
                    Menu(user);
                    break;
                case "4":
                    AddAmount(user);
                    var newUser1 = Validation();
                    Menu(newUser1);
                    break;
                case "5":
                    ChangePin(user);
                    Menu(user);
                    break;
                case "6":
                    ChangeAmount(user);
                    Menu(user);
                    break;
                case "7":
                    var newUser2 = Validation();
                    Menu(newUser2);
                    break;
                default:
                    Console.WriteLine("Invalid input \n");
                    Menu(user);
                    break;
            }
        }
        static void writejsontransaction(CardholderData data, Transaction transaction, string type)
        {
            try
            {
                DateTime currentUtcTime = DateTime.UtcNow;
                string formattedTime = currentUtcTime.ToString("yyyy-MM-ddTHH:mm:ssZ");

                // Assuming you have a CardholderData object with new transaction data
                CardholderData newData = new CardholderData
                {
                    firstName = data.firstName,
                    lastName = data.lastName,
                    cardDetails = new CardDetails
                    {
                        cardNumber = data.cardDetails.cardNumber,
                        expirationDate = data.cardDetails.expirationDate,
                        CVC = data.cardDetails.CVC
                    },
                    pinCode = data.pinCode,
                    transactionHistory = new Transaction[]
                    {
                    new Transaction
                    {
                    transactionDate = DateTime.UtcNow,
                    transactionType = type,
                    amount = transaction.amount,
                    amountGEL = transaction.amountGEL,
                    amountUSD = transaction.amountUSD,
                    amountEUR = transaction.amountEUR
                    }
                    }
                };

                // Specify the file path
                var filePath = "C:\\Users\\george.chkhaidze\\source\\repos\\ATM\\ATM\\cardInfo.json";

                // Read existing data from the file
                string jsonContent = File.ReadAllText(filePath);
                CardholderData[] existingData = JsonConvert.DeserializeObject<CardholderData[]>(jsonContent);

                // Find the cardholder data to update
                CardholderData existingCardholder = Array.Find(existingData, card => card.cardDetails.cardNumber == newData.cardDetails.cardNumber);

                if (existingCardholder != null)
                {
                    existingCardholder.pinCode = newData.pinCode;
                    // Convert array to a list, add the new transaction, and convert back to an array
                    List<Transaction> updatedTransactions = new List<Transaction>(existingCardholder.transactionHistory);
                    updatedTransactions.InsertRange(0, newData.transactionHistory);
                    existingCardholder.transactionHistory = updatedTransactions.ToArray();
                }

                // Write the updated data back to the file
                string updatedJsonContent = JsonConvert.SerializeObject(existingData, Formatting.Indented);
                File.WriteAllText(filePath, updatedJsonContent);
            }
            catch (Exception ex)
            {
                // get a Logger object and log exception here using NLog. 
                // this will use the "fileLogger" logger from our NLog.config file
                Logger logger = LogManager.GetLogger("fileLogger");

                // add custom message and pass in the exception
                logger.Error(ex, $"Error: {ex.Message}");
            }
        }
        static Transaction CheckDeposit(CardholderData data)
        {
            Console.WriteLine("\nAmount is");
            try
            {
                foreach (var transaction in data.transactionHistory)
                {

                    Console.WriteLine($"Amount (GEL): {transaction.amountGEL}");
                    Console.WriteLine($"Amount (USD): {transaction.amountUSD}");
                    Console.WriteLine($"Amount (EUR): {transaction.amountEUR}");
                    Console.WriteLine();

                    writejsontransaction(data, transaction, "Deposit Check");
                    return transaction; // Exit the loop after displaying Balance

                }
            }
            catch (Exception ex)
            {
                // get a Logger object and log exception here using NLog. 
                // this will use the "fileLogger" logger from our NLog.config file
                Logger logger = LogManager.GetLogger("fileLogger");

                // add custom message and pass in the exception
                logger.Error(ex, $"Error: {ex.Message}");
            }
            return null;

        }
        static void getAmount(CardholderData data)
        {
            try
            {
                Console.WriteLine("Please enter the amount you want to get:");
                decimal.TryParse(Console.ReadLine(), out decimal amount);
                if (amount > 0)
                {
                    foreach (var transaction in data.transactionHistory)
                    {
                        transaction.amount = amount;
                        transaction.amountGEL = transaction.amountGEL - amount;
                        writejsontransaction(data, transaction, "Get Amount");
                        Console.WriteLine("Do you want a receipt? Y/N");
                        var receipt = Console.ReadLine();
                        if (receipt == "Y" || receipt == "y")
                        {
                            CheckDeposit(data);
                            return;
                        }
                        else if (receipt == "N" || receipt == "n")
                        {
                            Console.WriteLine("Thanks for your effort in saving the environment");
                            return;
                        }
                        else
                            return;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Amunt has been entered!");
                    getAmount(data);
                }
            }
            catch (Exception ex)
            {
                // get a Logger object and log exception here using NLog. 
                // this will use the "fileLogger" logger from our NLog.config file
                Logger logger = LogManager.GetLogger("fileLogger");

                // add custom message and pass in the exception
                logger.Error(ex, $"Error: {ex.Message}");
            }

        }
        static void GetTransactionHistory(CardholderData user)
        {
            Console.WriteLine("\nTransaction History:\n");

            var counter = 0;
            try
            {
                foreach (var transaction in user.transactionHistory)
                {
                    Console.WriteLine($"Transaction Date: {transaction.transactionDate}");
                    Console.WriteLine($"Transaction Type: {transaction.transactionType}");
                    Console.WriteLine($"Amount : {transaction.amount}");
                    Console.WriteLine($"Amount (GEL): {transaction.amountGEL}");
                    Console.WriteLine($"Amount (USD): {transaction.amountUSD}");
                    Console.WriteLine($"Amount (EUR): {transaction.amountEUR}");
                    Console.WriteLine();
                    counter++;

                    if (counter == 5)
                    {
                        writejsontransaction(user, transaction, "Get Transactions History");
                        return; // Exit the loop after displaying 5 transactions
                    }
                }
            }
            catch (Exception ex)
            {
                // get a Logger object and log exception here using NLog. 
                // this will use the "fileLogger" logger from our NLog.config file
                Logger logger = LogManager.GetLogger("fileLogger");

                // add custom message and pass in the exception
                logger.Error(ex, $"Error: {ex.Message}");
            }
        }
        static void AddAmount(CardholderData data)
        {
            Console.WriteLine("Please enter the amount you want to add:");
            try
            {
                decimal.TryParse(Console.ReadLine(), out decimal amount);
                amount = Math.Round(amount, 2);
                if (amount > 0)
                {
                    foreach (var transaction in data.transactionHistory)
                    {
                        transaction.amount = amount;
                        transaction.amountGEL = transaction.amountGEL + amount;
                        writejsontransaction(data, transaction, "Fill Amount");
                        Console.WriteLine($"The amount of {amount} has succesfuli addit to your GEL account\n");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Amunt has been entered!");
                    AddAmount(data);
                }
                Console.WriteLine("Do you want a receipt? Y/N");
                var receipt = Console.ReadLine();
                if (receipt == "Y" || receipt == "y")
                {
                    CheckDeposit(data);
                    return;
                }
                else if (receipt == "N" || receipt == "n")
                {
                    Console.WriteLine("Thanks for your effort in saving the environment");
                    return;
                }
                else
                    return;
            }
            catch (Exception ex)
            {
                // get a Logger object and log exception here using NLog. 
                // this will use the "fileLogger" logger from our NLog.config file
                Logger logger = LogManager.GetLogger("fileLogger");

                // add custom message and pass in the exception
                logger.Error(ex, $"Error: {ex.Message}");
            }
        }
        static void ChangePin(CardholderData data)
        {
            Console.WriteLine("Please enter 4 digit new Pin Code");
            try
            {
                int.TryParse(Console.ReadLine(), out int pinCode);
                if (pinCode.ToString().Length != 4)
                {
                    ChangePin(data);
                }
                Console.WriteLine("Please Reenter new Pin Code");
                int.TryParse(Console.ReadLine(), out int pinCode1);
                if (pinCode == pinCode1)
                {
                    data.pinCode = pinCode.ToString();
                    foreach (var transaction in data.transactionHistory)
                    {
                        writejsontransaction(data, transaction, "Change Pin");
                        Console.WriteLine("\nPIN Code changed successfully!\n");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Pin Code has been entered!");
                    ChangePin(data);
                }
            }
            catch (Exception ex)
            {
                // get a Logger object and log exception here using NLog. 
                // this will use the "fileLogger" logger from our NLog.config file
                Logger logger = LogManager.GetLogger("fileLogger");

                // add custom message and pass in the exception
                logger.Error(ex, $"Error: {ex.Message}");
            }
            return;
        }
        static void ChangeAmount(CardholderData data)
        {
            Console.WriteLine("Please enter the number of Currency you want to convert:");
            Console.WriteLine("1. GEL to USD");
            Console.WriteLine("2. GEL to EUR");
            Console.WriteLine("3. USD to GEL");
            Console.WriteLine("4. EUR to GEL");
            Console.WriteLine("5. USD to EUR");
            Console.WriteLine("6. EUR to USD");
            Console.WriteLine("7. Back to Menu");
            var num = Console.ReadLine();
            var exchangeRateUSD = 2.7m;
            var exchangeRateEUR = 3.00m;
            var exchangeRateUSDtoEUR = 0.91m;
            var exchangeRateEURtoUSD = 1.10m;
            try
            {
                switch (num)
                {
                    case "1":
                        Console.WriteLine("Please enter the amount of money to convert GEL into USD");
                        decimal.TryParse(Console.ReadLine(), out decimal exchange);
                        exchange = Math.Round(exchange, 2);
                        if (exchange > 0)
                        {
                            foreach (var transaction in data.transactionHistory)
                            {
                                if (exchange < transaction.amountGEL)
                                {
                                    transaction.amount = exchange;
                                    transaction.amountGEL = transaction.amountGEL - exchange;
                                    transaction.amountUSD = Math.Round(transaction.amountUSD + exchange / exchangeRateUSD, 2);
                                    writejsontransaction(data, transaction, "Change Amount");
                                    Console.WriteLine("\nThe amount has succesfuly converted\n");
                                    return;
                                }
                                else
                                    Console.WriteLine("\nThe amount you entered is more than the amount available in your account\n");
                                ChangeAmount(data);
                                return;
                            }
                        }
                        Console.WriteLine("\ninvalid input\n");
                        ChangeAmount(data);
                        return;
                    case "2":
                        Console.WriteLine("Please enter the amount of money to convert GEL into EUR");
                        decimal.TryParse(Console.ReadLine(), out decimal exchange1);
                        exchange1 = Math.Round(exchange1, 2);
                        if (exchange1 > 0)
                        {
                            foreach (var transaction in data.transactionHistory)
                            {
                                if (exchange1 < transaction.amountGEL)
                                {
                                    transaction.amount = exchange1;
                                    transaction.amountGEL = transaction.amountGEL - exchange1;
                                    transaction.amountEUR = Math.Round(transaction.amountEUR + exchange1 / exchangeRateEUR, 2);
                                    writejsontransaction(data, transaction, "Change Amount");
                                    Console.WriteLine("\nThe amount has succesfuly converted\n");
                                    return;
                                }
                                else
                                    Console.WriteLine("\nThe amount you entered is more than the amount available in your account\n");
                                ChangeAmount(data);
                                return;

                            }
                        }
                        Console.WriteLine("\ninvalid input\n");
                        ChangeAmount(data);
                        return;
                    case "3":
                        Console.WriteLine("Please enter the amount of money to convert USD into GEL");
                        decimal.TryParse(Console.ReadLine(), out decimal exchange2);
                        exchange2 = Math.Round(exchange2, 2);
                        if (exchange2 > 0)
                        {
                            foreach (var transaction in data.transactionHistory)
                            {
                                if (exchange2 < transaction.amountUSD)
                                {
                                    transaction.amount = exchange2;
                                    transaction.amountUSD = transaction.amountUSD - exchange2;
                                    transaction.amountGEL = Math.Round(transaction.amountGEL + exchange2 * exchangeRateUSD,2);
                                    writejsontransaction(data, transaction, "Change Amount");
                                    Console.WriteLine("\nThe amount has succesfuly converted\n");
                                    return;
                                }
                                else
                                    Console.WriteLine("\nThe amount you entered is more than the amount available in your account\n");
                                ChangeAmount(data);
                                return;
                            }
                        }
                        Console.WriteLine("\ninvalid input\n");
                        ChangeAmount(data);
                        return;
                    case "4":
                        Console.WriteLine("Please enter the amount of money to convert EUR into GEL");
                        decimal.TryParse(Console.ReadLine(), out decimal exchange3);
                        exchange3 = Math.Round(exchange3, 2);
                        if (exchange3 > 0)
                        {
                            foreach (var transaction in data.transactionHistory)
                            {
                                if (exchange3 < transaction.amountEUR)
                                {
                                    transaction.amount = exchange3;
                                    transaction.amountEUR = transaction.amountEUR - exchange3;
                                    transaction.amountGEL = Math.Round(transaction.amountGEL + exchange3 * exchangeRateEUR, 2);
                                    writejsontransaction(data, transaction, "Change Amount");
                                    Console.WriteLine("\nThe amount has succesfuly converted\n");
                                    return;
                                }
                                else
                                    Console.WriteLine("\nThe amount you entered is more than the amount available in your account\n");
                                ChangeAmount(data);
                                return;
                            }
                        }
                        Console.WriteLine("\ninvalid input\n");
                        ChangeAmount(data);
                        return;
                    case "5":
                        Console.WriteLine("Please enter the amount of money to convert USD into EUR");
                        decimal.TryParse(Console.ReadLine(), out decimal exchange4);
                        exchange4 = Math.Round(exchange4, 2);
                        if (exchange4 > 0)
                        {
                            foreach (var transaction in data.transactionHistory)
                            {
                                if (exchange4 < transaction.amountUSD)
                                {
                                    transaction.amount = exchange4;
                                    transaction.amountUSD = transaction.amountUSD - exchange4;
                                    transaction.amountEUR = Math.Round(transaction.amountEUR + exchange4 * exchangeRateUSDtoEUR, 2);
                                    writejsontransaction(data, transaction, "Change Amount");
                                    Console.WriteLine("\nThe amount has succesfuly converted\n");
                                    return;
                                }
                                else
                                    Console.WriteLine("\nThe amount you entered is more than the amount available in your account\n");
                                ChangeAmount(data);
                                return;
                            }
                        }
                        Console.WriteLine("\ninvalid input\n");
                        ChangeAmount(data);
                        return;
                    case "6":
                        Console.WriteLine("Please enter the amount of money to convert EUR into USD");
                        decimal.TryParse(Console.ReadLine(), out decimal exchange5);
                        exchange5 = Math.Round(exchange5, 2);
                        if (exchange5 > 0)
                        {
                            foreach (var transaction in data.transactionHistory)
                            {
                                if (exchange5 < transaction.amountEUR)
                                {
                                    transaction.amount = exchange5;
                                    transaction.amountEUR = transaction.amountEUR - exchange5;
                                    transaction.amountUSD = Math.Round(transaction.amountUSD + exchange5 * exchangeRateEURtoUSD, 2);
                                    writejsontransaction(data, transaction, "Change Amount");
                                    Console.WriteLine("\nThe amount has succesfuly converted\n");
                                    return;
                                }
                                else
                                    Console.WriteLine("\nThe amount you entered is more than the amount available in your account\n");
                                ChangeAmount(data);
                                return;
                            }
                        }
                        Console.WriteLine("\ninvalid input\n");
                        ChangeAmount(data);
                        return;
                    case "7":
                        Menu(data);
                        return;
                    default:
                        Console.WriteLine("\nWrong input\n");
                        ChangeAmount(data);
                        break;

                }
            }
            catch (Exception ex)
            {
                // get a Logger object and log exception here using NLog. 
                // this will use the "fileLogger" logger from our NLog.config file
                Logger logger = LogManager.GetLogger("fileLogger");

                // add custom message and pass in the exception
                logger.Error(ex, $"Error: {ex.Message}");
            }
        }

    }
}