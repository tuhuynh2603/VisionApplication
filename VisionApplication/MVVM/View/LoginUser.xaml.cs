﻿using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using System.Windows;
using VisionApplication.Define;
using Application = VisionApplication.AppMagnus;
using VisionApplication.MVVM.ViewModel;
using Microsoft.Xaml.Behaviors.Core;
using UserControl = System.Windows.Controls.UserControl;
using Brushes = System.Windows.Media.Brushes;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using Panel = System.Windows.Controls.Panel;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

namespace VisionApplication.MVVM.View
{
	public partial class LoginUser : UserControl

	{
		public DataTable tableAccount = new DataTable();

		//private MainWindowVM mainVM;
		public LoginUser()
		{
			InitializeComponent();
			//mainVM = (MainWindowVM)MainWindow.mainWindow.DataContext;

			InitLogInDialog();

            acountDefault();
            try
            {
                ReadLogAccount();
                LogMessage.WriteToDebugViewer(2, string.Format("Read Log Account Success "));
            }
            catch (Exception)
            {
                LogMessage.WriteToDebugViewer(2, string.Format("Read Log Account Failed "));
            }
            //LoginUserVM loginUserVM = DataContext as LoginUserVM;
            //loginUserVM._mainWindowVM.enableButton(true);
        }
        //public void AssignMainWindow()
        //{
        //	if (main == null)
        //		main = MainWindowVM.mainWindow;
        //}
        private void SetupAccount()
		{
			DataColumn column;

			//DataRow row;
			column = new DataColumn();
			column.ColumnName = "username";
			column.DataType = typeof(string);
			column.Unique = true;
			tableAccount.Columns.Add(column);

			column = new DataColumn();
			column.ColumnName = "access";
			column.DataType = typeof(AccessLevel);
			column.Unique = false;
			tableAccount.Columns.Add(column);

			column = new DataColumn();
			column.ColumnName = "password";
			column.DataType = typeof(string);
			column.Unique = false;
			tableAccount.Columns.Add(column);
		}
		#region USER AND PASSWORD
		private string EncryptPass(string password)
		{

			string enscryptPassword = "";
			password += "ADMN";
			foreach (char c in password)
			{
				enscryptPassword += (char)(c - 15);
			}

			return enscryptPassword;
		}
		private bool CheckUsername(string username)
		{
			if (username != "")
			{
				foreach (DataRow row in tableAccount.Rows)
				{
					if (row["username"].ToString() == username)
						return true;
				}
			}
			return false;
		}
		public AccessLevel GetAccessLevel(string username)
		{
			foreach (DataRow row in tableAccount.Rows)
			{
				if (row["username"].ToString() == username)
					return (AccessLevel)row["access"];
			}
			return AccessLevel.None;
		}
		private string GetPassword(string username)
		{
			foreach (DataRow row in tableAccount.Rows)
			{
				if (row["username"].ToString() == username)
                {
					string strPass = row["password"].ToString();
					return strPass;

				}
			}
			return null;
		}
		string AccessLevelString(AccessLevel accessLevel)
		{
			switch (accessLevel)
			{
				case AccessLevel.Engineer:
					return "Engineer";
				case AccessLevel.Operator:
					return "Operator";
				case AccessLevel.User:
					return "User";
				default:
					return "None";
			}
		}
		public bool ChangePasswordDataTable(string username, string newPassword)
		{
			foreach (DataRow row in tableAccount.Rows)
			{
				if (row["username"].ToString() == username)
				{
					row["password"] = EncryptPass(newPassword);
					SaveLogAccount();
					return true;
				}
			}
			return false;
		}
		public void SaveLogAccount()
		{
			List<string> listaccounts = new List<string>();
			listaccounts.Add("[NUM USER]");
			listaccounts.Add(string.Format("NoOfUsers={0}", tableAccount.Rows.Count));
			listaccounts.Add("");
			int i = 0;
			foreach (DataRow row in tableAccount.Rows)
			{
				listaccounts.Add(string.Format("[User{0}]", i++));
				listaccounts.Add(string.Format("Name={0}", row["username"]));
				listaccounts.Add(string.Format("Level={0}", AccessLevelString((AccessLevel)row["access"])));
				listaccounts.Add(string.Format("Pswd={0}", row["password"]));
				listaccounts.Add("");
			}

			string pathFile = System.IO.Path.Combine(AppMagnus.pathRecipe, "LogAccount.lgn");
			if (!File.Exists(pathFile))
				return;
			using (StreamWriter Files = new StreamWriter(pathFile))
			{
				foreach (string line in listaccounts)
					Files.WriteLine(line);
			}
			//DebugMessage.WriteToDebugViewer(0, "Save Log Account file");
		}
		private AccessLevel GetCheckedRadionbutton()
		{
			if (engineerLevel.IsChecked == true)
				return AccessLevel.Engineer;
			else if (operatorLevel.IsChecked == true)
				return AccessLevel.Operator;
			else if (userLevel.IsChecked == true)
				return AccessLevel.User;
			else
				return AccessLevel.None;
		}
		private DataRow GetRow(string username)
		{
			foreach (DataRow row in tableAccount.Rows)
			{
				if (row["username"].ToString() == username)
					return row;
			}
			return null;
		}

