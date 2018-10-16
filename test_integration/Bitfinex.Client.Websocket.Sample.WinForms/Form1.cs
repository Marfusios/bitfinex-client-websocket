using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bitfinex.Client.Websocket.Sample.WinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(BitfinexApi.Start, TaskCreationOptions.LongRunning);
        }
    }
}
