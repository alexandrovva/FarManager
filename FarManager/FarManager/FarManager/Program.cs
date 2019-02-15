using System;
using System.IO;
using System.Diagnostics;

namespace FarManager
{
    class FarManager
    {
        string path;
        public int cursor;
        public int sz;
        public bool ok;
        DirectoryInfo directory = null;
        FileSystemInfo currentFs = null;

        public FarManager(string path)
        {
            directory = new DirectoryInfo(path);
            sz = directory.GetFileSystemInfos().Length; //сколько файлов и папок в PP2
            this.path = path;
            cursor = 0;
        }

        public void Color(FileSystemInfo fs, int index)
        {
            if (cursor == index) //цвет для строки, на которой курсор
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                currentFs = fs;
            }
            else if (fs is DirectoryInfo) //если папка
            {
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
            }
            else //если файл
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public void Show()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.Clear(); //вывести заново список при изменении положения курсора

            directory = new DirectoryInfo(path); //берем все о папке PP2

                        FileSystemInfo[] fs = directory.GetFileSystemInfos(); //берем все файлы и папки в папке PP2

            int index = 0; //чтобы пробегаться по файлам, когда список без скрытых
            int j = 1;
            for (int i = 0; i < fs.Length; i++)
            {
                if (ok == false && fs[i].Name[0] == '.')
                {
                    continue;
                }
                Color(fs[i],index); //сразу красим в цвет в зависимости от того файл это или папка
                index++;
                Console.Write(j+"."+" ");
                Console.WriteLine(fs[i].Name); //выводим все названия папок и файлов из PP2
                j++;
            }
        }

        public void Up()
        {
            cursor--;
            if (cursor < 0) //чтобы курсор не уходил за верхний файл и переходил сразу на последний
                cursor = sz - 1;
        }

        public void Down()
        {
            cursor++;
            if (cursor == sz) //чтобы курсор не уходил за последний файл и переходил сразу на первый
                cursor = 0;
        }

        public void CalcSz() //пересчет размера каждый раз, когда убираем скрытые или наоборот добавляем
        {
            directory = new DirectoryInfo(path);
            FileSystemInfo[] fs = directory.GetFileSystemInfos();
            sz = fs.Length;
            if (ok == false)
            {
                for(int i = 0; i < fs.Length; i++)
                {
                    if (fs[i].Name[0] == '.')
                        sz--;
                }
            }
        }

        public void Start()
        {
            ConsoleKeyInfo consoleKey = Console.ReadKey();
            Show();
            while (consoleKey.Key != ConsoleKey.Escape) //пока не нажата эта клавиша программа работает
            {
                //CalcSz();
                //Show();
                consoleKey = Console.ReadKey();
                if (consoleKey.Key == ConsoleKey.UpArrow) //если нажать стрелку вверх, то курсор уйдет вверх
                {
                    Up();
                    CalcSz();
                    Show();
                }
                if (consoleKey.Key == ConsoleKey.DownArrow)  //курсор вниз
                {
                    Down();
                    CalcSz();
                    Show();
                }
                if (consoleKey.Key == ConsoleKey.RightArrow) //уберем при нажатии скрытые файлы 
                {
                    ok = false;
                    CalcSz();
                    Show();
                    cursor = 0;   
                }
                if (consoleKey.Key == ConsoleKey.LeftArrow) //не уберем скрытые при нажатии
                {
                    ok = true;
                    cursor = 0;
                    CalcSz();
                    Show();
                }
                if (consoleKey.Key==ConsoleKey.Enter) //вход в папки
                {
                    string path2;
                    path2 = currentFs.FullName;
                    if (currentFs is DirectoryInfo)
                    {
                        path = currentFs.FullName;
                        CalcSz();
                        cursor = 0;
                        Show();
                    }
                    else //чтение файла
                    {
                        Process.Start(path2);
                        Show();
                    }
                }
                if (consoleKey.Key == ConsoleKey.Backspace) //возврат к предыдущей папке
                {
                    cursor = 0;
                    path = directory.Parent.FullName; //path становится предыдущей папкой
                    CalcSz();
                    Show();
                }
                if (consoleKey.Key == ConsoleKey.D)
                {
                    string path1;
                    path1 = currentFs.FullName;
                    if (currentFs is FileInfo)
                    {
                        File.Delete(path1);
                        CalcSz();
                        Show();
                    }
                    else
                    {

                        Directory.Delete(path1);
                        CalcSz();
                        Show();
                        cursor = 0;
                    }
                }
                if (consoleKey.Key == ConsoleKey.R)
                {
                    string path4 = directory.FullName; //путь к текущей папке
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.Clear(); //очищаем 
                    string name = Console.ReadLine(); //новое имя элемента

                    if (currentFs is FileInfo)
                    {
                        string sourcefile = currentFs.FullName; //первоначальный файл 
                        string destfile = Path.Combine(path4, name);
                        File.Move(sourcefile, destfile); //преобразуем во второй

                    }
                    if (currentFs is DirectoryInfo)
                    {
                        string sourcedir = currentFs.FullName;
                        string destdir = Path.Combine(path4, name);//к ссылке папки добавляем имя элемента
                        Directory.Move(sourcedir, destdir); //копируем старый элемент в новый, изменяя название
                    }
                    Show();
                }
            }

            }


        public static void Main(string[] args)
        {
            string path = "/Users/definix/Documents/PP2"; //рабочая папка
            FarManager farManager = new FarManager(path); //указываем рабочую папку
            farManager.Start(); //вызываем функцию вывода
        }
    }
}
