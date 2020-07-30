using GoogleLib;

namespace Management.Operations
{
    public static class TransitionToNewMonth
    {
        #region Delegates and Events

        public delegate void TransitionHandler(int value, string message);
        public static event TransitionHandler UpdateProgress;
        public static event TransitionHandler CompletedTransition;

        #endregion

        public static bool Processing { get; set; } = false;

        public async static void StartTransitionAsync()
        {
            Processing = true;
            GoogleDrive drive = new GoogleDrive();
            GoogleSheets sheets = new GoogleSheets();

            if (await drive.TransitionCheck())
            {
                //Create folder and copy files
                UpdateProgress?.Invoke(0, "Копирование файлов в отдельную папку ...");
                await drive.CreateFolderAndCopyFilesAsync();

                //Add payments
                UpdateProgress?.Invoke(1, "Добавление платежей ...");
                await sheets.AddNewPayments();

                //Correct files
                UpdateProgress?.Invoke(2, "Переход на новый месяц в файлах ...");
                await sheets.CorrectFiles();
                UpdateProgress?.Invoke(3, "Завершение ...");

                CompletedTransition?.Invoke(0, "Все операции были успешно выполнены!");
            }
            else
            {
                CompletedTransition?.Invoke(-1, "Переход на новый месяц уже был выполнен в этом месяце!");
            }
            Processing = false;
        }
    }
}
