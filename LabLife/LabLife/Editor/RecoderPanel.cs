using LabLife.Contorols;
using LabLife.Processer;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
//using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace LabLife.Editor
{
    public class RecoderPanel : AImageResourcePanel
    {

        private ListBox ListBox_ResourcePanels = new ListBox();

        private ListBox ListBox_Recoder = new ListBox();

        private ListBox ListBox_ProjectionPanel = new ListBox();

        public List<Recoder> List_Recoder = new List<Recoder>();

        private DateTime starttime;
        public bool IsRecStarted { get; set; }

        private uint currentframe;
        private BinaryWriter writer;
        private BinaryReader reader;
        private byte[] data;

        VideoWriter vw;
        OpenCvSharp.CPlusPlus.VideoCapture vc ;

        List<AImageResourcePanel> resourcePanels;
        private SliderAndTextControl SliderTextBox;
        private int saveImageIndex;
        private TextBox textbox_SavefileName = new TextBox();
        private TextBox textbox_LoadfileName = new TextBox();
        private ComboBox size = new ComboBox();
    
        private Image Image_Load = new Image();
        private WriteableBitmap WriteableBitmap_Load;

        string savefilename;
        string loadfilename;
        string savefolder;
        string loadfolder;

        Mat loadmat;
        WriteableBitmap loadbit;

        public RecoderPanel()
        {
            base.TitleName = "Recoder";

        }

        public override void Initialize(MainWindow mainwindow)
        {
            base.Initialize(mainwindow);

            UniformGrid grid = new UniformGrid();
            grid.Columns = 2;
            Border b1 = new Border();
            b1.Style = (Style)App.Current.Resources["Border_Default"];
            b1.Child = this.ListBox_ResourcePanels;
            this.ListBox_ResourcePanels.SelectionChanged += ListBox_ResourcePanels_SelectionChanged;
            //UniformGrid SaveGrid = new UniformGrid();
            //SaveGrid.Rows = 2;
            StackPanel savestack = new StackPanel();
            TextBlock textblock_save = new TextBlock();
            textblock_save.Text = "保存用";
            Button Button_Save = new Button();
            Button_Save.Click += Button_Save_Click;
            Button_Save.Content = "保存";
            Button Button_Stop_save = new Button();
            Button_Stop_save.Click += Button_Stop_save_Click;
            Button_Stop_save.Content = "停止";
            Button Button_savefolder = new Button();
            Button_savefolder.Click += Button_savefile_Click;
            Button_savefolder.Content = "フォルダ選択";
            
            textbox_SavefileName.Text = "name";
            Button Button_FileName = new Button();
            Button_FileName.Click += Button_FileName_Click;
            Button_FileName.Content = "名前変更";
            
            size.Items.Add("320,240");
            size.Items.Add("512,424");

            SliderTextBox = new SliderAndTextControl();
            savestack.Children.Add(SliderTextBox);
            SliderTextBox.TextBlock_Title.Text = "Image Index : ";
            SliderTextBox.Slider_Main.ValueChanged += Slider_Main_ValueChanged;

            savestack.Children.Add(textblock_save);
            savestack.Children.Add(Button_Save);
            
            savestack.Children.Add(Button_Stop_save);
            savestack.Children.Add(Button_savefolder);
            savestack.Children.Add(textbox_SavefileName);
            savestack.Children.Add(Button_FileName);
            savestack.Children.Add(size);
            grid.Children.Add(savestack);
            grid.Children.Add(b1);

            Grid.SetColumn(b1, 0);

            Border b2 = new Border();
            b2.Style = (Style)App.Current.Resources["Border_Default"];
            StackPanel loadstack = new StackPanel();
            TextBlock textblock_load = new TextBlock();
            textblock_load.Text = "再生用";
            Button Button_Loadone = new Button();
            Button_Loadone.Click += Button_Loadone_Click; 
            Button_Loadone.Content = "読み込み";
            Button Button_Load = new Button();
            Button_Load.Click += Button_Load_Click;
            Button_Load.Content = "再生";
            Button Button_Stop_load = new Button();
            Button_Stop_load.Click += Button_Stop_load_Click; ;
            Button_Stop_load.Content = "終了";
            Button Button_loadfolder = new Button();
            Button_loadfolder.Click += Button_loadfolder_Click; 
            Button_loadfolder.Content = "フォルダ選択";
            textbox_LoadfileName.Text = "name";
            Button Button_LoadFileName = new Button();
            Button_LoadFileName.Click += Button_LoadFileName_Click; ;
            Button_LoadFileName.Content = "名前変更";

            loadstack.Children.Add(textblock_load);
            loadstack.Children.Add(Button_Loadone);
            loadstack.Children.Add(Button_Load);
            loadstack.Children.Add(Button_Stop_load);
            loadstack.Children.Add(Button_loadfolder);
            loadstack.Children.Add(textbox_LoadfileName);
            loadstack.Children.Add(Button_LoadFileName);

            grid.Children.Add(loadstack);
            grid.Children.Add(b2);
            Grid.SetColumn(b2, 1);


            StackPanel stackpanel = new StackPanel();


            Button Button_Update = new Button();
            Button_Update.Content = "Update";
            Button_Update.Click += Button_Update_Click;

            Button Button_Delete = new Button();
            Button_Delete.Click += Button_Delete_Click;
            Button_Delete.Content = "Delete";


            //stackpanel.Children.Add(Button_Add);
            stackpanel.Children.Add(Button_Update);

            stackpanel.Children.Add(Button_Delete);
            base.AddContent(stackpanel, Dock.Top);
            base.AddContent(grid, Dock.Top);

            this.SetImageToGridChildren(this.Image_Load);
            Image_Load.Source = loadbit;
            this.AddContent(base.Grid_Image, Dock.Bottom);

            this.UpdateLists();

            var timer = new Timer(new TimerCallback(loadtimercallback), null, 0, 30);

        }

        private void Button_Loadone_Click(object sender, RoutedEventArgs e)
        {
            playinit();
            PlayVideoOnce();
        }

        private void Button_loadfolder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog2 = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog2.Description = "フォルダを選択";
            folderBrowserDialog2.RootFolder = System.Environment.SpecialFolder.MyComputer;
            folderBrowserDialog2.SelectedPath = "C:\\Windows";
            folderBrowserDialog2.ShowNewFolderButton = false;
            if (folderBrowserDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.loadfolder = folderBrowserDialog2.SelectedPath;
            }
            folderBrowserDialog2.Dispose();
        }

        private void loadtimercallback(object state)
        {
            try
            {
                if (loadmat == null)
                {
                    loadmat = new Mat(320, 240, MatType.CV_8UC4);

                }

                OnImageFrameArrived(new ImageFrameArrivedEventArgs(new Mat[] { loadmat }));

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void Button_LoadFileName_Click(object sender, RoutedEventArgs e)
        {
            loadfilename = textbox_LoadfileName.Text;
        }

        private void Button_Stop_load_Click(object sender, RoutedEventArgs e)
        {
            if(vc != null)
            {
                vc.Dispose();
                vc = new VideoCapture();
            }
            
        }

        private void Button_FileName_Click(object sender, RoutedEventArgs e)
        {
            savefilename = textbox_SavefileName.Text;
       
        }

        private void Button_Stop_save_Click(object sender, RoutedEventArgs e)
        {
            if (vw != null)
            {
                vw.Dispose();
                vw = new VideoWriter();
            }
        }

        private void Button_savefile_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog1.Description = "フォルダを選択";
            folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            folderBrowserDialog1.SelectedPath = "C:\\Windows";
            folderBrowserDialog1.ShowNewFolderButton = true;
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.savefolder = folderBrowserDialog1.SelectedPath;
            }
            folderBrowserDialog1.Dispose();

        }

        private void Slider_Main_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.SliderTextBox.Slider_Main.Value = ((int)(this.SliderTextBox.Slider_Main.Value));
        }

        private void ListBox_ResourcePanels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ListBox_ResourcePanels.SelectedIndex < 0)
            {
                return;
            }

            this.SliderTextBox.Slider_Main.Maximum = resourcePanels[this.ListBox_ResourcePanels.SelectedIndex].ImageNum - 1;
            this.SliderTextBox.Slider_Main.Minimum = 0;
            this.SliderTextBox.Slider_Main.Value = 0;
        }

        private void Button_Load_Click(object sender, RoutedEventArgs e)
        {

            playinit();
            Task task = new Task(new Action(this.PlayVideo));
            task.Start();
            
        }

        private void playinit()
        {
            vc = new VideoCapture();
            if (textbox_LoadfileName.ToString() == "name")
            {
                Console.WriteLine("nofile");
                return;
            }
            else
            {
                vc.Open(loadfolder + @"\" + loadfilename + ".avi");
            }
        }

        private void PlayVideo()
        {
            //this.Image_Load.Dispatcher.BeginInvoke()
            base.Grid_Image.Dispatcher.BeginInvoke(new Action(() =>
            {

                //loadbit= new WriteableBitmap( loadmat.Width, loadmat.Height, 96, 96, PixelFormats.Rgb24,new BitmapPalette() );

            }));

            while (true)
            {
                try
                {
                    vc.Read(loadmat);
                    Cv2.ImShow("playtest", loadmat);
                    base.Grid_Image.Dispatcher.BeginInvoke(new Action(() =>
                    {

                        loadbit.WritePixels(new Int32Rect(0,0,loadmat.Width,loadmat.Height),loadmat.CvPtr,loadmat.Width * loadmat.Channels(),0) ;

                    }));

                    //OnImageFrameArrived(new ImageFrameArrivedEventArgs(new Mat[] { mat }));
                    Cv2.WaitKey((int)(1000 / vc.Fps));
                }
                catch
                {
                    Console.WriteLine("終了");
                    Cv2.DestroyAllWindows();
                    vc.Dispose();
                    break;
                }
            }
        }

        private void PlayVideoOnce()
        {

            while (true)
            {
                try
                {
                    vc.Read(loadmat);
                    Cv2.ImShow("playtest", loadmat);
                    loadbit = WriteableBitmapConverter.ToWriteableBitmap(loadmat);

                    //OnImageFrameArrived(new ImageFrameArrivedEventArgs(new Mat[] { mat }));
                    Cv2.WaitKey((int)(1000 / vc.Fps));
                    break;
                }
                catch
                {
                    Console.WriteLine("終了");
                    Cv2.DestroyAllWindows();
                    vc.Dispose();
                    break;
                }
            }
        }

        public void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CvSize sz;
                string strRECName;

                if (savefilename == "name")
                {
                    strRECName = savefolder + @"\"+ "video" + "noname" + ".avi";
                }
                else
                {
                    strRECName = savefolder+ @"\" + savefilename + ".avi";
                }
                if (size.SelectedItem.ToString() == "320,240")
                {
                    sz = new CvSize(320, 240);
                }
                else if (size.SelectedItem.ToString() == "512,424")
                {
                    sz = new CvSize(512, 424);
                }
                else return;

                vw = new VideoWriter();
                vw.Open(strRECName, -1, 30, sz, true);

                var panel = this.resourcePanels[this.ListBox_ResourcePanels.SelectedIndex];
                this.starttime = DateTime.Now;
                this.currentframe = 0;
                this.IsRecStarted = true;
                this.saveImageIndex = (int)this.SliderTextBox.Slider_Main.Value;
                panel.ImageFrameArrived += Panel_ImageFrameArrived;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void Panel_ImageFrameArrived(object Sender, ImageFrameArrivedEventArgs e)
        {
            vw.Write(e.Image[this.saveImageIndex]);

        }
        

        private void Button_Update_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateLists();
        }

        public void UpdateLists()
        {
            this.resourcePanels = base.m_MainWindow.GetPanels<AImageResourcePanel>();

            if (resourcePanels != null)
            {
                this.ListBox_ResourcePanels.Items.Clear();
                foreach (var p in resourcePanels)
                {

                    this.ListBox_ResourcePanels.Items.Add(p.TitleName);

                }
                this.ListBox_ResourcePanels.SelectedIndex = 0;
            }

        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {

            //this.List_Recoder.RemoveAt(this.ListBox_Recoder.SelectedIndex);

        }

        protected virtual void REC_data()
        {
            if (this.IsRecStarted)
            {
                this.writer.Write((uint)this.currentframe);
                //this.writer.Write((long)); //時間
                this.writer.Write((int)this.data.Length);
                this.writer.Write(this.data);
            }
        }

    }
}