		void ResetTextBox()
		{
			// For Login
			userName.Text = ""; userName.Foreground = Brushes.Gray;
			passWord.Password = "1111111"; passWord.Foreground = Brushes.Gray;

			// For create new account
			userNameNew.Text = "USERNAME"; userNameNew.Foreground = Brushes.Gray;
			passWordNew.Password = "1111111"; passWordNew.Foreground = Brushes.Gray;
			ConfirmPassWordNew.Password = "1111111"; ConfirmPassWordNew.Foreground = Brushes.Gray;

			// For change password
			NewPassWord.Password = "1111111"; NewPassWord.Foreground = Brushes.Gray;
			ConfirmNewPassWord.Password = "1111111"; ConfirmNewPassWord.Foreground = Brushes.Gray;

			NotifyLogin.Text = "";
			NotifyNewUser.Text = "";
			NotifyChangePw.Text = "";
		}
		//public void KeyShortcut(object sender, KeyEventArgs e)
		//{
		//	switch (e.KeyValue)
		//	{
		//		case (int)Key.Enter:
		//			if (Panel.GetZIndex(panelLogIn) == 2 || Panel.GetZIndex(panelLogIn) == 3 && !loginOk.IsFocused)
		//				LoginClickCmd();
		//			else if (Panel.GetZIndex(panelCreateUser) == 2 && !createOk.IsFocused)
		//				CreateNewUser_Click(sender, e);
		//			else if (Panel.GetZIndex(panelChangePassword) == 2 && !chagneOk.IsFocused)
		//				ChangePW_Click(sender, e);
		//			break;
		//		default:
		//			break;
		//	}
		//}
		public class ConvertColorToBool : IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			{
				return (System.Windows.Media.Brush)value == Brushes.Gray ? false : true;
			}
			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			{
				return Brushes.Gray;
			}
		}
        #endregion

        #region EVENT CLICK BUTTON

        private ActionCommand loginCommand;

        public ICommand LoginCommand
        {
            get
            {
                if (loginCommand == null)
                {
                    loginCommand = new ActionCommand(LoginClickCmd);
                }
                return loginCommand;
            }
        }

