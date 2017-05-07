using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;

namespace Bosel.Vsto
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }        

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
            this.Application.WorkbookActivate += Application_WorkbookActivate;
            this.Application.WorkbookBeforeClose += Application_WorkbookBeforeClose;
        }
        
        private List<Excel.Worksheet> _Worksheets = new List<Excel.Worksheet>();

        private void Application_WorkbookActivate(Excel.Workbook Wb)
        {
            if (_Worksheets.Count == 0 && this.Application.Worksheets?.Count > 0)
            {
                foreach (Excel.Worksheet sheet in this.Application.Worksheets)
                {
                    _Worksheets.Add(sheet);
                    sheet.SelectionChange += Sheet_SelectionChange;
                }
            }
        }

        private void Sheet_SelectionChange(Excel.Range Target)
        {
            var activeCell = Globals.ThisAddIn.Application.ActiveCell;
            if (activeCell?.Value != null)
            {
                Globals.Ribbons.RibbonEdit.cbxCategoriesCodes.Text = !Globals.Ribbons.RibbonEdit.chxUpperTxt.Checked 
                    ? Convert.ToString(activeCell.Value) 
                    : Convert.ToString(activeCell.Value).ToUpper();
            }
        }

        private void Application_WorkbookBeforeClose(Excel.Workbook Wb, ref bool Cancel)
        {
            if (Globals.Ribbons.RibbonEdit.HasSomethingToSave && System.Windows.Forms.MessageBox.Show("Some modifications were not changed. Do you want to save them?"
                        , "Warning"
                        , System.Windows.Forms.MessageBoxButtons.YesNo
                        , System.Windows.Forms.MessageBoxIcon.Question
                        , System.Windows.Forms.MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {
                Cancel = true;
            }
        }
    }
}
