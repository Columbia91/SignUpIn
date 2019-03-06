using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace SignUpIn
{
    class Program
    {
        public static List<User> users = new List<User>();
        static void Main(string[] args)
        {
                Console.WriteLine("1) Регистрация" +
                    "\n2) Вход" +
                    "\n3) Выход");
                int choice = int.Parse(Console.ReadLine());

            if (choice == 1)
            {
                User user = new User();

                DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<User>));

                JsonReading(jsonFormatter);

                EnterLogin(user);
                EnterPassword(user);
                ConfirmPassword(user);
                EnterEmail(user);
                EnterPhoneNumber(user);

                users.Add(user);

                using (FileStream fs = new FileStream("people.json", FileMode.OpenOrCreate)) // папка bin -> debug
                {
                    jsonFormatter.WriteObject(fs, users);
                }
            }
            else if (choice == 2)
            {
                User user = new User();
                bool check = true;

                DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<User>));

                JsonReading(jsonFormatter);

                while (check) {
                    Console.Clear();
                    Console.Write("Введите данные: " +
                        "\nЛогин: ");
                    user.Login = Console.ReadLine();
                    Console.Write("Пароль: ");
                    user.Password = HideCharacter();
                    user.Password = user.Password.TrimEnd(user.Password[user.Password.Length - 1]);

                    for (int i = 0; i < users.Count; i++)
                    {
                        if (user.Login == users[i].Login)
                        {
                            if (user.Password != users[i].Password)
                            {
                                Console.WriteLine("\nВы ввели неправильный пароль, нажмите Enter чтобы ввести заново...");
                                Console.ReadKey(); break;
                            }
                            else
                            {
                                check = false; break;
                            }
                        }
                        if (i == users.Count - 1)
                        {
                            Console.WriteLine("\nУказанный Вами логин не существует, нажмите Enter чтобы ввести заново...");
                            Console.ReadKey();
                        }
                    }
                }
            }
            else
            {
                return;
            }
        }

        #region Вывод на консоль
        static void Show(User user, int numb, string stars = "", string stars2 = "")
        {
            for (int i = 0; i < numb; i++)
            {
                switch (i + 1)
                {
                    case 1: Console.Write("Login: " + user.Login); break;
                    case 2: Console.Write("\nPassword: " + stars); break;
                    case 3: Console.Write("\nConfirm Password: " + stars2); break;
                    case 4: Console.Write("\nEmail: " + user.Email); break;
                    case 5: Console.Write("\nPhone number (+XYYYZZZZZZZ): " + user.PhoneNumber); break;
                }
            }
        }
        #endregion

        #region Ввод логина
        static void EnterLogin(User user)
        {
            Console.Clear();
            const int FIELD_NUMBER = 1, MIN_LOGIN_LENGTH = 3;

            Show(user, FIELD_NUMBER);
            user.Login = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(user.Login))
            {
                Console.WriteLine("Логин содержит недопустимые символы, нажмите Enter чтобы ввести заново...");
                user.Login = "";
                Console.ReadKey();
                EnterLogin(user);
            }

            Regex regex = new Regex(@"\W|[А-Яа-я]+");
            MatchCollection matches = regex.Matches(user.Login);
            if (matches.Count > 0)
            {
                Console.WriteLine("Логин содержит недопустимые символы, нажмите Enter чтобы ввести заново...");
                user.Login = "";
                Console.ReadKey();
                EnterLogin(user);
            }
            if (user.Login.Length < MIN_LOGIN_LENGTH)
            {
                Console.WriteLine("Логин слишком короткий, нажмите Enter чтобы ввести заново...");
                user.Login = "";
                Console.ReadKey();
                EnterLogin(user);
            }
            for (int i = 0; i < users.Count; i++)
            {
                if(user.Login == users[i].Login)
                {
                    Console.WriteLine("Пользователь с таким логином уже существует, нажмите Enter чтобы ввести заново...");
                    user.Login = "";
                    Console.ReadKey();
                    EnterLogin(user);
                }
            }
        }
        #endregion

        #region Ввод пароля
        static void EnterPassword(User user)
        {
            Console.Clear();
            const int FIELD_NUMBER_2 = 2;

            Show(user, FIELD_NUMBER_2);
            user.Password = HideCharacter();
            user.Password = user.Password.TrimEnd(user.Password[user.Password.Length - 1]);

            string pattern = @"(?=^.{6,32}$)((?=.*\d)(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$";

            while (true)
            {
                if (user.Password.Length < 6 || user.Password.Length > 32)
                    Console.WriteLine("\nДлина пароля должна быть не меньше 6 символов и не больше 32 символов, нажмите Enter чтобы ввести заново...");
                else if (!(Regex.IsMatch(user.Password, pattern)))
                    Console.WriteLine("\nПароль должен содержать цифровой и спец символы, а также буквы верхнего, нижнего регистра, нажмите Enter чтобы ввести заново...");
                else
                    break;
                user.Password = "";
                Console.ReadKey();
                EnterPassword(user);
            }
        }
        #endregion

        #region Подтверждение пароля
        static void ConfirmPassword(User user)
        {
            Console.Clear();
            const int FIELD_NUMBER_3 = 3;
            string stars = GiveMeStars(user);

            Show(user, FIELD_NUMBER_3, stars);
            user.PasswordCopy = HideCharacter();
            user.PasswordCopy = user.PasswordCopy.TrimEnd(user.PasswordCopy[user.PasswordCopy.Length - 1]);

            if (user.Password != user.PasswordCopy)
            {
                Console.WriteLine("Пароли не совпадают, нажмите Enter чтобы ввести заново...");
                user.PasswordCopy = "";
                Console.ReadKey();
                ConfirmPassword(user);
            }
        }
        #endregion

        #region Ввод почты
        static void EnterEmail(User user)
        {
            Console.Clear();
            const int FIELD_NUMBER_4 = 4;
            string stars = GiveMeStars(user);

            Show(user, FIELD_NUMBER_4, stars, stars);
            user.Email = Console.ReadLine();

            string pattern = @"^[-\w.]+@([A-z0-9][-A-z0-9]+\.)+[A-z]{2,4}$";

            if (!(Regex.IsMatch(user.Email, pattern)))
            {
                Console.WriteLine("Некорректный email, нажмите Enter чтобы ввести заново...");
                user.Email = "";
                Console.ReadKey();
                EnterEmail(user);
            }
            for (int i = 0; i < users.Count; i++)
            {
                if (user.Email == users[i].Email)
                {
                    Console.WriteLine("Данный почтовый адрес уже используется, нажмите Enter чтобы ввести заново...");
                    user.Email = "";
                    Console.ReadKey();
                    EnterEmail(user);
                }
            }
        }
        #endregion

        #region Ввод номера
        static void EnterPhoneNumber(User user)
        {
            Console.Clear();
            const int FIELD_NUMBER_5 = 5;
            string stars = GiveMeStars(user);

            Show(user, FIELD_NUMBER_5, stars, stars);
            user.PhoneNumber = Console.ReadLine();

            string pattern = @"^\+?[7]\d{10}$";

            if (!(Regex.IsMatch(user.PhoneNumber, pattern, RegexOptions.IgnoreCase)))
            {
                Console.WriteLine("Телефонный номер содержит недопустимые символы или введен не корректно, нажмите Enter чтобы ввести заново...");
                user.PhoneNumber = "";
                Console.ReadKey();
                EnterPhoneNumber(user);
            }
            for (int i = 0; i < users.Count; i++)
            {
                if (user.PhoneNumber == users[i].PhoneNumber)
                {
                    Console.WriteLine("Данный номер уже используется, нажмите Enter чтобы ввести заново...");
                    user.PhoneNumber = "";
                    Console.ReadKey();
                    EnterPhoneNumber(user);
                }
            }
        }
        #endregion

        #region Скрытие пароля
        public static string HideCharacter()
        {
            ConsoleKeyInfo key;
            string code = "";
            do
            {
                key = Console.ReadKey(true);
                Console.Write("*");
                code += key.KeyChar;
            } while (key.Key != ConsoleKey.Enter);

            //Console.WriteLine("\n" + code);
            return code;
        }
        #endregion

        #region Звездочки
        public static string GiveMeStars(User user)
        {
            char[] symbols = new char[user.Password.Length];

            for (int i = 0; i < user.Password.Length; i++)
            {
                symbols[i] = '*';
            }
            string str = new string(symbols);
            return str;
        }
        #endregion

        #region Десериализация
        static void JsonReading(DataContractJsonSerializer jsonFormatter)
        {
            string[] str = File.ReadAllLines("people.json");
            if (str.Length > 0)
            {
                using (FileStream fs = new FileStream("people.json", FileMode.OpenOrCreate))
                {
                    users = (List<User>)jsonFormatter.ReadObject(fs);
                }
            }
        }
        #endregion
    }
}
