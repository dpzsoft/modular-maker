using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XormManager {
    public partial class FrmSetting : Form {

        // 设置组信息
        private dpz3.File.Conf.SettingGroup settingGroup;

        public FrmSetting() {
            InitializeComponent();
        }

        /// <summary>
        /// 设置配置组信息
        /// </summary>
        /// <param name="group"></param>
        public void SetConfigGroup(dpz3.File.Conf.SettingGroup group) {
            settingGroup = group;

            // 清理所有行
            this.dataGridView1.Rows.Clear();
        }

        /// <summary>
        /// 设置配置项
        /// </summary>
        /// <param name="name"></param>
        public void SetConfigIitem(string name) {
            this.dataGridView1.Rows.Add(name, settingGroup[name]);
        }

        private void DataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e) {
            // 强制半角输入
            e.Control.ImeMode = ImeMode.KatakanaHalf;
        }

        private void FrmSetting_Load(object sender, EventArgs e) {

        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e) {

            if (e.RowIndex < 0) return;

            // 获取相关信息
            var name = this.dataGridView1.Rows[e.RowIndex].Cells[0].Value as string;
            var value = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value as string;

            // 将输入应用到配置组中
            settingGroup[name] = value;

        }
    }
}