		public void LoginClickCmd()
		{
            string pw = "";
            if (userName.Foreground == Brushes.Gray && userName.Text == "")
            {
                NotifyLogin.Text = "Type your Username !";
                return;
            }
            if (passWord.Foreground == Brushes.Gray)
                pw = EncryptPass("");
            else pw = EncryptPass(passWord.Password);
            if (!CheckUsername(userName.Text))
            {
                NotifyLogin.Text = "Username is wrong !";
                return;
            }
            else if (pw == GetPassword(userName.Text))
            {
                MainWindowVM.IsFisrtLogin = true;
                //main.AddHotKey();
                MainWindowVM.accountUser = userName.Text;
                MainWindowVM.accessLevel = GetAccessLevel(userName.Text);

                MainWindowVM.UICurrentState = UISTate.IDLE_STATE;

                currentUser.Content = userName.Text;
                currentAccesslevel.Content = AccessLevelString(GetAccessLevel(userName.Text));
                LogMessage.WriteToDebugViewer(0, string.Format("Login to user '{0}' Success", userName.Text));
                ResetTextBox();

                //main.ChangeUIState();
                //main.btnLogIn.IsChecked = false;
                MainWindow.mainWindow.btnLogIn.IsEnabled = true;
                MainWindow.mainWindow.btnLogIn.Content = MainWindowVM.accountUser.ToString();
                MainWindow.mainWindow.acessLevel.Text = MainWindowVM.accessLevel.ToString();
                ResetTextBox();

                LoginUserVM loginUserVM = this.DataContext as LoginUserVM;
                loginUserVM._mainWindowVM.enableButton(true);
				loginUserVM._mainWindowVM.mLoginUserVM.isVisible = Visibility.Collapsed;

                MainWindowVM.loadAllStatisticDelegate?.Invoke(false);

                return;
            }
            else
            {
                LogMessage.WriteToDebugViewer(0, string.Format("Login to user '{0}' Fail, Password is wrong.", userName.Text));
                NotifyLogin.Text = "Password is wrong !";
                return;
            }
        }

