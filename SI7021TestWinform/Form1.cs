using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using ZedGraph;
using SI7021Module;

namespace SI7021TestWinform
{
    public partial class Form1 : Form
    {
        System.Timers.Timer pollingTimers;

		Random rnd;
        RollingPointPairList temperatureSeries;
        RollingPointPairList humiditySeries;
		SI7021Sensor _siSensor;
        public Form1()
        {
            InitializeComponent();
            pollingTimers = new System.Timers.Timer();
            pollingTimers.Interval = 200;
            pollingTimers.Elapsed += PollingTimers_Elapsed;

			rnd = new Random();
			_siSensor = new SI7021Sensor();
            initGraph();
        }

        private void initGraph()
        {
            temperatureSeries = new RollingPointPairList(300);
            humiditySeries = new RollingPointPairList(300);

            zedGraphControl1.GraphPane.Title.Text = "Temperature/Humidity Chart with SI7021 sensors";
			zedGraphControl1.GraphPane.XAxis.Type = AxisType.Date;
			zedGraphControl1.GraphPane.XAxis.Scale.Format = "HH:mm:ss";
            zedGraphControl1.GraphPane.AddCurve("Temperature", temperatureSeries, Color.Green,SymbolType.None);
			zedGraphControl1.GraphPane.AddCurve("Humidity", humiditySeries, Color.Blue,SymbolType.None);
            updateAxis();

        }

        private delegate void UpdateAxis();
        private void updateAxis()
        {
            if(zedGraphControl1.InvokeRequired)
            {
                this.Invoke(new UpdateAxis(updateAxis));
                return;
            }
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }
        private void clearGraph()
        {
            temperatureSeries.Clear();
            humiditySeries.Clear();
        }
        private void updateChart(double temperature, double humidity)
        {
            XDate x = new XDate(DateTime.Now);

            temperatureSeries.Add(x, temperature);
            humiditySeries.Add(x, humidity);
            updateAxis();
        }

        private void PollingTimers_Elapsed(object sender, ElapsedEventArgs e)
        {
			double t = _siSensor.ReadTemp();
			double h = _siSensor.ReadRH();
			updateChart(t,h);
            
        }
        bool isStart = false;
        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (!isStart)
            {
                clearGraph();
                pollingTimers.Start();
                buttonStart.Text = "Stop";
            }
            else
            {
                pollingTimers.Stop();
                buttonStart.Text = "Start";
            }
            isStart = !isStart;

        }
    }
}
