// Author: John Kudela

/*
 * Project 2 - Secure Messaging
 *
 * Allows for encoding/decoding of private keys for
 * communication via the RIT CS server
 */

namespace SecureMessaging
{
    public class SecureMessaging
    {
        private static readonly ProgramManager ProgramManager = new();

        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintUsageError("<option>", "<other arguments>");
                return;
            }

            switch (args[0])
            {
                case "keyGen":
                    int keysize;
                    if (args.Length != 2)
                    {
                        PrintUsageError(args[0], "<keysize>");
                    }
                    else if (!int.TryParse(args[1], out keysize) || keysize < 1)
                    {
                        Console.WriteLine("keysize must be a positive integer");
                    }
                    else
                    {
                        ProgramManager.KeyGen(keysize);
                        // process this
                    }

                    break;
                case "sendKey":
                    if (args.Length != 2)
                    {
                        PrintUsageError(args[0], "<email>");
                    }
                    else
                    {
                        ProgramManager.SendKey(args[1]);
                    }

                    break;
                case "getKey":
                    if (args.Length != 2)
                    {
                        PrintUsageError(args[0], "<email>");
                    }
                    else
                    {
                        ProgramManager.GetKey(args[1]);
                    }

                    break;
                case "sendMsg":
                    if (args.Length != 3)
                    {
                        PrintUsageError(args[0], "<email> <message>");
                    }
                    else
                    {
                        ProgramManager.SendMsg(args[1], args[2]);
                    }

                    break;
                case "getMsg":
                    if (args.Length != 2)
                    {
                        PrintUsageError(args[0], "<email>");
                    }
                    else
                    {
                        ProgramManager.GetMsg(args[1]);
                    }

                    break;
                default:
                    PrintUsageError("<option>", "<other arguments>");
                    break;
            }
        }

        // Helper method to print usage errors to console
        private static void PrintUsageError(string option, string args)
        {
            Console.WriteLine($"Usage: dotnet run {option} {args}");
        }
    }
}