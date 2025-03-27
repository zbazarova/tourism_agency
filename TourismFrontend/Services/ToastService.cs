using System;

namespace TourismFrontend.Services
{
    public enum ToastType
    {
        Success,
        Error,
        Warning,
        Info
    }

    public class ToastService : IDisposable
    {
        public event Action<string, string, ToastType>? OnShow;

        public ToastService()
        {
            OnShow = null;
        }

        public void ShowSuccess(string message, string title = "Успех")
        {
            OnShow?.Invoke(message, title, ToastType.Success);
        }

        public void ShowError(string message, string title = "Ошибка")
        {
            OnShow?.Invoke(message, title, ToastType.Error);
        }

        public void ShowWarning(string message, string title = "Предупреждение")
        {
            OnShow?.Invoke(message, title, ToastType.Warning);
        }

        public void ShowInfo(string message, string title = "Информация")
        {
            OnShow?.Invoke(message, title, ToastType.Info);
        }

        public void Dispose()
        {
            OnShow = null;
        }
    }
} 