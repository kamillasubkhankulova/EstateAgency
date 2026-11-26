using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace EstateAgency.Services
{
    /// <summary>
    /// Централизованная обработка ошибок
    /// </summary>
    public static class ExceptionHandler
    {
        private static readonly object SyncRoot = new();
        public static string LogFilePath { get; } =
            Path.Combine(AppContext.BaseDirectory, "ErrorLog.txt");

        public static void Handle(Exception exception, string source)
        {
            ArgumentNullException.ThrowIfNull(exception);

            Log(exception, source);
            ShowUserMessage(exception);
        }
        public static void Log(Exception exception, string source)
        {
            try
            {
                var builder = new StringBuilder();

                builder.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Источник: {source}");
                builder.AppendLine($"Тип: {exception.GetType().FullName}");
                builder.AppendLine($"Сообщение: {exception.Message}");

                if (exception.InnerException is not null)
                    builder.AppendLine($"Внутренняя ошибка: {exception.InnerException.Message}");

                builder.AppendLine(exception.StackTrace);
                builder.AppendLine(new string('-', 80));

                lock (SyncRoot)
                {
                    File.AppendAllText(LogFilePath, builder.ToString(), Encoding.UTF8);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Текст, который увидит пользователь
        /// </summary>
        public static string GetUserMessage(Exception exception) => exception switch
        {
            ArgumentException => exception.Message,
            KeyNotFoundException => exception.Message,
            InvalidOperationException => exception.Message,
            UnauthorizedAccessException => "Недостаточно прав для выполнения операции.",
            DbUpdateException => "Не удалось сохранить данные. Проверьте обязательные поля и связи между записями.",
            _ => "Произошла непредвиденная ошибка. Приложение продолжит работу."
        };

        private static void ShowUserMessage(Exception exception)
        {
            try
            {
                MessageBox.Show(
                    $"{GetUserMessage(exception)}\n\nПодробности записаны в файл:\n{LogFilePath}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch
            {

            }
        }
    }
}