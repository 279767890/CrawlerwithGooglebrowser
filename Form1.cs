using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 谷歌内核浏览器
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitBrowser();
        }
        public ChromiumWebBrowser browser;//定义一个谷歌浏览器
        public void InitBrowser()
        {
            Cef.Initialize(new CefSettings());
            browser = new ChromiumWebBrowser("http://wenshu.court.gov.cn/website/wenshu/181217BMTKHNT2W0/index.html?pageId=9aac9de52aeb9304f54122dd34650d25&s21=%E8%99%9A%E5%81%87%E9%99%88%E8%BF%B0");
            this.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            browser.GetBrowser().MainFrame.ExecuteJavaScriptAsync("document.getElementsByClassName('pageButton')[(document.getElementsByClassName('pageButton').length)-1].click();");

            browser_FrameLoadEnd(null, null);

        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "开始")
            {
                EvaluateScriptAsync(5);
                timer.Enabled = true;
                button2.Text = "暂停";
            }
            else if (button2.Text == "暂停")
            {
                timer.Enabled = false;
                button2.Text = "开始";
            }

        }

        //定义Timer类
        System.Timers.Timer timer;
        /// <summary>
        /// 页面加载完成后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {

            var result = await browser.GetSourceAsync();//获取当前页面的全部网页


            EvaluateScriptAsync(5);



        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitTimer();// 加载定时器 
            //browser.FrameLoadEnd += browser_FrameLoadEnd;
        }
        /// <summary>
        /// 获取案件div
        /// </summary>
        /// <param name="pagesize">每页显示的多少行</param>
        async void EvaluateScriptAsync(int pagesize)
        {
            string result = "";
            string result2 = "";
            for (int i = 0; i < pagesize; i++)
            {
                string script = "document.getElementsByClassName('LM_list')[" + i + "].innerHTML";
                JavascriptResponse response = await browser.GetBrowser().MainFrame.EvaluateScriptAsync(script);
                if (response != null)
                    result += response.Result;

                string script2 = "document.getElementsByClassName('caseName')[" + i + "].href";
                JavascriptResponse response2 = await browser.GetBrowser().MainFrame.EvaluateScriptAsync(script2);
                if (response2 != null)
                    result2 += response2.Result+ "\r\n";
            }


        }
        /// <summary>
        /// 初始化定时器控件
        /// </summary>
        private void InitTimer()
        {
            //设置定时间隔(毫秒为单位)
            int interval = 5000;
            timer = new System.Timers.Timer(interval);
            //设置执行一次（false）还是一直执行(true)
            timer.AutoReset = true;
            //设置是否执行System.Timers.Timer.Elapsed事件
            timer.Enabled = false;
            //绑定Elapsed事件
            timer.Elapsed += new System.Timers.ElapsedEventHandler(TimerUp);
        }
        // <summary>
        /// Timer类执行定时到点事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerUp(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                browser.GetBrowser().MainFrame.ExecuteJavaScriptAsync("document.getElementsByClassName('pageButton')[(document.getElementsByClassName('pageButton').length)-1].click();");

                browser_FrameLoadEnd(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("执行定时到点事件失败:" + ex.Message);
            }
        }


    }
}
