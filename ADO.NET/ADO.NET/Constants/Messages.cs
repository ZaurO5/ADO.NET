using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO.NET.Constants
{
    public static class Messages
    {
        public static void InvalidInputMessage(string title) => Console.WriteLine($"{title} is invalid, please try again");
        public static void InputMessage(string title) => Console.WriteLine($"Input {title}");
        public static void SuccessAddMessage(string title, string value) => Console.WriteLine($"{title} - {value} successfuly added");
        public static void SuccessUpdateMessage(string title, string value) => Console.WriteLine($"{title} - {value} successfuly updated");
        public static void SuccessDeleteMessage(string title, string value) => Console.WriteLine($"{title} - {value} successfuly deleted");
        public static void ErrorOccuredMessage() => Console.WriteLine("Error occured. Please try again.");
        public static void AlreadyExistMessage(string title, string value) => Console.WriteLine($"{title} - {value} is already exist");
        public static void PrintMessage(string title, string value) => Console.WriteLine($"{title} - {value}");  
        public static void NotFoundMessage(string title, string value) => Console.WriteLine($"{title} - {value} not found");
        public static void PrintWantToChangeMessage(string title) => Console.WriteLine($"Do you want to change {title}? Y or N");
    
    }
}
