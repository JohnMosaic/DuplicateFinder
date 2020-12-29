using System.Windows.Forms;

namespace DuplicateFinder.Utils.Common
{
    public class DgvLayout
    {
        public void SetDgvColumnsWidth(DataGridView dgv)
        {
            dgv.Columns[0].Width = 45;//IsDel
            dgv.Columns[1].Width = 65;//SN
            dgv.Columns[2].Width = 250;//MD5
            dgv.Columns[3].Width = 250;//Name
            dgv.Columns[4].Width = 100;//Type
            dgv.Columns[5].Width = 90;//Size
            dgv.Columns[6].Width = 600;//Path
        }
    }
}
