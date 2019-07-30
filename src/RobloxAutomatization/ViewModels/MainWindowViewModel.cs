using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Prism.Commands;
using Prism.Mvvm;
using RobloxAutomatization.Data;
using RobloxAutomatization.Helpers;
using RobloxAutomatization.Models;
using RobloxAutomatization.Services;

namespace RobloxAutomatization.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IRobloxAutomatizationService _automatization;
        private bool _useCustomUsername;    
        private bool _useUsernameGenerator;
        private int _generatedAccountCount;
        private string _usernamesFilePath;
        private string _monitor;

        public bool UseCustomUsername { get => _useCustomUsername; set { SetProperty(ref _useCustomUsername, value); } }
        public bool UseUsernameGenerator { get => _useUsernameGenerator; set { SetProperty(ref _useUsernameGenerator, value); } }
        public int GeneratedAccountCount { get => _generatedAccountCount; set { SetProperty(ref _generatedAccountCount, value); } }
        public string UsernamesFilePath { get => _usernamesFilePath; set { SetProperty(ref _usernamesFilePath, value); } }
        public string Monitor { get => _monitor; set { SetProperty(ref _monitor, value); } } 
        public DelegateCommand StartAutomatizationCommand { get; }
        public DelegateCommand OpenFileDialogCommand { get; }

        public MainWindowViewModel(ApplicationDbContext context, IRobloxAutomatizationService automatizationService)
        {
            //CheckTrial();

            _automatization = automatizationService;
            _db = context;
            _db.Database.EnsureCreated();
            UseCustomUsername = false;
            UseUsernameGenerator = true;

            //_automatization.ConfigureAntichatExtension();

            StartAutomatizationCommand = new DelegateCommand(() =>
            {
                Task.Run(() =>
                {
                    _automatization.OpenChrome();
                    
                    if (UseUsernameGenerator)
                    {
                        for (int i = 1; i <= GeneratedAccountCount; i++)
                        {                           
                            var generatedUser = UserGenerator.GenerateUser();
                            _automatization.RegisterNewUser(ref generatedUser);
                            _automatization.SaveUserCookies(generatedUser.Username);
                            _db.Users.Add(generatedUser);
                            AddToMonitor($"{i}. {generatedUser}");
                        }
                    }
                    else
                    {
                        var usernames = ExcelParser.ParseUsernames(UsernamesFilePath);
                        int i = 0;
                        foreach (var username in usernames)
                        {
                            var mergeUsernames = string.Concat(username.Item1, username.Item2);
                            var generatedUser = UserGenerator.GenerateUser(mergeUsernames);
                            _automatization.RegisterNewUser(ref generatedUser, true);
                            _automatization.SaveUserCookies(generatedUser.Username);
                            _db.Users.Add(generatedUser);
                            AddToMonitor($"{++i}. {generatedUser}");
                        }
                    }

                    _db.SaveChanges();
                });             
            });

            OpenFileDialogCommand = new DelegateCommand(() =>
            {
                using (var dialog = new OpenFileDialog())
                {
                    dialog.Multiselect = false;
                    dialog.Filter = "Excel file (.xlsx)|*.xlsx";
                    dialog.ShowDialog();
                    UsernamesFilePath = dialog.FileName;
                }
            });
        }        

        private void AddToMonitor(string str)
        {
            var builder = new StringBuilder();
            builder.Append(Monitor);
            builder.AppendLine(str);
            Monitor = builder.ToString();
        }

        private void CheckTrial()
        {
            var expiredDay = new DateTime(2019, 08, 3);
 
            if (DateTime.Now > expiredDay)
            {
                MessageBox.Show("Expired trial version program. Contact to developer suxrobGM@gmail.com", "Trial has expired", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Environment.Exit(1);
            }
        }
    }
}
