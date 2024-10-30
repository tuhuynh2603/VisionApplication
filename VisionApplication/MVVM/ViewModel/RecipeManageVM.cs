﻿using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MessageBox = System.Windows.MessageBox;

namespace VisionApplication.MVVM.ViewModel
{

    public class RecipeManageVM:BaseVM, ICustomUserControl
    {

        public MainWindowVM _mainWindowVM { get; set; }
        private DragDropUserControlVM _dragDropVM { set; get; }
        public void RegisterUserControl()
        {
            _dragDropVM.RegisterMoveGrid();
            _dragDropVM.RegisterResizeGrid();
        }


        private string _m_strNewRecipe = "";
        public string m_strNewRecipe{
            get => _m_strNewRecipe;
            set {
                _m_strNewRecipe = value;
                OnPropertyChanged(nameof(m_strNewRecipe));
            } 
        }

        public ICommand Cmd_AddNewRecipe { get; set; }

        public RecipeManageVM(DragDropUserControlVM dragDropVM, MainWindowVM mainVM)
        {
            _mainWindowVM = mainVM;
            _dragDropVM = dragDropVM;
            RegisterUserControl();

            Cmd_AddNewRecipe =new RelayCommand<RecipeManageVM>((p) => { 
                return true; 
            },
                                     (p) =>
                                     {
                                         AddNewRecipe(m_strNewRecipe);
                                     });
            InitComboRecipe();
        }


        public void AddNewRecipe(string strTxt)
        {
            if (strTxt.Replace(" ", "") == "")
            {
                MessageBox.Show("This name can not be empty");
                return;
            }

            var recipes = comboRecipes.Where(recipe=> recipe.Contains(strTxt)).ToList();

            if (recipes.Count >0)
            {
                MessageBox.Show("This name has been used, please try with different name!");
                return;
            }

            string pathOldFile = Path.Combine(AppMagnus.pathRecipe, AppMagnus.currentRecipe);
            string strFullPathRecipe = System.IO.Path.Combine(AppMagnus.pathRecipe, strTxt);
            Directory.CreateDirectory(strFullPathRecipe);
            CopyFolder(pathOldFile, Path.Combine(AppMagnus.pathRecipe, strTxt), strTxt, true);
        }



        public static void CopyFolder(string oldPathFolder, string newPathFolder, string strNewRecipeName, bool isHoldName = false, bool isChild = false, bool hasExtension = false)
        {
            string[] oldfiles = Directory.GetFiles(oldPathFolder);
            string[] oldfolders = Directory.GetDirectories(oldPathFolder);
            foreach (string oldFolder in oldfolders)
            {
                string namefolder = oldFolder.Split('\\').Last();
                string newFolder = System.IO.Path.Combine(newPathFolder, namefolder);
                Directory.CreateDirectory(newFolder);
                if (namefolder == "Data" || isChild)
                    CopyFolder(oldFolder, newFolder, strNewRecipeName, true, true, true);
                else
                    CopyFolder(oldFolder, newFolder, strNewRecipeName, true, false, true);
            }
            foreach (string oldFile in oldfiles)
            {
                CopyFile(oldFile, newPathFolder, strNewRecipeName, isHoldName, hasExtension);
            }
        }
        public static void CopyFile(string oldfile, string newPathfile, string strNewRecipeName, bool isHoldName = false, bool hasExtension = false)
        {
            string namefile = System.IO.Path.GetFileName(oldfile);
            if (isHoldName)
            {
                if (hasExtension)
                    namefile = Path.GetFileName(oldfile);
                string newfile = System.IO.Path.Combine(newPathfile, namefile);
                if (!File.Exists(newfile))
                    File.Copy(oldfile, newfile);
            }
            else
            {
                if (Char.IsDigit(namefile.Last()))
                {
                    string newfile = System.IO.Path.Combine(newPathfile, string.Format("{0}{1}{2}",
                                                            strNewRecipeName, namefile.Last(), System.IO.Path.GetExtension(oldfile)));
                    if (!File.Exists(newfile))
                        File.Copy(oldfile, newfile);
                }
                else
                {
                    string newfile = System.IO.Path.Combine(newPathfile, string.Format("{0}{1}",
                                                             strNewRecipeName, System.IO.Path.GetExtension(oldfile)));
                    if (!File.Exists(newfile))
                        File.Copy(oldfile, newfile);
                }
            }
        }


        private object _comboRecipeSelectedItem;

        public object comboRecipeSelectedItem
        {
            get => _comboRecipeSelectedItem;
            set
            {
                _comboRecipeSelectedItem = value;
                OnPropertyChanged(nameof(comboRecipeSelectedItem));
            }
        }


        private ActionCommand deleteRecipeCommand;

        public ICommand DeleteRecipeCommand
        {
            get
            {
                if (deleteRecipeCommand == null)
                {
                    deleteRecipeCommand = new ActionCommand(DeleteRecipe);
                }

                return deleteRecipeCommand;
            }
        }

        private void DeleteRecipe()
        {
        }

        private ActionCommand loadRecipeCommand;

        public ICommand LoadRecipeCommand
        {
            get
            {
                if (loadRecipeCommand == null)
                {
                    loadRecipeCommand = new ActionCommand(LoadRecipe);
                }

                return loadRecipeCommand;
            }
        }

        private void LoadRecipe()
        {
            MainWindowVM.master.LoadRecipe(comboRecipeSelectedItem.ToString());
        }

        private ActionCommand closeRecipeCommand;

        public ICommand CloseRecipeCommand
        {
            get
            {
                if (closeRecipeCommand == null)
                {
                    closeRecipeCommand = new ActionCommand(CloseRecipe);
                }

                return closeRecipeCommand;
            }
        }

        private void CloseRecipe()
        {
        }

        private ObservableCollection<string> _comboRecipes = new ObservableCollection<string>();

        public ObservableCollection<string> comboRecipes
        {
            get => _comboRecipes;
            set
            {
                _comboRecipes = value;
                OnPropertyChanged(nameof(comboRecipes));

            }
        }

        public void InitComboRecipe()
        {
            if (AppMagnus.pathRecipe == null)
                return;
            string[] oldfolders = Directory.GetDirectories(AppMagnus.pathRecipe);
            for (int n = 0; n < oldfolders.Length; n++)
            {
                string strRight = oldfolders[n].Replace(AppMagnus.pathRecipe, "");
                string[] strSplits = strRight.Split('\\');
                string strName = strSplits[1];
                if (strName == "")
                    continue;
                comboRecipes.Add(strName);
            }
            comboRecipeSelectedItem = comboRecipes.Where(recipe => recipe == AppMagnus.currentRecipe).FirstOrDefault();

        }
    }
}
