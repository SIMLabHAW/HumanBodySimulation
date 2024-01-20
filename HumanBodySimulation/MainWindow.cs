using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HumanBodySimulation
{
    public partial class MainWindow : Form
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        List<IOrgan> organs = new List<IOrgan>();
        List<Heart> hearts = new List<Heart>();
        List<Lung> lungs = new List<Lung>();

        public MainWindow()
        {
            InitializeComponent();

            initializeOrgans();
        }

        public void initializeOrgans()
        {
            // Add Organs here
            organs.Add(new Lung());

            // Added the Heart here
            hearts.Add(new Heart());

         

            foreach (IOrgan organ in organs)
            {
                organ.init(parameters);
            }
        }

        private void start_Click(object sender, EventArgs e)
        {
            timer.Start();
        }

        private void stop_Click(object sender, EventArgs e)
        {
            timer.Stop();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            foreach(IOrgan organ in organs)
            {
                organ.update((int)simStepSize.Value, parameters);
            }
        }
    }
}
