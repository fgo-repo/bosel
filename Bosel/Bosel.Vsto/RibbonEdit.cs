using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using System.IO;
using Microsoft.Office.Interop.Excel;
using Bosel.Model.Common.Source;
using Utils;
using System.Windows.Forms;

namespace Bosel.Vsto
{
    public partial class RibbonEdit
    {
        private CategoryList _categoriesDef = CategoryList.Default();
        private string _pathFile;
        private bool _hasSomethingToSave = false;
        internal bool HasSomethingToSave
        {
            get
            {
                return _hasSomethingToSave;
            }
        }

        private void RibbonEdit_Load(object sender, RibbonUIEventArgs e)
        {
            cbxCategories.TextChanged += CbxCategories_TextChanged;
            btnAddCategory.Click += btnAddCategory_Click;
            btnAddCode.Click += BtnAddCode_Click;
            btnOpen.Click += BtnOpen_Click;
            btnSave.Click += BtnSave_Click;
            btnDelCategory.Click += BtnDelCategory_Click;
            btnDelCode.Click += BtnDelCode_Click;
        }

        private void BtnSave_Click(object sender, RibbonControlEventArgs e)
        {
            var catJson = _categoriesDef.SetJson();
            if(File.Exists(_pathFile))
            {
                try
                {
                    File.WriteAllText(_pathFile, catJson);
                    _hasSomethingToSave = false;
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message
                        , "Error"
                        , System.Windows.Forms.MessageBoxButtons.OK
                        , System.Windows.Forms.MessageBoxIcon.Error
                        , System.Windows.Forms.MessageBoxDefaultButton.Button1);
                }
            }
            else
            {
                if (saveFileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _pathFile = saveFileDlg.FileName;
                    btnSave.ScreenTip = $"output path: {_pathFile}";
                    File.WriteAllText(_pathFile, catJson);
                    _hasSomethingToSave = false;
                }
            }
        }

        private void BtnOpen_Click(object sender, RibbonControlEventArgs e)
        {
            if (openFileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _pathFile = openFileDlg.FileName;
                btnSave.ScreenTip = $"output path: {_pathFile}";
                _categoriesDef = _pathFile.GetJson<CategoryList>();

                cbxCategories.Items.Clear();
                foreach (var category in _categoriesDef.Categories)
                {
                    var item = Factory.CreateRibbonDropDownItem();
                    item.Label = category.Code;
                    cbxCategories.Items.Add(item);
                }
            }
        }        

        private void btnAddCategory_Click(object sender, RibbonControlEventArgs e)
        {
            var txt = cbxCategories.Text;

            var selectedCategory = _categoriesDef.Categories.Select(c => c.Code).FirstOrDefault(c => c == txt);
            if (string.IsNullOrEmpty(selectedCategory))
            {
                var item = Factory.CreateRibbonDropDownItem();
                item.Label = txt;
                cbxCategories.Items.Add(item);
                _categoriesDef.Categories.Add(new Category() { Code = txt, CategoryCodes = new List<CategoryCode>() });
                cbxCategoriesCodes.Items.Clear();

                _hasSomethingToSave = true;
            }
        }

        private void BtnAddCode_Click(object sender, RibbonControlEventArgs e)
        {
            var txt = cbxCategories.Text;
            var newCode = cbxCategoriesCodes.Text;            

            if (_categoriesDef != null)
            {
                var selectedCategory = _categoriesDef.Categories.FirstOrDefault(c => c.Code == txt);

                if (!string.IsNullOrWhiteSpace(newCode)
                    && !string.IsNullOrWhiteSpace(txt)
                    && selectedCategory != null
                    && (selectedCategory.CategoryCodes == null || selectedCategory.CategoryCodes.FirstOrDefault(c => c.Code == newCode) == null)
                    )
                {
                    if(selectedCategory.CategoryCodes == null)
                    {
                        selectedCategory.CategoryCodes = new List<CategoryCode>();
                    }
                    selectedCategory.CategoryCodes.Add(new CategoryCode() { Code = newCode });
                    var item = Factory.CreateRibbonDropDownItem();
                    item.Label = newCode;
                    cbxCategoriesCodes.Items.Add(item);
                    cbxCategoriesCodes.Text = string.Empty;

                    _hasSomethingToSave = true;
                }
            }
        }

        private void CbxCategories_TextChanged(object sender, RibbonControlEventArgs e)
        {
            var txt = cbxCategories.Text;

            if (_categoriesDef != null)
            {
                var selectedCategory = _categoriesDef.Categories.FirstOrDefault(c => c.Code == txt);
                if (selectedCategory != null)
                {
                    RefreshCategoryCodes(selectedCategory);
                }
            }
        }

        private void RefreshCategoryCodes(Category selectedCategory)
        {
            cbxCategoriesCodes.Items.Clear();
            foreach (var categoryCode in selectedCategory.CategoryCodes.Select(s => s.Code))
            {
                var item = Factory.CreateRibbonDropDownItem();
                item.Label = categoryCode;
                cbxCategoriesCodes.Items.Add(item);
            }
        }

        private void BtnDelCode_Click(object sender, RibbonControlEventArgs e)
        {
            var txt = cbxCategories.Text;
            var newCode = cbxCategoriesCodes.Text;

            if (_categoriesDef != null)
            {
                var selectedCategory = _categoriesDef.Categories.FirstOrDefault(c => c.Code == txt);

                if (!string.IsNullOrWhiteSpace(newCode)
                    && !string.IsNullOrWhiteSpace(txt)
                    && selectedCategory != null
                    && !string.IsNullOrEmpty(selectedCategory.CategoryCodes.Select(c => c.Code).FirstOrDefault(c => c == newCode))
                    )
                {
                    var code = selectedCategory.CategoryCodes.First(c => c.Code == newCode);
                    selectedCategory.CategoryCodes.Remove(code);
                    cbxCategoriesCodes.Text = string.Empty;
                    RefreshCategoryCodes(selectedCategory);

                    _hasSomethingToSave = true;
                }
            }
        }

        private void BtnDelCategory_Click(object sender, RibbonControlEventArgs e)
        {
            var txt = cbxCategories.Text;
            var selectedCategory = _categoriesDef.Categories.FirstOrDefault(c => c.Code == txt);

            if (selectedCategory != null)
            {
                _categoriesDef.Categories.Remove(selectedCategory);
                cbxCategories.Text = string.Empty;
                cbxCategories.Items.Clear();
                foreach (var category in _categoriesDef.Categories)
                {
                    var item = Factory.CreateRibbonDropDownItem();
                    item.Label = category.Code;
                    cbxCategories.Items.Add(item);
                }
                cbxCategoriesCodes.Items.Clear();

                _hasSomethingToSave = true;
            }
        }
    }
}