		private void Login_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			string pw = "";
			if (userName.Foreground == Brushes.Gray && userName.Text == "")
			{
				NotifyLogin.Text = "Type your Username !";
				return;
			}
			if (passWord.Foreground == Brushes.Gray)
				pw = EncryptPass("");
			else pw = EncryptPass(passWord.Password);
			if (!CheckUsername(userName.Text))
			{
				NotifyLogin.Text = "Username is wrong !";
				return;
			}
			else if (pw == GetPassword(userName.Text))
			{
				MainWindowVM.IsFisrtLogin = true;
				//main.AddHotKey();
				MainWindowVM.accountUser = userName.Text;
				MainWindowVM.accessLevel = GetAccessLevel(userName.Text);

				MainWindowVM.UICurrentState = UISTate.IDLE_STATE;

				currentUser.Content = userName.Text;
				currentAccesslevel.Content = AccessLevelString(GetAccessLevel(userName.Text));
				LogMessage.WriteToDebugViewer(0, string.Format("Login to user '{0}' Success", userName.Text));
				ResetTextBox();

				//main.ChangeUIState();
				//main.btnLogIn.IsChecked = false;
				MainWindow.mainWindow.btnLogIn.IsEnabled = true;
				MainWindow.mainWindow.btnLogIn.Content = MainWindowVM.accountUser.ToString();
				MainWindow.mainWindow.acessLevel.Text = MainWindowVM.accessLevel.ToString();
				ResetTextBox();

				LoginUserVM loginUserVM = this.DataContext as LoginUserVM;
				loginUserVM._mainWindowVM.enableButton(true);
                loginUserVM._mainWindowVM.mLoginUserVM.isVisible = Visibility.Collapsed;

                MainWindowVM.loadAllStatisticDelegate?.Invoke(false);

				return;
			}
			else
			{
				LogMessage.WriteToDebugViewer(0, string.Format("Login to user '{0}' Fail, Password is wrong.", userName.Text));
				NotifyLogin.Text = "Password is wrong !";
				return;
			}

		}

		private void Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			int currentTabIndex = MainWindow.mainWindow.tab_controls.SelectedIndex;
            MainWindow.mainWindow.btnLogIn.IsChecked = false;
            MainWindow.mainWindow.btnLogIn.IsEnabled = true;
			if (MainWindowVM.IsFisrtLogin)
			{

			}
            MainWindow.mainWindow.tab_controls.SelectedIndex = currentTabIndex;
			ResetTextBox();
            LoginUserVM loginUserVM = this.DataContext as LoginUserVM;
            loginUserVM._mainWindowVM.mLoginUserVM.isVisible = Visibility.Collapsed;

            //loginUserVM.loginUserVisible = Visibility.Collapsed;


        }
        private void ChangePW_Click(object sender, System.Windows.RoutedEventArgs e)
		{

			if (NewPassWord.Foreground == Brushes.Gray) NewPassWord.Password = "";
			if (ConfirmNewPassWord.Foreground == Brushes.Gray) ConfirmNewPassWord.Password = "";
			if (NewPassWord.Password != ConfirmNewPassWord.Password)
			{
				NotifyChangePw.Text = "Password and Confirm password is not match !\nCheck again.";
				return;
			}
			else
			{
				if (NewPassWord.Foreground == Brushes.Gray) NewPassWord.Password = "";
				string newpass = NewPassWord.Password;
				ResetTextBox();
				if (ChangePasswordDataTable(MainWindowVM.accountUser, newpass))
				{
					NotifyChangePw.Text = "Password is changed !";
					LogMessage.WriteToDebugViewer(0, string.Format("User '{0}' changed Password", MainWindowVM.accountUser));
				}
				else
					NotifyChangePw.Text = "Password can not change !";
			}
		}

		private void CreateNewUser_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (passWordNew.Foreground == Brushes.Gray) passWordNew.Password = "";
			if (ConfirmPassWordNew.Foreground == Brushes.Gray) ConfirmPassWordNew.Password = "";
			if (userNameNew.Foreground == Brushes.Gray || userNameNew.Text == "")
			{
				NotifyNewUser.Text = "Type Username !";
				return;
			}

			if (CheckUsername(userNameNew.Text))
			{
				NotifyNewUser.Text = "Username is exist! Create another Username";
				return;
			}
			if (passWordNew.Password != ConfirmPassWordNew.Password)
			{
				NotifyNewUser.Text = "Password and Confirm password is not match !\n Check again.";
				return;
			}

			DataRow row = tableAccount.NewRow();
			row["username"] = userNameNew.Text;
			row["password"] = EncryptPass(passWordNew.Password);
			row["access"] = GetCheckedRadionbutton();
			tableAccount.Rows.Add(row);
			LogMessage.WriteToDebugViewer(0, string.Format("Create new Account '{0}' Success", userName.Text));
			SaveLogAccount();
			ResetTextBox();
			NotifyNewUser.Text = string.Format("Created new account Username: '{0}',\nAccess Level: {1}", row["username"], AccessLevelString((AccessLevel)row["access"]));


		}

		#endregion

		#region EVENT MOUSEDOWN
		private void logInMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			InitLogInDialog();
		}

		public void InitLogInDialog()
        {
			panelLogIn.IsEnabled = true;
			panelLogIn.Visibility = Visibility.Visible;

			panelChangePassword.IsEnabled = false;
			panelChangePassword.Visibility = Visibility.Collapsed;

			panelCreateUser.IsEnabled = false;
			panelCreateUser.Visibility = Visibility.Collapsed;

            Panel.SetZIndex(panelLogIn, 2);
            Panel.SetZIndex(panelChangePassword, 0);
            Panel.SetZIndex(panelCreateUser, 0);
            ResetTextBox();
			userName.Focus();
            userName.IsTabStop = true;
        }

		private void NewUserMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			//main.CleanHotKey();
			if (MainWindowVM.accountUser != "None")
			{
                Panel.SetZIndex(panelLogIn, 0);
                Panel.SetZIndex(panelChangePassword, 0);
                Panel.SetZIndex(panelCreateUser, 2);

                panelLogIn.IsEnabled = false ;
				panelLogIn.Visibility = Visibility.Collapsed;

				panelChangePassword.IsEnabled = false;
				panelChangePassword.Visibility = Visibility.Collapsed;

				panelCreateUser.IsEnabled = true;
				panelCreateUser.Visibility = Visibility.Visible;

				ResetTextBox();
				userNameNew.Focus();
				engineerLevel.IsEnabled = true;
				operatorLevel.IsEnabled = true;
				userLevel.IsEnabled = true;
				if (MainWindowVM.accessLevel == AccessLevel.Engineer)
				{
					engineerLevel.IsEnabled = true;
					engineerLevel.IsChecked = true;
				}
				else if (MainWindowVM.accessLevel == AccessLevel.Operator)
				{
					engineerLevel.IsEnabled = false;
					operatorLevel.IsChecked = true;
				}
				else if (MainWindowVM.accessLevel == AccessLevel.User)
				{
					engineerLevel.IsEnabled = false;
					operatorLevel.IsEnabled = false;
					userLevel.IsChecked = true;
				}
			}
			else
			{
				MessageBox.Show("Log in before create new account", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}
		private void ChangePWMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			//main.CleanHotKey();
			if (MainWindowVM.accountUser != "None")
			{
                Panel.SetZIndex(panelLogIn, 0);
                Panel.SetZIndex(panelChangePassword, 2);
                Panel.SetZIndex(panelCreateUser, 0);
                panelLogIn.IsEnabled = false;
				panelLogIn.Visibility = Visibility.Collapsed;

				panelChangePassword.IsEnabled = true;
				panelChangePassword.Visibility = Visibility.Visible;

				panelCreateUser.IsEnabled = false;
				panelCreateUser.Visibility = Visibility.Collapsed;

				ResetTextBox();
				NewPassWord.Focus();
			}
			else
			{
				MessageBox.Show("Log in before change password", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}
		private void DeleteUserMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (MainWindowVM.accountUser != "None")
			{
				if (MessageBox.Show(string.Format("Are you sure delete this account '{0}'", MainWindowVM.accountUser), "Information", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
				{

                    MainWindowVM.accessLevel = GetAccessLevel(MainWindowVM.accountUser);
					string pw = GetPassword(MainWindowVM.accountUser);
					if (MainWindowVM.accountUser == "Engineer" && MainWindowVM.accessLevel == AccessLevel.Engineer && pw == PwsDefault)
					{
						MessageBox.Show("Can not Delete this Acount", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
					}
					else
					{
						if (MainWindowVM.accessLevel != (int)AccessLevel.None)
						{
							tableAccount.Rows.Remove(GetRow(MainWindowVM.accountUser));
							SaveLogAccount();
							MainWindowVM.UICurrentState = UISTate.LOGOUT_STATE;
							//main.ChangeUIState();
							LogMessage.WriteToDebugViewer(0, string.Format("Delete user '{0}'", currentUser.Content));
							currentUser.Content = "None";
							currentAccesslevel.Content = "None";
							MainWindow.mainWindow.btnLogIn.IsChecked = false;
							MainWindow.mainWindow.btnLogIn.IsEnabled = true;
							MainWindow.mainWindow.btnLogIn.Label = currentUser.Content.ToString();
                            MainWindow.mainWindow.acessLevel.Text = currentAccesslevel.Content.ToString();
						}
                        MainWindowVM.IsFisrtLogin = false;
					}
				}
				else
				{
					int currentTabIndex = MainWindow.mainWindow.tab_controls.SelectedIndex;
                    MainWindow.mainWindow.btnLogIn.IsChecked = false;
                    MainWindow.mainWindow.btnLogIn.IsEnabled = true;
                    //main.AddHotKey();
                    MainWindow.mainWindow.tab_controls.SelectedIndex = currentTabIndex;
				}
			}
			else
			{
				MessageBox.Show("Log in before delete account", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}
		private void LogOutMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (MainWindowVM.accountUser != "None")
			{
				if (MessageBox.Show("Log Out This Account ?", "Question ?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{

					currentUser.Content = "None";
					currentAccesslevel.Content = "None";

                    MainWindowVM.IsFisrtLogin = false;
					//main.CleanHotKey();
					MainWindowVM.UICurrentState = UISTate.LOGOUT_STATE;
					//main.ChangeUIState();
					LogMessage.WriteToDebugViewer(0, string.Format("Loged out of user '{0}'", currentUser.Content));
					currentUser.Content = "None";
					currentAccesslevel.Content = "None";
					MainWindow.mainWindow.acessLevel.Text = "None";
					MainWindow.mainWindow.btnLogIn.Label = "None";
					MainWindow.mainWindow.btnLogIn.Content = "None";
					MainWindow.mainWindow.btnLogIn.IsChecked = false;
                    MainWindow.mainWindow.btnLogIn.IsEnabled = true;

                    LoginUserVM loginUserVM = this.DataContext as LoginUserVM;
                    loginUserVM._mainWindowVM.enableButton(false);

                }
				ResetTextBox();
			}
		}
		#endregion

		#region ANIMATION USER AND PASSWORD
		private void PWGotFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			var obj = sender as PasswordBox;
			if (obj != null && obj.Foreground == Brushes.Gray)
			{
				obj.Password = "";
				obj.Foreground = Brushes.Black;
			}
		}
		private void PWLostFoucs(object sender, System.Windows.RoutedEventArgs e)
		{
			var obj = sender as PasswordBox;
			if (obj != null && obj.Password == "")
			{
				obj.Password = "1111111111";
				obj.Foreground = Brushes.Gray;
			}
		}
		private void TextBoxLostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			var obj = sender as TextBox;
			if (obj != null && obj.Text == "")
			{
				obj.Text = "USERNAME";
				obj.Foreground = Brushes.Gray;
			}
		}
		private void TextboxGotFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			var obj = sender as TextBox;
			if (obj != null && obj.Foreground == Brushes.Gray)
			{
				obj.Text = "";
				obj.Foreground = Brushes.Black;
			}
		}
        #endregion



        public static string UserDefault;
        public static string LevelDefault;
        public static string PwsDefault;

        private string nameUserDefault;
        private string levelUserDefault;
        private string pwsUserDefault;

        public void acountDefault()
        {
            UserDefault = "Engineer";
            LevelDefault = "Engineer";
            PwsDefault = "6_XZ_VVc25>?";

            nameUserDefault = "Name=" + UserDefault;
            levelUserDefault = "Level=" + LevelDefault;
            pwsUserDefault = "Pswd=" + PwsDefault;
        }

        #region Acount
        //public static LoginUser loginUser = new LoginUser();
        public void ReadLogAccount()
        {
            DataColumn column = new DataColumn();

            tableAccount = new DataTable();
            column.ColumnName = "username";
            column.DataType = typeof(string);
            column.Unique = true;
            tableAccount.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "access";
            column.DataType = typeof(AccessLevel);
            column.Unique = false;
            tableAccount.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "password";
            column.DataType = typeof(string);
            column.Unique = false;
            tableAccount.Columns.Add(column);

            Directory.CreateDirectory(AppMagnus.pathRecipe);
            string pathFile = Path.Combine(AppMagnus.pathRecipe, "LogAccount.lgn");
            if (!File.Exists(pathFile))
            {
                // pw: Engineer
                string[] fileAccount =
                {
                    "[NUM USER]",
                    "NoOfUsers=1",
                    "",
                    "[User0]",
                   nameUserDefault,
                   levelUserDefault,
                   pwsUserDefault,
                };

                using (StreamWriter Files = new StreamWriter(pathFile))
                {
                    foreach (string line in fileAccount)
                        Files.WriteLine(line);
                }
                // To do: convert datatable
                GetTableAccount(tableAccount, fileAccount);
                tableAccount = tableAccount;
            }
            else
            {
                string[] fileAccount = File.ReadAllLines(pathFile);
                GetTableAccount(tableAccount, fileAccount);
                tableAccount = tableAccount;

            }
        }
        public void GetTableAccount(DataTable tableacount, string[] accounts)
        {
            DataRow row;
            foreach (string line in accounts)
            {
                if (line.Contains("[User"))
                {
                    int pos = Array.IndexOf(accounts, line);
                    row = tableacount.NewRow();
                    row["username"] = accounts[++pos].Split('=')[1];
                    row["access"] = StringToAccessLevel(accounts[++pos].Split('=')[1]);
                    row["password"] = accounts[++pos].Split('=')[1];
                    tableacount.Rows.Add(row);
                }
            }
        }
        private AccessLevel StringToAccessLevel(string level)
        {
            if (string.Compare(level, "Engineer", true) == 0) return AccessLevel.Engineer;
            else if (string.Compare(level, "Operator", true) == 0) return AccessLevel.Operator;
            else if (string.Compare(level, "User", true) == 0) return AccessLevel.User;
            else return AccessLevel.None;
        }

		#endregion


	}

}
